# Task 1 Report: ThemeService + preference storage

**Status:** DONE  
**Branch:** `feature/dark-theme`  
**Date:** 2026-09-29

## Summary

Implemented theme preference enum, singleton `ThemeService` with local settings persistence, system theme change listening, and app startup hooks. Corrected the plan’s redundant `Apply()` `RequestedTheme` assignment; added `ApplyApplicationThemeOnly()` for pre-window startup.

## Files changed

| Action | Path |
|--------|------|
| Created | `Services/AppThemePreference.cs` |
| Created | `Services/ThemeService.cs` |
| Modified | `App.xaml.cs` |

## Implementation notes

### `AppThemePreference`

Enum values: `System`, `Light`, `Dark` (default when unset: `System`).

### `ThemeService`

- **Singleton:** `ThemeService.Instance` via `Lazy<ThemeService>`.
- **Storage:** `ApplicationData.Current.LocalSettings`, key `AppTheme` (`SettingsKey`), string enum name.
- **`Apply()`:** Sets `Application.Current.RequestedTheme` with a single switch (Light / Dark / `ResolveSystemTheme()` for System). Updates `App.MainWindow.Content` root `FrameworkElement.RequestedTheme` when a window exists.
- **`ApplyApplicationThemeOnly()`:** Same application-level theme logic using `ReadPreference()` so saved preference applies before `Initialize()` sets `Preference`.
- **`Initialize(DispatcherQueue)`:** Loads preference, full `Apply()`, registers `UISettings.ColorValuesChanged` when preference is System.
- **`SetPreference`:** Persists, applies, adjusts listener, raises `ThemeChanged`.
- **System theme:** Background color luminance heuristic per plan.

### `App.xaml.cs`

1. Constructor: touch `ThemeService.Instance` after `InitializeComponent()`.
2. `OnLaunched`: `ApplyApplicationThemeOnly()` → create `MainWindow` → assign `MainWindow` → `Initialize(DispatcherQueue)` → `Activate()`.

### Deviations / refinements

- Extracted `SetApplicationRequestedTheme(AppThemePreference)` to share logic between `Apply()` and `ApplyApplicationThemeOnly()` without duplicating the fixed switch.
- Brief interface line lists `void Initialize()`; plan Step 2/3 use `Initialize(DispatcherQueue)` — implemented with dispatcher parameter as specified in steps.

## Build verification

| Command | Result |
|---------|--------|
| `dotnet build ScoopX.csproj -c Debug` | **Failed** — MSIX packaging error: AnyCPU / missing `RuntimeIdentifier` (project-level; DLL compile step succeeded before packaging target). |
| `dotnet build ScoopX.csproj -c Debug -p:Platform=x64` | **Succeeded** — 0 warnings, 0 errors |

Task Step 4 expects plain Debug build to succeed; on this machine/SDK, specify `-p:Platform=x64` (or another configured platform) for a full successful build.

## Manual check

Not run in this session (no interactive app launch). Visual theme resources unchanged until Task 2; startup path should behave as before with preference loaded silently.

## Self-review checklist

- [x] Enum and namespace match plan
- [x] Settings key and default `System`
- [x] Buggy double-assign in sample `Apply()` removed
- [x] `ApplyApplicationThemeOnly()` before window creation
- [x] Full `Apply()` from `Initialize`
- [x] System listener attach/detach on preference
- [x] `ThemeChanged` on user set and system color change (UI thread)
- [x] No git commit (per constraints)
- [x] No ThemeDictionaries / Settings UI (Task 1 scope only)

## Concerns

1. **Build command:** Default Debug build without `Platform` may fail MSIX step; use `x64` (or project’s usual platform) for CI/local verification until project defaults are adjusted (out of Task 1 scope).
2. **`Initialize()` signature** in brief “Interfaces” section vs implementation with `DispatcherQueue` — follow steps 2–3 for later tasks.

## Next tasks (not done here)

- Task 2+: theme dictionaries and settings UI consuming `ThemeService.Instance.SetPreference`.
