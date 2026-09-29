# Task 1 Report: AppSettings service

**Status:** DONE  
**Branch:** `feature/dark-theme`  
**Date:** 2026-09-29

## Summary

Created singleton `AppSettings` in `Services/AppSettings.cs` per task brief. Persists five keys via `ApplicationData.Current.LocalSettings`, exposes `Changed`, and implements `SetStartWithWindowsAsync` with `StartupTask` sync for `StartWithWindows`.

## Files changed

| Action | Path |
|--------|------|
| Created | `Services/AppSettings.cs` |

## Implementation notes

### Keys and defaults

| Property | LocalSettings key | Default |
|----------|-------------------|---------|
| `CloseToTray` | `CloseToTray` | `true` |
| `StartWithWindows` | `StartWithWindows` | `false` |
| `StartHiddenToTray` | `StartHiddenToTray` | `false` |
| `ScoopRootPath` | `ScoopRootPath` | `""` |
| `WebsitesRootPath` | `WebsitesRootPath` | `""` |

### API

- **Singleton:** `AppSettings.Instance` via `Lazy<AppSettings>`.
- **`StartupTaskId`:** public const `"ScoopXStartup"` for manifest wiring in later tasks.
- **Setters:** bool/string properties write LocalSettings and call `RaiseChanged()` except `StartWithWindows`, whose setter fire-and-forgets `SetStartWithWindowsAsync`.
- **`SetStartWithWindowsAsync`:** returns `(bool ok, string? error)`; enables via `RequestEnableAsync`, disables via `Disable()`; on failure or exception persists `false` and returns user-facing Chinese error strings.

### Out of scope (unchanged)

- `SettingsPage`, `MainWindow`, `Package.appxmanifest` — not modified.

## Build verification

| Command | Result |
|---------|--------|
| `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo` | **Succeeded** — 0 warnings, 0 errors |

## Self-review checklist

- [x] Matches brief code structure and key names
- [x] Defaults match spec (`CloseToTray` true, others false/empty)
- [x] `Changed` raised on all successful persistence paths including startup-task failures
- [x] Pattern aligned with existing `ThemeService` (LocalSettings singleton)
- [x] No git commit
- [x] No UI or manifest changes

## Concerns

1. **Interface vs implementation:** Brief “Interfaces” lists `Task SetStartWithWindowsAsync(bool)`; implementation returns `Task<(bool ok, string? error)>` as in the brief code block — UI tasks should use the tuple for error dialogs.
2. **Runtime:** `StartupTask` APIs throw or fail when unpackaged; catch path documents need for packaged build — expected until Task 2 manifest + install.

## Next tasks (not done here)

- Manifest `StartupTask` extension declaration.
- Wire `MainWindow` / `App` for close-to-tray and start hidden.
- Settings page toggles and path fields bound to `AppSettings.Instance`.
