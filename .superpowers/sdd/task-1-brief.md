### Task 1: ThemeService + preference storage

**Files:**
- Create: `Services/AppThemePreference.cs`
- Create: `Services/ThemeService.cs`
- Modify: `App.xaml.cs`

**Interfaces:**
- Produces: `enum AppThemePreference { System, Light, Dark }`
- Produces: `ThemeService.Instance` with `AppThemePreference Preference { get; }`, `void Initialize()`, `void SetPreference(AppThemePreference preference)`, `event EventHandler? ThemeChanged`
- Consumes: `ApplicationData.Current.LocalSettings`, `Application.Current.RequestedTheme`, `UISettings.ColorValuesChanged`

- [ ] **Step 1: Add preference enum**

Create `Services/AppThemePreference.cs`:

```csharp
namespace ScoopX.Services
{
    public enum AppThemePreference
    {
        System,
        Light,
        Dark
    }
}
```

- [ ] **Step 2: Implement ThemeService**

Create `Services/ThemeService.cs`:

```csharp
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

        public void Apply()
        {
            var app = Application.Current;
            if (app is null)
            {
                return;
            }

            app.RequestedTheme = Preference switch
            {
                AppThemePreference.Light => ApplicationTheme.Light,
                AppThemePreference.Dark => ApplicationTheme.Dark,
                _ => Application.Current.RequestedTheme == ApplicationTheme.Dark
                    ? ApplicationTheme.Dark
                    : ResolveSystemTheme()
            };

            // System: map OS theme explicitly
            if (Preference == AppThemePreference.System)
            {
                app.RequestedTheme = ResolveSystemTheme();
            }

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

        private static ApplicationTheme ResolveSystemTheme()
        {
            var color = new UISettings().GetColorValue(UIColorType.Background);
            // Light backgrounds are high luminance
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
```

Fix the redundant `Apply()` branch when implementing: for non-System set Light/Dark directly; for System call `ResolveSystemTheme()` only once (remove the buggy double-assign shown above).

Correct `Apply()` body to use:

```csharp
app.RequestedTheme = Preference switch
{
    AppThemePreference.Light => ApplicationTheme.Light,
    AppThemePreference.Dark => ApplicationTheme.Dark,
    _ => ResolveSystemTheme()
};
```

- [ ] **Step 3: Hook App startup**

Modify `App.xaml.cs`:

```csharp
using Microsoft.UI.Xaml;
using ScoopX.Services;

namespace ScoopX
{
    public partial class App : Application
    {
        private Window? _window;

        public static Window? MainWindow { get; private set; }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            MainWindow = _window;
            ThemeService.Instance.Initialize(_window.DispatcherQueue);
            _window.Activate();
        }
    }
}
```

Note: `MainWindow` ctor runs before `Initialize`; first `Apply` after window creation still updates `Content.RequestedTheme`. If first paint flashes wrong theme, move `Initialize` to `App()` after `InitializeComponent()` for preference load + `Application.RequestedTheme` only, then re-`Apply` after `MainWindow` is set.

Preferred order:

```csharp
public App()
{
    InitializeComponent();
    // Preference only 鈥?Application.RequestedTheme before any window
    var pref = ThemeService.Instance; // ensure singleton
}

protected override void OnLaunched(...)
{
    // Set Application.RequestedTheme from saved preference BEFORE creating window
    ThemeService.Instance.ApplyApplicationThemeOnly();
    _window = new MainWindow();
    MainWindow = _window;
    ThemeService.Instance.Initialize(_window.DispatcherQueue);
    _window.Activate();
}
```

Implement `ApplyApplicationThemeOnly()` as the part of `Apply()` that only sets `Application.Current.RequestedTheme` (no window root). Call full `Apply()` from `Initialize`.

- [ ] **Step 4: Build**

Run: `dotnet build ScoopX.csproj -c Debug`
Expected: BUILD SUCCEEDED

- [ ] **Step 5: Manual check**

Launch app, confirm it still opens. (Theme visuals unchanged until Task 2.)

---

