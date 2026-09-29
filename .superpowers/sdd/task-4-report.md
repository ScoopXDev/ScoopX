# Task 4 Report: HomePage + StatusGauge theme brushes

## Status
**Complete**

## Commits
None (per instructions).

## Changes

### App.xaml
- Added `ScoopGaugeTrackBrush`: Light `#E5E7EB`, Dark `#3F3F46` in both ThemeDictionaries

### Pages/HomePage.xaml
- Disk IO panel: `ScoopSurfaceFillBrush`, primary/secondary/body text brushes, card backgrounds via `ScoopCardFillBrush`
- Section title「状态」: `ScoopTextPrimaryBrush` (was `TextFillColorPrimaryBrush`)
- Left `#F472B6` / `#60A5FA` series dots unchanged

### Controls/StatusGauge.xaml
- Track ring: `ScoopGaugeTrackBrush`
- Detail / usage / label: `ScoopTextBodyBrush`, `ScoopTextSecondaryBrush`, `ScoopTextTertiaryBrush`

### Controls/StatusGauge.xaml.cs
- Removed `DetailDark` override in `SetSimple` so XAML theme brushes apply

## Build
```
dotnet build ScoopX.csproj -c Debug -p:Platform=x64
```
**Result:** Success — 0 warnings, 0 errors (~13s).

## Manual verification (not run in agent)
- Open Home in light/dark: metric cards, disk IO panel, gauge labels and track readable
- Gauges at caution/critical still use fixed accent/caution/critical colors in code-behind

## Concerns
- `HomePage.xaml.cs` chart grid/axis labels still use hard-coded `#E5E7EB` / `#9CA3AF`; dark mode chart area may need a follow-up task
- Gauge progress/percent colors remain static `#2563EB` / orange / red (by design for status semantics)
- `PercentText` / progress still bind `ScoopAccentBrush` (StaticResource) at rest; animated states override in code
