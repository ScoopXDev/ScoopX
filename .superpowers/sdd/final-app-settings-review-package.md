# Final review package — app-settings plan
Plan: docs/superpowers/plans/2026-09-29-app-settings.md
Spec: docs/superpowers/specs/2026-09-29-app-settings-design.md

## Scope files (app-settings only; ignore unrelated dark-theme hunks elsewhere)
- Services/AppSettings.cs (new)
- Package.appxmanifest (StartupTask / uap5)
- MainWindow.xaml.cs (CloseToTray / StartHiddenToTray only)
- Pages/SettingsPage.xaml (new/updated full page)
- Pages/SettingsPage.xaml.cs

## Minor findings from task reviews (triage)
- StartHidden Activated may miss if already activated
- StartWithWindows UI should await (done in T4); setter still fire-and-forget
- Manual QA checklist not run
- Path TextBoxes no existence validation (by design)

## Package.appxmanifest diff vs HEAD
diff --git a/MainWindow.xaml.cs b/MainWindow.xaml.cs
index 589187d..5bb89d9 100644
--- a/MainWindow.xaml.cs
+++ b/MainWindow.xaml.cs
@@ -9,6 +9,7 @@ using Microsoft.UI.Xaml.Media;
 using Microsoft.UI.Xaml.Media.Animation;
 using ScoopX.Controls;
 using ScoopX.Pages;
+using ScoopX.Services;
 using Windows.Foundation;
 using Windows.UI;
 
@@ -16,9 +17,6 @@ namespace ScoopX
 {
     public sealed partial class MainWindow : Window
     {
-        private static readonly SolidColorBrush ActiveBrush = new(Color.FromArgb(255, 37, 99, 235)); // #2563EB 鍏ㄥ眬涓昏壊
-        private static readonly SolidColorBrush InactiveBrush = new(Color.FromArgb(255, 75, 85, 99));
-
         private string _selectedTag = "Home";
         private Storyboard? _pillStoryboard;
         private bool _railReady;
@@ -62,20 +60,70 @@ namespace ScoopX
 
             SetWindowIcon();
             WireNavHover();
+            ThemeService.Instance.ThemeChanged += (_, _) => RefreshNavColors();
             ApplyNavSelection("Home", animatePill: false);
             NavigateTo("Home");
+
+            if (AppSettings.Instance.StartHiddenToTray)
+            {
+                // 寤惰繜鍒扮獥鍙ｆ縺娲诲悗鍐嶈棌锛岄伩鍏嶉棯涓€涓嬪張绔嬪埢 Hide 鐨勭珵鎬?+                void OnActivated(object sender, WindowActivatedEventArgs args)
+                {
+                    Activated -= OnActivated;
+                    HideToTray();
+                }
+                Activated += OnActivated;
+            }
+        }
+
+        private SolidColorBrush ActiveBrush =>
+            (SolidColorBrush)Application.Current.Resources["ScoopAccentBrush"];
+
+        private SolidColorBrush InactiveBrush =>
+            (SolidColorBrush)GetThemeBrush("ScoopNavInactiveBrush");
+
+        private void RefreshNavColors()
+        {
+            ApplyNavSelection(_selectedTag, animatePill: false);
+        }
+
+        private static string GetThemeDictionaryKey(FrameworkElement root)
+        {
+            var theme = root.ActualTheme;
+            if (theme == ElementTheme.Default)
+            {
+                theme = Application.Current.RequestedTheme == ApplicationTheme.Dark
+                    ? ElementTheme.Dark
+                    : ElementTheme.Light;
+            }
+
+            return theme == ElementTheme.Dark ? "Dark" : "Light";
+        }
+
+        private Brush GetThemeBrush(string key)
+        {
+            var root = (FrameworkElement)Content;
+            var dicts = Application.Current.Resources.ThemeDictionaries;
+            var rd = (ResourceDictionary)dicts[GetThemeDictionaryKey(root)];
+            return (Brush)rd[key];
         }
 
         private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
         {
-            // 闈炰富鍔ㄩ€€鍑烘椂锛屽叧闂彧鏄棌鍒版墭鐩?             if (_isExitRequested)
             {
                 return;
             }
 
-            args.Cancel = true;
-            HideToTray();
+            if (AppSettings.Instance.CloseToTray)
+            {
+                args.Cancel = true;
+                HideToTray();
+                return;
+            }
+
+            // 鍏佽鍏抽棴锛氬厛娓呯悊鎵樼洏
+            TrayIcon.Dispose();
         }
 
         private void MinimizeButton_Click(object sender, RoutedEventArgs e)
@@ -88,7 +136,14 @@ namespace ScoopX
 
         private void CloseButton_Click(object sender, RoutedEventArgs e)
         {
-            HideToTray();
+            if (AppSettings.Instance.CloseToTray)
+            {
+                HideToTray();
+            }
+            else
+            {
+                ExitApplication();
+            }
         }
 
         private void TrayShow_Click(object sender, RoutedEventArgs e)
@@ -192,6 +247,9 @@ namespace ScoopX
                 case "Websites":
                     ContentFrame.Navigate(typeof(WebsitesPage));
                     break;
+                case "Settings":
+                    ContentFrame.Navigate(typeof(SettingsPage));
+                    break;
                 default:
                     ContentFrame.Content = null;
                     break;
@@ -316,7 +374,7 @@ namespace ScoopX
             }
         }
 
-        private static void SetNavItemState(FontIcon icon, TextBlock label, bool selected)
+        private void SetNavItemState(FontIcon icon, TextBlock label, bool selected)
         {
             icon.Foreground = selected ? ActiveBrush : InactiveBrush;
             label.Foreground = selected ? ActiveBrush : InactiveBrush;
diff --git a/Package.appxmanifest b/Package.appxmanifest
index 74a6a66..277cfd6 100644
--- a/Package.appxmanifest
+++ b/Package.appxmanifest
@@ -4,9 +4,10 @@
   xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
   xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest"
   xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
+  xmlns:uap5="http://schemas.microsoft.com/appx/manifest/uap/windows10/5"
   xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
   xmlns:systemai="http://schemas.microsoft.com/appx/manifest/systemai/windows10"
-  IgnorableNamespaces="uap rescap systemai">
+  IgnorableNamespaces="uap uap5 rescap systemai">
 
   <Identity
     Name="34114be7-3b1f-4822-bcf4-971a979670c2"
@@ -43,6 +44,14 @@
         <uap:DefaultTile Wide310x150Logo="Assets\Wide310x150Logo.png" />
         <uap:SplashScreen Image="Assets\SplashScreen.png" />
       </uap:VisualElements>
+      <Extensions>
+        <uap5:Extension Category="windows.startupTask">
+          <uap5:StartupTask
+            TaskId="ScoopXStartup"
+            Enabled="false"
+            DisplayName="ScoopX" />
+        </uap5:Extension>
+      </Extensions>
     </Application>
   </Applications>
 

## AppSettings.cs

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace ScoopX.Services
{
    public sealed class AppSettings
    {
        public const string StartupTaskId = "ScoopXStartup";
        private const string KeyCloseToTray = "CloseToTray";
        private const string KeyStartWithWindows = "StartWithWindows";
        private const string KeyStartHiddenToTray = "StartHiddenToTray";
        private const string KeyScoopRootPath = "ScoopRootPath";
        private const string KeyWebsitesRootPath = "WebsitesRootPath";

        private static readonly Lazy<AppSettings> Lazy = new(() => new AppSettings());
        public static AppSettings Instance => Lazy.Value;

        public event EventHandler? Changed;

        private AppSettings() { }

        public bool CloseToTray
        {
            get => ReadBool(KeyCloseToTray, defaultValue: true);
            set { WriteBool(KeyCloseToTray, value); RaiseChanged(); }
        }

        public bool StartHiddenToTray
        {
            get => ReadBool(KeyStartHiddenToTray, defaultValue: false);
            set { WriteBool(KeyStartHiddenToTray, value); RaiseChanged(); }
        }

        public string ScoopRootPath
        {
            get => ReadString(KeyScoopRootPath);
            set { WriteString(KeyScoopRootPath, value ?? ""); RaiseChanged(); }
        }

        public string WebsitesRootPath
        {
            get => ReadString(KeyWebsitesRootPath);
            set { WriteString(KeyWebsitesRootPath, value ?? ""); RaiseChanged(); }
        }

        public bool StartWithWindows
        {
            get => ReadBool(KeyStartWithWindows, defaultValue: false);
            set => _ = SetStartWithWindowsAsync(value);
        }

        public async Task<(bool ok, string? error)> SetStartWithWindowsAsync(bool enabled)
        {
            try
            {
                var task = await StartupTask.GetAsync(StartupTaskId);
                if (enabled)
                {
                    var state = await task.RequestEnableAsync();
                    if (state is StartupTaskState.Disabled
                        or StartupTaskState.DisabledByUser
                        or StartupTaskState.DisabledByPolicy)
                    {
                        WriteBool(KeyStartWithWindows, false);
                        RaiseChanged();
                        return (false, "鏈兘鍚敤寮€鏈鸿嚜鍚紙鍙兘琚郴缁熸垨鐢ㄦ埛绛栫暐绂佺敤锛夈€?);
                    }
                }
                else
                {
                    task.Disable();
                }

                WriteBool(KeyStartWithWindows, enabled);
                RaiseChanged();
                return (true, null);
            }
            catch (Exception ex)
            {
                // unpackaged / missing extension
                WriteBool(KeyStartWithWindows, false);
                RaiseChanged();
                return (false, $"寮€鏈鸿嚜鍚笉鍙敤锛堥渶瀹夎鍖呯増鏈級锛歿ex.Message}");
            }
        }

        private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);

        private static bool ReadBool(string key, bool defaultValue)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var v) && v is bool b)
                return b;
            return defaultValue;
        }

        private static void WriteBool(string key, bool value) =>
            ApplicationData.Current.LocalSettings.Values[key] = value;

        private static string ReadString(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var v) && v is string s)
                return s;
            return "";
        }

        private static void WriteString(string key, string value) =>
            ApplicationData.Current.LocalSettings.Values[key] = value;
    }
}

## Instruct reviewer to Read Pages/SettingsPage.xaml and .xaml.cs on disk

