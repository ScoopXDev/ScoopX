# Task 3 review package
Focus ONLY on CloseToTray / StartHiddenToTray / AppSettings wiring in MainWindow.xaml.cs.
Ignore unrelated dark-theme hunks if present in the same file.

## Diff (MainWindow.xaml.cs vs HEAD)
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
