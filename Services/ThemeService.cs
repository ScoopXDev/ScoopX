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

        /// <summary>Current Light/Dark for FrameworkElement.RequestedTheme (ContentDialog must set this explicitly).</summary>
        public ElementTheme CurrentElementTheme => ResolveElementTheme(Preference);

        public event EventHandler? ThemeChanged;

        private ThemeService() { }

        /// <summary>
        /// Must run in App() before InitializeComponent().
        /// Only forces Application.RequestedTheme for Light/Dark; System leaves the default (follow OS).
        /// </summary>
        public void ApplyApplicationThemeBeforeInit(Application app)
        {
            Preference = ReadPreference();

            if (Preference == AppThemePreference.Light)
            {
                app.RequestedTheme = ApplicationTheme.Light;
            }
            else if (Preference == AppThemePreference.Dark)
            {
                app.RequestedTheme = ApplicationTheme.Dark;
            }
            // System: do not set Application.RequestedTheme — default follows Windows.
        }

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
            // Never touch Application.RequestedTheme after startup — WinUI throws COMException.
            Apply();
            EnsureSystemListener();
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Apply()
        {
            if (App.MainWindow?.Content is FrameworkElement root)
            {
                root.RequestedTheme = ResolveElementTheme(Preference);
            }
        }

        private static ApplicationTheme ResolveSystemTheme()
        {
            var color = new UISettings().GetColorValue(UIColorType.Background);
            var isDark = (color.R + color.G + color.B) < (128 * 3);
            return isDark ? ApplicationTheme.Dark : ApplicationTheme.Light;
        }

        private static ElementTheme ResolveElementTheme(AppThemePreference preference)
        {
            return preference switch
            {
                AppThemePreference.Light => ElementTheme.Light,
                AppThemePreference.Dark => ElementTheme.Dark,
                _ => ResolveSystemTheme() == ApplicationTheme.Dark
                    ? ElementTheme.Dark
                    : ElementTheme.Light
            };
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
