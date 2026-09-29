# Dark Theme Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add real Light/Dark ScoopX visuals plus Settings preference (System / Light / Dark) so dark Windows no longer breaks contrast on the light glass canvas.

**Architecture:** Extend `App.xaml` `ThemeDictionaries` with paired `Scoop*` brushes; a small `ThemeService` persists preference and sets `Application.RequestedTheme` (and the main content root when needed). Pages replace hardcoded `#` colors with `{ThemeResource Scoop*Brush}`. New `SettingsPage` owns the appearance control.

**Tech Stack:** WinUI 3, Windows App SDK, XAML `ThemeDictionaries`, `ApplicationData.LocalSettings`, `Windows.UI.ViewManagement.UISettings`

## Global Constraints

- Default preference: `System` (follow Windows)
- Options: `System` / `Light` / `Dark` only
- Keep existing light glass aesthetic; dark is a darker glass counterpart, not flat system gray only
- Do not change app icon assets in this plan
- Database / Store pages may stay empty; nav must remain readable in Dark
- No new test project required; verify manually (repo has no test projects)
- Do not `git commit` unless the user explicitly asks

## File Structure

| File | Responsibility |
|------|----------------|
| `Services/AppThemePreference.cs` | Enum `System`, `Light`, `Dark` |
| `Services/ThemeService.cs` | Load/save preference, apply theme, listen for system changes |
| `App.xaml` | Light/Dark `Scoop*` resources + styles using ThemeResource |
| `App.xaml.cs` | Call `ThemeService.Instance.Initialize()` before window activate |
| `MainWindow.xaml` / `.cs` | Theme-aware chrome, title, orbs, nav colors |
| `Pages/HomePage.xaml` | Surface/card/text ThemeResources |
| `Controls/StatusGauge.xaml` | Gauge text/track ThemeResources |
| `Pages/WebsitesPage.xaml` | Table links, row text ThemeResources |
| `Pages/AddSiteDialog.xaml` / `EditSiteDialog.xaml` | Dialog chrome ThemeResources |
| `Pages/SettingsPage.xaml` / `.cs` | Appearance ComboBox |
| `docs/superpowers/specs/2026-09-29-dark-theme-design.md` | Spec (already written) |

---

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
    // Preference only — Application.RequestedTheme before any window
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

### Task 2: Scoop ThemeDictionaries (Light + Dark)

**Files:**
- Modify: `App.xaml`

**Interfaces:**
- Consumes: existing Accent ThemeDictionaries
- Produces ThemeResource keys (both Light and Dark):  
  `ScoopAmbientGradient`, `ScoopGlassAcrylicBrush`, `ScoopGlassStrongAcrylicBrush`, `ScoopGlassBorderBrush`, `ScoopTextPrimaryBrush`, `ScoopTextSecondaryBrush`, `ScoopTextTertiaryBrush`, `ScoopTextBodyBrush` (`#374151` / `#D1D5DB`), `ScoopSurfaceFillBrush` (`#F7F7F7` / `#252528`), `ScoopCardFillBrush` (`#FFFFFF` / `#2A2A2E`), `ScoopNavInactiveBrush`, `SoftInputFillBrush`, `ScoopCaptionHoverBrush`, `ScoopOrbBlueBrush` / `ScoopOrbPinkBrush` / `ScoopOrbVioletBrush` (RadialGradientBrush for MainWindow orbs)

- [ ] **Step 1: Move glass + text brushes into ThemeDictionaries**

Inside existing `<ResourceDictionary x:Key="Light">` add (keep accent colors already there):

```xml
<LinearGradientBrush x:Key="ScoopAmbientGradient" StartPoint="0,0" EndPoint="1,1">
    <GradientStop Color="#EEF2FF" Offset="0" />
    <GradientStop Color="#F5F3FF" Offset="0.38" />
    <GradientStop Color="#FDF2F8" Offset="0.72" />
    <GradientStop Color="#EFF6FF" Offset="1" />
</LinearGradientBrush>
<AcrylicBrush x:Key="ScoopGlassAcrylicBrush" TintColor="#FFFFFF" TintOpacity="0.42" TintLuminosityOpacity="0.92" FallbackColor="#F4F6FB" />
<AcrylicBrush x:Key="ScoopGlassStrongAcrylicBrush" TintColor="#FFFFFF" TintOpacity="0.62" TintLuminosityOpacity="0.95" FallbackColor="#FAFBFE" />
<SolidColorBrush x:Key="ScoopGlassBorderBrush" Color="#B3FFFFFF" />
<SolidColorBrush x:Key="ScoopTextPrimaryBrush" Color="#111827" />
<SolidColorBrush x:Key="ScoopTextSecondaryBrush" Color="#6B7280" />
<SolidColorBrush x:Key="ScoopTextTertiaryBrush" Color="#9CA3AF" />
<SolidColorBrush x:Key="ScoopTextBodyBrush" Color="#374151" />
<SolidColorBrush x:Key="ScoopSurfaceFillBrush" Color="#F7F7F7" />
<SolidColorBrush x:Key="ScoopCardFillBrush" Color="#FFFFFF" />
<SolidColorBrush x:Key="ScoopNavInactiveBrush" Color="#4B5563" />
<SolidColorBrush x:Key="SoftInputFillBrush" Color="#F2F3F5" />
<SolidColorBrush x:Key="ScoopCaptionHoverBrush" Color="#14000000" />
<SolidColorBrush x:Key="ScoopCaptionPressedBrush" Color="#26000000" />
```

Inside `<ResourceDictionary x:Key="Dark">` add paired dark values:

```xml
<LinearGradientBrush x:Key="ScoopAmbientGradient" StartPoint="0,0" EndPoint="1,1">
    <GradientStop Color="#0B1220" Offset="0" />
    <GradientStop Color="#15101F" Offset="0.38" />
    <GradientStop Color="#1A1218" Offset="0.72" />
    <GradientStop Color="#0F172A" Offset="1" />
</LinearGradientBrush>
<AcrylicBrush x:Key="ScoopGlassAcrylicBrush" TintColor="#1C1C1E" TintOpacity="0.72" TintLuminosityOpacity="0.85" FallbackColor="#1C1C1E" />
<AcrylicBrush x:Key="ScoopGlassStrongAcrylicBrush" TintColor="#2A2A2E" TintOpacity="0.82" TintLuminosityOpacity="0.9" FallbackColor="#2A2A2E" />
<SolidColorBrush x:Key="ScoopGlassBorderBrush" Color="#33FFFFFF" />
<SolidColorBrush x:Key="ScoopTextPrimaryBrush" Color="#F3F4F6" />
<SolidColorBrush x:Key="ScoopTextSecondaryBrush" Color="#9CA3AF" />
<SolidColorBrush x:Key="ScoopTextTertiaryBrush" Color="#6B7280" />
<SolidColorBrush x:Key="ScoopTextBodyBrush" Color="#D1D5DB" />
<SolidColorBrush x:Key="ScoopSurfaceFillBrush" Color="#252528" />
<SolidColorBrush x:Key="ScoopCardFillBrush" Color="#2A2A2E" />
<SolidColorBrush x:Key="ScoopNavInactiveBrush" Color="#9CA3AF" />
<SolidColorBrush x:Key="SoftInputFillBrush" Color="#3A3A3E" />
<SolidColorBrush x:Key="ScoopCaptionHoverBrush" Color="#14FFFFFF" />
<SolidColorBrush x:Key="ScoopCaptionPressedBrush" Color="#26FFFFFF" />
```

- [ ] **Step 2: Remove duplicate non-theme copies**

Delete the old top-level `ScoopAmbientGradient`, `ScoopGlass*`, `ScoopGlassBorderBrush`, `SoftInputFillBrush` from outside ThemeDictionaries (keys must exist only in Light/Dark).

Update styles to ThemeResource:

```xml
<Style x:Key="ScoopGlassCardStyle" TargetType="Border">
    <Setter Property="Background" Value="{ThemeResource ScoopGlassAcrylicBrush}" />
    <Setter Property="BorderBrush" Value="{ThemeResource ScoopGlassBorderBrush}" />
    ...
</Style>

<Style x:Key="DialogFieldLabelStyle" TargetType="TextBlock">
    <Setter Property="Foreground" Value="{ThemeResource ScoopTextPrimaryBrush}" />
    ...
</Style>

<Style x:Key="DialogFieldHintStyle" TargetType="TextBlock">
    <Setter Property="Foreground" Value="{ThemeResource ScoopTextTertiaryBrush}" />
    ...
</Style>

<Style x:Key="SoftCancelButtonStyle" ...>
    <Setter Property="Background" Value="{ThemeResource SoftInputFillBrush}" />
    <Setter Property="Foreground" Value="{ThemeResource ScoopTextBodyBrush}" />
    ...
</Style>
```

Update `TableLinkButtonStyle` ContentPresenter:

```xml
<ContentPresenter
    ...
    Foreground="{TemplateBinding Foreground}"
    Background="Transparent"
    ... />
```

And add setter:

```xml
<Setter Property="Foreground" Value="{ThemeResource AccentTextFillColorPrimaryBrush}" />
```

Caption hover states: use `{ThemeResource ScoopCaptionHoverBrush}` / `ScoopCaptionPressedBrush` instead of `#14000000`.

Keep `ScoopAccentColor` / `ScoopAccentBrush` as shared (non-theme) resources.

- [ ] **Step 3: Build**

Run: `dotnet build ScoopX.csproj -c Debug`  
Expected: SUCCEEDED. If XAML complains missing StaticResource for glass, ensure MainWindow uses `ThemeResource` (Task 3).

---

### Task 3: MainWindow theme wiring

**Files:**
- Modify: `MainWindow.xaml`
- Modify: `MainWindow.xaml.cs`

**Interfaces:**
- Consumes: `ScoopAmbientGradient`, text/nav brushes, `ThemeService.ThemeChanged`
- Produces: nav colors refresh on theme change

- [ ] **Step 1: XAML brushes**

Replace:

```xml
<StaticResource ResourceKey="ScoopAmbientGradient" />
```

with:

```xml
<ThemeResource ResourceKey="ScoopAmbientGradient" />
```

Title / subtitle:

```xml
<TextBlock Text="ScoopX Workspace" ... Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
<TextBlock Text="为每一位..." ... Foreground="{ThemeResource ScoopTextSecondaryBrush}" />
```

Content glass borders that use `StaticResource ScoopGlassAcrylicBrush` → `ThemeResource`.

Nav inactive XAML defaults `#4B5563` → `{ThemeResource ScoopNavInactiveBrush}` for all inactive FontIcon/TextBlock initial Foreground.

Orbs: reduce opacity in dark by binding Fill to theme-specific brushes if added in Task 2; otherwise leave orbs but lower Opacity is optional. Prefer adding three `RadialGradientBrush` keys per theme (`ScoopOrbBlue`, `ScoopOrbPink`, `ScoopOrbViolet`) with darker alpha stops in Dark dictionary, then reference via ThemeResource.

- [ ] **Step 2: Code-behind nav brushes**

Replace static brushes with helpers that read current theme resources from `App.Current.Resources` / themed dictionary, or subscribe to `ThemeService.ThemeChanged` and refresh:

```csharp
private SolidColorBrush ActiveBrush =>
    (SolidColorBrush)Application.Current.Resources["ScoopAccentBrush"];

private SolidColorBrush InactiveBrush =>
    (SolidColorBrush)((FrameworkElement)Content).Resources["ScoopNavInactiveBrush"]
    // Prefer: lookup from Content root ActualTheme
```

Practical approach: after `InitializeComponent`, subscribe:

```csharp
ThemeService.Instance.ThemeChanged += (_, _) => RefreshNavColors();
```

And implement `RefreshNavColors()` to re-apply `ApplyNavSelection(_selectedTag, animatePill: false)` using brushes resolved from:

```csharp
private Brush ResolveBrush(string key)
{
    var root = (FrameworkElement)Content;
    if (root.Resources.TryGetValue(key, out var local) && local is Brush b1)
        return b1;
    return (Brush)Application.Current.Resources[key];
}
```

Theme dictionaries resolve via `ThemeResource` on elements; for code-behind use:

```csharp
private Brush GetThemeBrush(string key)
{
    return (Brush)((FrameworkElement)Content).FindResource? // not available
}
```

WinUI pattern: put `ScoopNavInactiveBrush` also as a keyed resource accessible via `(SolidColorBrush)App.Current.Resources[key]` only works for non-theme keys. For theme keys from code:

```csharp
var theme = ((FrameworkElement)Content).ActualTheme;
var dicts = Application.Current.Resources.ThemeDictionaries;
var rd = (ResourceDictionary)dicts[theme == ElementTheme.Dark ? "Dark" : "Light"];
return (Brush)rd[key];
```

Handle `ElementTheme.Default` by mapping with `Application.Current.RequestedTheme`.

- [ ] **Step 3: Build + manual**

Run: `dotnet build`  
Manual: Windows Dark + default System → title readable on dark gradient; nav inactive not washed out.

---

### Task 4: HomePage + StatusGauge

**Files:**
- Modify: `Pages/HomePage.xaml`
- Modify: `Controls/StatusGauge.xaml`

- [ ] **Step 1: HomePage**

Replace:

| Old | New |
|-----|-----|
| `Background="#F7F7F7"` | `{ThemeResource ScoopSurfaceFillBrush}` |
| `Foreground="#111827"` | `{ThemeResource ScoopTextPrimaryBrush}` |
| `Foreground="#6B7280"` | `{ThemeResource ScoopTextSecondaryBrush}` |
| `Background="#FFFFFF"` (metric cards + chart) | `{ThemeResource ScoopCardFillBrush}` |
| `Foreground="#374151"` | `{ThemeResource ScoopTextBodyBrush}` |
| Section title `TextFillColorPrimaryBrush` | `{ThemeResource ScoopTextPrimaryBrush}` (optional consistency) |

Leave pink/blue series dots (`#F472B6`, `#60A5FA`) as fixed brand series colors.

- [ ] **Step 2: StatusGauge**

Replace `#E5E7EB` track with `{ThemeResource ScoopTextTertiaryBrush}` at ~0.35 opacity OR add `ScoopGaugeTrackBrush` Light `#E5E7EB` / Dark `#3F3F46`. Prefer dedicated key in ThemeDictionaries:

Light: `#E5E7EB`  
Dark: `#3F3F46`

Labels: primary/secondary/tertiary Scoop brushes instead of `#374151` / `#6B7280` / `#9CA3AF`.

- [ ] **Step 3: Build + manual on Home**

Expected: Dark mode home cards and gauges readable.

---

### Task 5: WebsitesPage links + table text

**Files:**
- Modify: `Pages/WebsitesPage.xaml`

- [ ] **Step 1: Link + row foregrounds**

On site name / path HyperlinkButtons (already `TableLinkButtonStyle`), set:

```xml
Foreground="{ThemeResource AccentTextFillColorPrimaryBrush}"
```

Inner TextBlocks:

```xml
Foreground="{ThemeResource AccentTextFillColorPrimaryBrush}"
```

For path, secondary look is OK:

```xml
Foreground="{ThemeResource ScoopTextSecondaryBrush}"
```

on the path TextBlock (and matching HyperlinkButton Foreground).

Remark / PhpVersion TextBlocks:

```xml
Foreground="{ThemeResource ScoopTextPrimaryBrush}"
```

设置 / 删除 HyperlinkButtons:

```xml
Style="{StaticResource TableLinkButtonStyle}"
Foreground="{ThemeResource AccentTextFillColorPrimaryBrush}"
```

Any glass card Background using StaticResource → ThemeResource.

- [ ] **Step 2: Build + manual**

Windows Dark: website list paths and 设置/删除 clearly visible; ToggleSwitch thumb/track match dark glass (no black-on-white mismatch).

---

### Task 6: Dialogs theme

**Files:**
- Modify: `Pages/AddSiteDialog.xaml`
- Modify: `Pages/EditSiteDialog.xaml`

- [ ] **Step 1: Theme dialog chrome**

Replace local overrides:

```xml
<SolidColorBrush x:Key="ContentDialogBackground" Color="#FFFFFF" />
```

with ThemeDictionaries entries (add keys to App.xaml Light/Dark):

Light: `#FFFFFF`  
Dark: `#2A2A2E`

And in dialogs use:

```xml
<!-- Remove hard-coded white brushes from dialog Resources if they force Light;
     either delete overrides to inherit Fluent, or set:
-->
<StaticResource x:Key="ContentDialogBackground" ResourceKey="ScoopCardFillBrush" />
```

WinUI may not allow StaticResource alias easily for this; simplest: delete the three hard-coded ContentDialog* brushes from dialog Resources so system theme dialog chrome applies, and change title `Foreground="#111827"` / `#1F2937` to `{ThemeResource ScoopTextPrimaryBrush}`.

Keep terminal/black SSH panels in EditSiteDialog as intentional dark panels (`#000000`) — do not theme those to light.

- [ ] **Step 2: Build + open Add Site dialog in Dark**

Expected: dialog background dark, labels readable.

---

### Task 7: SettingsPage appearance control

**Files:**
- Create: `Pages/SettingsPage.xaml`
- Create: `Pages/SettingsPage.xaml.cs`
- Modify: `MainWindow.xaml.cs` (`NavigateTo`)

**Interfaces:**
- Consumes: `ThemeService.Instance.Preference`, `SetPreference`
- Produces: Settings navigation target

- [ ] **Step 1: SettingsPage XAML**

```xml
<?xml version="1.0" encoding="utf-8"?>
<Page
    x:Class="ScoopX.Pages.SettingsPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    Background="Transparent">

    <ScrollViewer Padding="20,16,24,24">
        <StackPanel Spacing="16" MaxWidth="560" HorizontalAlignment="Left">
            <TextBlock
                Text="设置"
                FontSize="18"
                FontWeight="SemiBold"
                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />

            <Border
                Background="{ThemeResource ScoopGlassStrongAcrylicBrush}"
                BorderBrush="{ThemeResource ScoopGlassBorderBrush}"
                BorderThickness="1"
                CornerRadius="16"
                Padding="20,16">
                <Grid ColumnSpacing="16">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="Auto" />
                    </Grid.ColumnDefinitions>
                    <StackPanel Spacing="4" VerticalAlignment="Center">
                        <TextBlock Text="外观" FontSize="14" FontWeight="SemiBold"
                                   Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                        <TextBlock Text="选择浅色、深色，或跟随 Windows 系统设置"
                                   FontSize="12"
                                   Foreground="{ThemeResource ScoopTextSecondaryBrush}"
                                   TextWrapping="Wrap" />
                    </StackPanel>
                    <ComboBox
                        x:Name="ThemeComboBox"
                        Grid.Column="1"
                        MinWidth="140"
                        VerticalAlignment="Center"
                        SelectionChanged="ThemeComboBox_SelectionChanged">
                        <ComboBoxItem Content="跟随系统" Tag="System" />
                        <ComboBoxItem Content="浅色" Tag="Light" />
                        <ComboBoxItem Content="深色" Tag="Dark" />
                    </ComboBox>
                </Grid>
            </Border>
        </StackPanel>
    </ScrollViewer>
</Page>
```

- [ ] **Step 2: Code-behind**

```csharp
using Microsoft.UI.Xaml.Controls;
using ScoopX.Services;

namespace ScoopX.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private bool _suppressThemeEvent;

        public SettingsPage()
        {
            InitializeComponent();
            _suppressThemeEvent = true;
            ThemeComboBox.SelectedIndex = ThemeService.Instance.Preference switch
            {
                AppThemePreference.Light => 1,
                AppThemePreference.Dark => 2,
                _ => 0
            };
            _suppressThemeEvent = false;
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressThemeEvent || ThemeComboBox.SelectedItem is not ComboBoxItem item)
            {
                return;
            }

            var tag = item.Tag as string ?? "System";
            var pref = tag switch
            {
                "Light" => AppThemePreference.Light,
                "Dark" => AppThemePreference.Dark,
                _ => AppThemePreference.System
            };
            ThemeService.Instance.SetPreference(pref);
        }
    }
}
```

- [ ] **Step 3: Navigate**

In `MainWindow.xaml.cs` `NavigateTo`:

```csharp
case "Settings":
    ContentFrame.Navigate(typeof(SettingsPage));
    break;
```

- [ ] **Step 4: Full manual acceptance**

1. Windows Light + 跟随系统 → looks like today’s light UI  
2. Windows Dark + 跟随系统 → dark glass, readable title/paths/toggles  
3. Force 浅色 while Windows Dark → app stays light  
4. Force 深色 while Windows Light → app stays dark  
5. Restart → last preference restored  
6. Settings page opens from sidebar  

---

## Spec coverage check

| Spec item | Task |
|-----------|------|
| Scoop* ThemeDictionaries | Task 2 |
| ThemeService + LocalSettings | Task 1 |
| Default System | Task 1 |
| Settings 跟随/浅/深 | Task 7 |
| MainWindow / Home / Gauge / Websites / Dialogs | Tasks 3–6 |
| No icon changes | (out of scope) |
| Nav readable in Dark | Tasks 2–3 |

## Placeholder / consistency self-review

- `Apply()` double-assign bug called out; implementer must use the corrected switch.
- `ApplyApplicationThemeOnly` named in Task 1 preferred startup path — implement as a public method on `ThemeService`.
- Brush key names consistent: `ScoopTextBodyBrush`, `ScoopSurfaceFillBrush`, `ScoopCardFillBrush`, `ScoopNavInactiveBrush`.
