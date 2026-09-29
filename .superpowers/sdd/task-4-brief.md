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

