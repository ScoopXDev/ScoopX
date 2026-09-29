# Review package — Task 1
BASE: 0b17adbd (working tree; no commits)
Note: includes untracked Services/

## Status
 M App.xaml.cs
?? Services/

## Diff App.xaml.cs

diff --git a/App.xaml.cs b/App.xaml.cs
index 3b29fcc..aa0a4d7 100644
--- a/App.xaml.cs
+++ b/App.xaml.cs
@@ -1,23 +1,27 @@
 锘縰sing Microsoft.UI.Xaml;
+using ScoopX.Services;
 
 namespace ScoopX
 {
     public partial class App : Application
     {
         private Window? _window;
 
         public static Window? MainWindow { get; private set; }
 
         public App()
         {
             InitializeComponent();
+            _ = ThemeService.Instance;
         }
 
         protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
         {
+            ThemeService.Instance.ApplyApplicationThemeOnly();
             _window = new MainWindow();
             MainWindow = _window;
+            ThemeService.Instance.Initialize(_window.DispatcherQueue);
             _window.Activate();
         }
     }
 }

## New file: Services/AppThemePreference.cs

namespace ScoopX.Services
{
    public enum AppThemePreference
    {
        System,
        Light,
        Dark
    }
}

## New file: Services/ThemeService.cs

using System;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace ScoopX.Services
{
    public sealed class ThemeService
    {
        public const string SettingsKey = "AppTheme";
        private static readonly Lazy<ThemeService> Lazy = new(() => new ThemeService());
        public static ThemeService Instance => Lazy.Value;

        private readonly UISettings _uiSettings = new();
        private DispatcherQueue? _dispatcherQueue;
        private bool _listening;

        public AppThemePreference Preference { get; private set; } = AppThemePreference.System;

        public event EventHandler? ThemeChanged;

        private ThemeService() { }

        public void Initialize(DispatcherQueue dispatcherQueue)
        {
            _dispatcherQueue = dispatcherQueue;
            Preference = ReadPreference();
            Apply();
            EnsureSystemListener();
        }

        public void SetPreference(AppThemePreference preference)
        {
            Preference = preference;
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = preference.ToString();
            Apply();
            EnsureSystemListener();
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ApplyApplicationThemeOnly()
        {
            var app = Application.Current;
            if (app is null)
            {
                return;
            }

            SetApplicationRequestedTheme(ReadPreference());
        }

        public void Apply()
        {
            var app = Application.Current;
            if (app is null)
            {
                return;
            }

            SetApplicationRequestedTheme(Preference);

            if (App.MainWindow?.Content is FrameworkElement root)
            {
                root.RequestedTheme = Preference switch
                {
                    AppThemePreference.Light => ElementTheme.Light,
                    AppThemePreference.Dark => ElementTheme.Dark,
                    _ => ElementTheme.Default
                };
            }
        }

        private static void SetApplicationRequestedTheme(AppThemePreference preference)
        {
            var app = Application.Current;
            if (app is null)
            {
                return;
            }

            app.RequestedTheme = preference switch
            {
                AppThemePreference.Light => ApplicationTheme.Light,
                AppThemePreference.Dark => ApplicationTheme.Dark,
                _ => ResolveSystemTheme()
            };
        }

        private static ApplicationTheme ResolveSystemTheme()
        {
            var color = new UISettings().GetColorValue(UIColorType.Background);
            var isDark = (color.R + color.G + color.B) < (128 * 3);
            return isDark ? ApplicationTheme.Dark : ApplicationTheme.Light;
        }

        private static AppThemePreference ReadPreference()
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(SettingsKey, out var value)
                && value is string s
                && Enum.TryParse<AppThemePreference>(s, ignoreCase: true, out var parsed))
            {
                return parsed;
            }

            return AppThemePreference.System;
        }

        private void EnsureSystemListener()
        {
            if (Preference == AppThemePreference.System)
            {
                if (_listening)
                {
                    return;
                }

                _uiSettings.ColorValuesChanged += OnColorValuesChanged;
                _listening = true;
            }
            else if (_listening)
            {
                _uiSettings.ColorValuesChanged -= OnColorValuesChanged;
                _listening = false;
            }
        }

        private void OnColorValuesChanged(UISettings sender, object args)
        {
            if (Preference != AppThemePreference.System)
            {
                return;
            }

            void ApplyOnUi()
            {
                Apply();
                ThemeChanged?.Invoke(this, EventArgs.Empty);
            }

            if (_dispatcherQueue is null || _dispatcherQueue.HasThreadAccess)
            {
                ApplyOnUi();
            }
            else
            {
                _ = _dispatcherQueue.TryEnqueue(ApplyOnUi);
            }
        }
    }
}
