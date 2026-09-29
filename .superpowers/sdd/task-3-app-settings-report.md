# Task 3: MainWindow / App CloseToTray & StartHiddenToTray — Report

## Status

**Complete.** `MainWindow` reads `AppSettings.Instance.CloseToTray` / `StartHiddenToTray` for close and startup hide behavior. Tray Exit still uses `ExitApplication()` unchanged.

## Changes

**File:** `MainWindow.xaml.cs` only (`App.xaml.cs` untouched)

1. **`AppWindow_Closing`**
   - If `_isExitRequested`: return (allow close).
   - If `CloseToTray`: `args.Cancel = true` + `HideToTray()`.
   - Else: `TrayIcon.Dispose()` and allow window close.

2. **`CloseButton_Click`**
   - If `CloseToTray`: `HideToTray()`.
   - Else: `ExitApplication()`.

3. **Constructor (after `NavigateTo("Home")`)**
   - If `StartHiddenToTray`: one-shot `Activated` handler that unsubscribes then `HideToTray()` (avoids flash/race vs immediate Hide).

## Build

```
dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo
```

- **Result:** Success
- **Warnings:** 0
- **Errors:** 0
- **Duration:** ~13s

## Git

No commits (per task constraints).

## Notes / Concerns

- Defaults unchanged via AppSettings: `CloseToTray=true`, `StartHiddenToTray=false` → current UX preserved until Settings UI (Task 4) toggles them.
- `Activated` one-shot not runtime-verified for “already activated” edge case; brief allows `DispatcherQueue.TryEnqueue` fallback if needed.
- When `CloseToTray=false`, system close path disposes tray then lets WinUI close; custom X button uses `ExitApplication()` (sets `_isExitRequested`, dispose, `Close()`).

## Verification checklist

- [x] CloseToTray branching in `AppWindow_Closing` and `CloseButton_Click`
- [x] StartHiddenToTray one-shot Activated after NavigateTo Home
- [x] Tray Exit → `ExitApplication` unchanged
- [x] No SettingsPage UI changes
- [x] x64 Debug build passes
