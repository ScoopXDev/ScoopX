# Task 1 Report: LanguageService

**Status:** DONE  
**Date:** 2026-09-29

## Summary

Created singleton `LanguageService` in `Services/LanguageService.cs` per task brief. Reads `AppLanguage` from `LocalSettings`, defaults to `zh-CN`, normalizes `en-US` / `en` to English and everything else to Chinese, and sets `ApplicationLanguages.PrimaryLanguageOverride` at startup via `ApplyBeforeUi()` before `InitializeComponent()`.

## Files changed

| Action | Path |
|--------|------|
| Created | `Services/LanguageService.cs` |
| Modified | `App.xaml.cs` |

## Implementation notes

### Constants and storage

| Symbol | Value |
|--------|-------|
| `SettingsKey` | `AppLanguage` |
| `ZhCn` | `zh-CN` |
| `EnUs` | `en-US` |

### API

- **Singleton:** `LanguageService.Instance` via `Lazy<LanguageService>`.
- **`ApplyBeforeUi()`:** `ReadTag()` → `CurrentTag` → `PrimaryLanguageOverride`; must run after theme init and before `InitializeComponent()`.
- **`SetLanguage(string tag)`:** `Normalize`, persist to LocalSettings, update override; does not restart the app (callers handle restart in later tasks).
- **`Normalize`:** case-insensitive `en-US` or `en` → `en-US`; otherwise `zh-CN`.

### App wiring

```csharp
ThemeService.Instance.ApplyApplicationThemeBeforeInit(this);
LanguageService.Instance.ApplyBeforeUi();
InitializeComponent();
```

### Out of scope (unchanged)

- `SettingsPage`, `.resw` resources, UI language picker — not modified.

## Build verification

| Command | Result |
|---------|--------|
| `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo` | **Succeeded** — 0 warnings, 0 errors |

## Self-review checklist

- [x] Matches brief code verbatim (structure, keys, normalization)
- [x] Wired in `App()` between theme and `InitializeComponent()`
- [x] Pattern aligned with `ThemeService` early-init hook
- [x] No git commit
- [x] No SettingsPage or resw changes

## Concerns

1. **Runtime language switch:** `SetLanguage` updates override without restart; UI strings from x:Uid/resw may not refresh until a later task wires restart or resource reload.
2. **Resource coverage:** Until resw and page bindings exist, override only affects resources that are already localized in the project.

## Next tasks (not done here)

- Settings UI for language selection calling `SetLanguage`.
- Localized `.resw` strings and x:Uid on pages.
- Optional app restart after language change.
