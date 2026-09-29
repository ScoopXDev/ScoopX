# Final-review fix wave

## Status
**Complete** — Critical + Important items addressed; minor WebsitesPage/HomePage done.

## Build
```
dotnet build ScoopX.csproj -c Debug -p:Platform=x64
```
**Result:** Success — 0 warnings, 0 errors (~13s).

## Critical — ThemeService
- `Apply()` no longer sets `Application.RequestedTheme`; only updates `MainWindow` root `FrameworkElement.RequestedTheme`.
- System preference at runtime uses explicit `ElementTheme.Light` / `Dark` via `ResolveElementTheme()` (not `ElementTheme.Default`).
- `ApplyApplicationThemeOnly()` unchanged: `ReadPreference()` → `Application.RequestedTheme` (System → `ResolveSystemTheme()`), called before `new MainWindow()`.
- `Initialize` / `SetPreference` / `OnColorValuesChanged` behavior aligned with review (runtime root-only apply; OS listener on System).
- Removed useless `_ = ThemeService.Instance` from `App()` constructor.

## Important — EditSiteDialog nav
- XAML: `ScoopNavInactiveBrush`, accent fill brushes for hover/pressed; nav icons use theme resources.
- Code-behind: `GetThemeBrush`, `ScoopAccentBrush` / `AccentFillColorTertiaryBrush` for selection; `ThemeChanged` refreshes nav while dialog open.

## Minor
- `WebsitesPage.xaml`: table header columns use `ScoopTextSecondaryBrush`.
- `HomePage.xaml.cs`: chart grid/axis colors from `ScoopGaugeTrackBrush` / `ScoopTextTertiaryBrush`.

## Concerns
- Edit dialog selected-row background uses semi-transparent accent tertiary (not exact light `#EFF6FF`); acceptable in dark/light per theme dict.
- HomePage chart does not auto-redraw on theme change until next timer tick (no `ThemeChanged` hook).

## Files touched
`Services/ThemeService.cs`, `App.xaml.cs`, `Pages/EditSiteDialog.xaml`, `Pages/EditSiteDialog.xaml.cs`, `Pages/WebsitesPage.xaml`, `Pages/HomePage.xaml.cs`
