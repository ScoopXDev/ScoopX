### Task 3: MainWindow theme wiring

**Files:**
- Modify: `MainWindow.xaml`
- Modify: `MainWindow.xaml.cs`

**Interfaces:**
- Consumes: `ScoopAmbientGradient`, text/nav brushes, `ThemeService.ThemeChanged`
- Produces: nav colors refresh on theme change

- [ ] **Step 1: XAML brushes**

Replace:

```xml
<StaticResource ResourceKey="ScoopAmbientGradient" />
```

with:

```xml
<ThemeResource ResourceKey="ScoopAmbientGradient" />
```

Title / subtitle:

```xml
<TextBlock Text="ScoopX Workspace" ... Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
<TextBlock Text="涓烘瘡涓€浣?.." ... Foreground="{ThemeResource ScoopTextSecondaryBrush}" />
```

Content glass borders that use `StaticResource ScoopGlassAcrylicBrush` 鈫?`ThemeResource`.

Nav inactive XAML defaults `#4B5563` 鈫?`{ThemeResource ScoopNavInactiveBrush}` for all inactive FontIcon/TextBlock initial Foreground.

Orbs: reduce opacity in dark by binding Fill to theme-specific brushes if added in Task 2; otherwise leave orbs but lower Opacity is optional. Prefer adding three `RadialGradientBrush` keys per theme (`ScoopOrbBlue`, `ScoopOrbPink`, `ScoopOrbViolet`) with darker alpha stops in Dark dictionary, then reference via ThemeResource.

- [ ] **Step 2: Code-behind nav brushes**

Replace static brushes with helpers that read current theme resources from `App.Current.Resources` / themed dictionary, or subscribe to `ThemeService.ThemeChanged` and refresh:

```csharp
private SolidColorBrush ActiveBrush =>
    (SolidColorBrush)Application.Current.Resources["ScoopAccentBrush"];

private SolidColorBrush InactiveBrush =>
    (SolidColorBrush)((FrameworkElement)Content).Resources["ScoopNavInactiveBrush"]
    // Prefer: lookup from Content root ActualTheme
```

Practical approach: after `InitializeComponent`, subscribe:

```csharp
ThemeService.Instance.ThemeChanged += (_, _) => RefreshNavColors();
```

And implement `RefreshNavColors()` to re-apply `ApplyNavSelection(_selectedTag, animatePill: false)` using brushes resolved from:

```csharp
private Brush ResolveBrush(string key)
{
    var root = (FrameworkElement)Content;
    if (root.Resources.TryGetValue(key, out var local) && local is Brush b1)
        return b1;
    return (Brush)Application.Current.Resources[key];
}
```

Theme dictionaries resolve via `ThemeResource` on elements; for code-behind use:

```csharp
private Brush GetThemeBrush(string key)
{
    return (Brush)((FrameworkElement)Content).FindResource? // not available
}
```

WinUI pattern: put `ScoopNavInactiveBrush` also as a keyed resource accessible via `(SolidColorBrush)App.Current.Resources[key]` only works for non-theme keys. For theme keys from code:

```csharp
var theme = ((FrameworkElement)Content).ActualTheme;
var dicts = Application.Current.Resources.ThemeDictionaries;
var rd = (ResourceDictionary)dicts[theme == ElementTheme.Dark ? "Dark" : "Light"];
return (Brush)rd[key];
```

Handle `ElementTheme.Default` by mapping with `Application.Current.RequestedTheme`.

- [ ] **Step 3: Build + manual**

Run: `dotnet build`  
Manual: Windows Dark + default System 鈫?title readable on dark gradient; nav inactive not washed out.

---

