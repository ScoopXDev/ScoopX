# Task 2 Report: Scoop ThemeDictionaries (Light + Dark)

**Status:** DONE  
**Branch:** `feature/dark-theme`  
**Date:** 2026-09-29

## Summary

Moved Scoop glass, text, surface, caption, and ambient resources into `ResourceDictionary.ThemeDictionaries` Light/Dark pairs. Updated shared styles in `App.xaml` to `{ThemeResource ...}`. Removed duplicate top-level brush keys. Kept `ScoopAccentColor` / `ScoopAccentBrush` as global non-theme resources.

## Files changed

| Action | Path |
|--------|------|
| Modified | `App.xaml` |

## Theme keys added (Light + Dark)

Per task brief Step 1: `ScoopAmbientGradient`, `ScoopGlassAcrylicBrush`, `ScoopGlassStrongAcrylicBrush`, `ScoopGlassBorderBrush`, `ScoopTextPrimaryBrush`, `ScoopTextSecondaryBrush`, `ScoopTextTertiaryBrush`, `ScoopTextBodyBrush`, `ScoopSurfaceFillBrush`, `ScoopCardFillBrush`, `ScoopNavInactiveBrush`, `SoftInputFillBrush`, `ScoopCaptionHoverBrush`, `ScoopCaptionPressedBrush`.

Also added (interfaces list; not in Step 1 XML snippet): `ScoopOrbBlueBrush`, `ScoopOrbPinkBrush`, `ScoopOrbVioletBrush`.

### Orb brushes (not in brief color table)

- **Light:** Same center/edge stops as inline `MainWindow.xaml` ellipses (`#B3BFDBFE` / `#A6FBCFE8` / `#99DDD6FE` → transparent edges).
- **Dark:** Same RGB hues with ~half alpha on center stops (`#59BFDBFE`, `#53FBCFE8`, `#4DDDD6FE`; edges unchanged). Brief/plan only say “lower alpha in Dark”; no exact hex — chosen to match plan guidance.

## Style updates (Step 2)

- `ScoopGlassCardStyle` → `ThemeResource` glass/border
- `DialogFieldLabelStyle` → `ScoopTextPrimaryBrush`
- `DialogFieldHintStyle` → `ScoopTextTertiaryBrush`
- `SoftCancelButtonStyle` → `SoftInputFillBrush`, `ScoopTextBodyBrush`
- `TableLinkButtonStyle` → `Foreground` accent + `ContentPresenter` `Foreground="{TemplateBinding Foreground}"`
- `CaptionButtonStyle` PointerOver/Pressed → `ScoopCaptionHoverBrush` / `ScoopCaptionPressedBrush`

## Build / StaticResource note

`MainWindow.xaml` and `Pages/WebsitesPage.xaml` still reference `StaticResource` for `ScoopAmbientGradient`, `ScoopGlassAcrylicBrush`, `ScoopGlassStrongAcrylicBrush`, `ScoopGlassBorderBrush`. **No MainWindow/page edits in this task.** XAML compile succeeded; runtime resolution of theme-only keys via `StaticResource` may be wrong until Task 3 switches those references to `ThemeResource`. No temporary top-level aliases were added.

## Build verification

| Command | Result |
|---------|--------|
| `dotnet build ScoopX.csproj -c Debug -p:Platform=x64` | **Succeeded** — 0 warnings, 0 errors |

## Self-review checklist

- [x] Exact brief hex values for Step 1 brushes (Light/Dark)
- [x] Duplicate top-level glass/gradient/SoftInput removed
- [x] Accent ThemeDictionaries unchanged except additions
- [x] Shared Scoop accent color/brush kept at root
- [x] No SettingsPage / page color migrations
- [x] No git commit
- [x] ThemeService untouched

## Concerns

1. **StaticResource in MainWindow/WebsitesPage** — Task 3 should migrate to `ThemeResource` and wire orb fills to `ScoopOrb*Brush`.
2. **Dark orb stop colors** — Not specified in brief; document if design wants different values.
3. **VisualState `ThemeResource` in CaptionButtonStyle** — Build passed; verify hover/pressed in Dark at runtime in Task 3+ manual QA.

## Next tasks (not done here)

- Task 3: MainWindow theme wiring + nav refresh
- Later: HomePage, dialogs, SettingsPage, hardcoded `#` cleanup
