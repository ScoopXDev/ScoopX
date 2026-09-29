# Task 3 Report: MainWindow theme wiring

## Status
**Complete**

## Commits
None (per instructions).

## Changes

### MainWindow.xaml
- `ScoopAmbientGradient`: `StaticResource` → `ThemeResource`
- Title/subtitle: `ScoopTextPrimaryBrush` / `ScoopTextSecondaryBrush`
- Content glass border: `ThemeResource` for acrylic + border brushes
- Orbs: `ScoopOrbBlueBrush`, `ScoopOrbPinkBrush`, `ScoopOrbVioletBrush` via `ThemeResource`
- Inactive nav icons/labels: `ScoopNavInactiveBrush` (replacing `#4B5563`)

### MainWindow.xaml.cs
- Removed hard-coded `ActiveBrush` / `InactiveBrush` static fields
- `ActiveBrush` from `Application.Current.Resources["ScoopAccentBrush"]`
- `InactiveBrush` via `GetThemeBrush("ScoopNavInactiveBrush")` (theme dictionary lookup with `ElementTheme.Default` → `RequestedTheme`)
- Subscribed to `ThemeService.Instance.ThemeChanged` → `RefreshNavColors()` → `ApplyNavSelection(_selectedTag, animatePill: false)`
- `SetNavItemState` changed from static to instance method

## Build
```
dotnet build ScoopX.csproj -c Debug -p:Platform=x64
```
**Result:** Success — 0 warnings, 0 errors (~15s).

## Manual verification (not run in agent)
- Windows Dark + System default: title readable on dark gradient; nav inactive not washed out
- Toggle theme in settings (when available) and confirm nav colors refresh without restart

## Concerns
- Orbs still use fixed `Opacity` (0.85/0.8/0.7); dark theme already uses lower-alpha gradient stops — may still feel bright; optional follow-up to theme-specific opacity
- `ScoopAccentBrush` remains app-global (not per-theme); dark mode accent differs in theme dict SystemAccent colors but nav active color stays `#2563EB`
- Hover state uses same brushes as selection; theme change during hover is edge case only
