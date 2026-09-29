# Task 1 language review package
diff --git a/Services/LanguageService.cs b/Services/LanguageService.cs
new file mode 100644
index 0000000..d62bb76
--- /dev/null
+++ b/Services/LanguageService.cs
@@ -0,0 +1,56 @@
+using System;
+using Windows.Globalization;
+using Windows.Storage;
+
+namespace ScoopX.Services
+{
+    public sealed class LanguageService
+    {
+        public const string SettingsKey = "AppLanguage";
+        public const string ZhCn = "zh-CN";
+        public const string EnUs = "en-US";
+
+        private static readonly Lazy<LanguageService> Lazy = new(() => new LanguageService());
+        public static LanguageService Instance => Lazy.Value;
+
+        public string CurrentTag { get; private set; } = ZhCn;
+
+        private LanguageService() { }
+
+        public void ApplyBeforeUi()
+        {
+            CurrentTag = ReadTag();
+            ApplicationLanguages.PrimaryLanguageOverride = CurrentTag;
+        }
+
+        public void SetLanguage(string tag)
+        {
+            var normalized = Normalize(tag);
+            CurrentTag = normalized;
+            ApplicationData.Current.LocalSettings.Values[SettingsKey] = normalized;
+            ApplicationLanguages.PrimaryLanguageOverride = normalized;
+        }
+
+        private static string ReadTag()
+        {
+            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(SettingsKey, out var v)
+                && v is string s)
+            {
+                return Normalize(s);
+            }
+
+            return ZhCn;
+        }
+
+        private static string Normalize(string tag)
+        {
+            if (string.Equals(tag, EnUs, StringComparison.OrdinalIgnoreCase)
+                || string.Equals(tag, "en", StringComparison.OrdinalIgnoreCase))
+            {
+                return EnUs;
+            }
+
+            return ZhCn;
+        }
+    }
+}

## App.xaml.cs diff

diff --git a/App.xaml.cs b/App.xaml.cs
index 3b29fcc..6b805b6 100644
--- a/App.xaml.cs
+++ b/App.xaml.cs
@@ -1,4 +1,5 @@
 锘縰sing Microsoft.UI.Xaml;
+using ScoopX.Services;
 
 namespace ScoopX
 {
@@ -10,6 +11,9 @@ namespace ScoopX
 
         public App()
         {
+            // Application.RequestedTheme must be set before InitializeComponent.
+            ThemeService.Instance.ApplyApplicationThemeBeforeInit(this);
+            LanguageService.Instance.ApplyBeforeUi();
             InitializeComponent();
         }
 
@@ -17,6 +21,7 @@ namespace ScoopX
         {
             _window = new MainWindow();
             MainWindow = _window;
+            ThemeService.Instance.Initialize(_window.DispatcherQueue);
             _window.Activate();
         }
     }
            // Application.RequestedTheme must be set before InitializeComponent.
            ThemeService.Instance.ApplyApplicationThemeBeforeInit(this);
            LanguageService.Instance.ApplyBeforeUi();
            ThemeService.Instance.Initialize(_window.DispatcherQueue);
