### Task 5: WebsitesPage links + table text

**Files:**
- Modify: `Pages/WebsitesPage.xaml`

- [ ] **Step 1: Link + row foregrounds**

On site name / path HyperlinkButtons (already `TableLinkButtonStyle`), set:

```xml
Foreground="{ThemeResource AccentTextFillColorPrimaryBrush}"
```

Inner TextBlocks:

```xml
Foreground="{ThemeResource AccentTextFillColorPrimaryBrush}"
```

For path, secondary look is OK:

```xml
Foreground="{ThemeResource ScoopTextSecondaryBrush}"
```

on the path TextBlock (and matching HyperlinkButton Foreground).

Remark / PhpVersion TextBlocks:

```xml
Foreground="{ThemeResource ScoopTextPrimaryBrush}"
```

璁剧疆 / 鍒犻櫎 HyperlinkButtons:

```xml
Style="{StaticResource TableLinkButtonStyle}"
Foreground="{ThemeResource AccentTextFillColorPrimaryBrush}"
```

Any glass card Background using StaticResource 鈫?ThemeResource.

- [ ] **Step 2: Build + manual**

Windows Dark: website list paths and 璁剧疆/鍒犻櫎 clearly visible; ToggleSwitch thumb/track match dark glass (no black-on-white mismatch).

---

