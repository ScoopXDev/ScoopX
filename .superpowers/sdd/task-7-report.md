# Task 7 Report: SettingsPage appearance control

## Status
**Complete**

## Commits
None (per instructions).

## Changes

### Pages/SettingsPage.xaml (new)
- Settings page with title「设置」and「外观」glass card
- `ThemeComboBox`: 跟随系统 / 浅色 / 深色 with `Tag` System/Light/Dark
- Uses `ScoopTextPrimaryBrush`, `ScoopGlassStrongAcrylicBrush`, etc.

### Pages/SettingsPage.xaml.cs (new)
- Initializes combo from `ThemeService.Instance.Preference` with `_suppressThemeEvent`
- `SelectionChanged` → `ThemeService.Instance.SetPreference`

### MainWindow.xaml.cs
- `NavigateTo`: `case "Settings"` → `ContentFrame.Navigate(typeof(SettingsPage))`

## Build
```
dotnet build ScoopX.csproj -c Debug -p:Platform=x64
```
**Result:** Success — 0 warnings, 0 errors (~13s).

## Manual verification (not run in agent)
1. Windows Light + 跟随系统 → light UI
2. Windows Dark + 跟随系统 → dark glass, readable text
3. Force 浅色 while Windows Dark → app stays light
4. Force 深色 while Windows Light → app stays dark
5. Restart → last preference restored
6. Settings opens from sidebar nav

## Concerns
- Combo selection not refreshed if preference changes elsewhere (only Settings writes today)
- Database/Store nav still clear frame (unchanged)

## Final-review fix wave

**Status:** Complete  
**Build:** `dotnet build ScoopX.csproj -c Debug -p:Platform=x64` — 0 warnings, 0 errors (~13s).

### Critical — ThemeService
- Runtime `Apply()` sets only main window root `RequestedTheme`; `Application.RequestedTheme` set only in `ApplyApplicationThemeOnly()` before window creation.
- `ResolveElementTheme()` maps System to explicit Light/Dark from OS (fixes post-window `NotSupportedException` and OS theme follow).
- Removed `_ = ThemeService.Instance` from `App()`.

### Important — EditSiteDialog
- Nav styles/icons: `ScoopNavInactiveBrush`, `AccentFillColorTertiaryBrush` / `Secondary` for states; code-behind uses theme brushes + `ThemeChanged` refresh.

### Minor
- WebsitesPage table headers → `ScoopTextSecondaryBrush`.
- HomePage IO chart grid/labels → `ScoopGaugeTrackBrush` / `ScoopTextTertiaryBrush` (refresh on timer, not immediate on theme change).

### Concerns
- Selected nav fill is theme accent tertiary, not legacy `#EFF6FF`.
- See also `.superpowers/sdd/final-fix-report.md`.
