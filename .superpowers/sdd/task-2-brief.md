### Task 2: Scoop ThemeDictionaries (Light + Dark)

**Files:**
- Modify: `App.xaml`

**Interfaces:**
- Consumes: existing Accent ThemeDictionaries
- Produces ThemeResource keys (both Light and Dark):  
  `ScoopAmbientGradient`, `ScoopGlassAcrylicBrush`, `ScoopGlassStrongAcrylicBrush`, `ScoopGlassBorderBrush`, `ScoopTextPrimaryBrush`, `ScoopTextSecondaryBrush`, `ScoopTextTertiaryBrush`, `ScoopTextBodyBrush` (`#374151` / `#D1D5DB`), `ScoopSurfaceFillBrush` (`#F7F7F7` / `#252528`), `ScoopCardFillBrush` (`#FFFFFF` / `#2A2A2E`), `ScoopNavInactiveBrush`, `SoftInputFillBrush`, `ScoopCaptionHoverBrush`, `ScoopOrbBlueBrush` / `ScoopOrbPinkBrush` / `ScoopOrbVioletBrush` (RadialGradientBrush for MainWindow orbs)

- [ ] **Step 1: Move glass + text brushes into ThemeDictionaries**

Inside existing `<ResourceDictionary x:Key="Light">` add (keep accent colors already there):

```xml
<LinearGradientBrush x:Key="ScoopAmbientGradient" StartPoint="0,0" EndPoint="1,1">
    <GradientStop Color="#EEF2FF" Offset="0" />
    <GradientStop Color="#F5F3FF" Offset="0.38" />
    <GradientStop Color="#FDF2F8" Offset="0.72" />
    <GradientStop Color="#EFF6FF" Offset="1" />
</LinearGradientBrush>
<AcrylicBrush x:Key="ScoopGlassAcrylicBrush" TintColor="#FFFFFF" TintOpacity="0.42" TintLuminosityOpacity="0.92" FallbackColor="#F4F6FB" />
<AcrylicBrush x:Key="ScoopGlassStrongAcrylicBrush" TintColor="#FFFFFF" TintOpacity="0.62" TintLuminosityOpacity="0.95" FallbackColor="#FAFBFE" />
<SolidColorBrush x:Key="ScoopGlassBorderBrush" Color="#B3FFFFFF" />
<SolidColorBrush x:Key="ScoopTextPrimaryBrush" Color="#111827" />
<SolidColorBrush x:Key="ScoopTextSecondaryBrush" Color="#6B7280" />
<SolidColorBrush x:Key="ScoopTextTertiaryBrush" Color="#9CA3AF" />
<SolidColorBrush x:Key="ScoopTextBodyBrush" Color="#374151" />
<SolidColorBrush x:Key="ScoopSurfaceFillBrush" Color="#F7F7F7" />
<SolidColorBrush x:Key="ScoopCardFillBrush" Color="#FFFFFF" />
<SolidColorBrush x:Key="ScoopNavInactiveBrush" Color="#4B5563" />
<SolidColorBrush x:Key="SoftInputFillBrush" Color="#F2F3F5" />
<SolidColorBrush x:Key="ScoopCaptionHoverBrush" Color="#14000000" />
<SolidColorBrush x:Key="ScoopCaptionPressedBrush" Color="#26000000" />
```

Inside `<ResourceDictionary x:Key="Dark">` add paired dark values:

```xml
<LinearGradientBrush x:Key="ScoopAmbientGradient" StartPoint="0,0" EndPoint="1,1">
    <GradientStop Color="#0B1220" Offset="0" />
    <GradientStop Color="#15101F" Offset="0.38" />
    <GradientStop Color="#1A1218" Offset="0.72" />
    <GradientStop Color="#0F172A" Offset="1" />
</LinearGradientBrush>
<AcrylicBrush x:Key="ScoopGlassAcrylicBrush" TintColor="#1C1C1E" TintOpacity="0.72" TintLuminosityOpacity="0.85" FallbackColor="#1C1C1E" />
<AcrylicBrush x:Key="ScoopGlassStrongAcrylicBrush" TintColor="#2A2A2E" TintOpacity="0.82" TintLuminosityOpacity="0.9" FallbackColor="#2A2A2E" />
<SolidColorBrush x:Key="ScoopGlassBorderBrush" Color="#33FFFFFF" />
<SolidColorBrush x:Key="ScoopTextPrimaryBrush" Color="#F3F4F6" />
<SolidColorBrush x:Key="ScoopTextSecondaryBrush" Color="#9CA3AF" />
<SolidColorBrush x:Key="ScoopTextTertiaryBrush" Color="#6B7280" />
<SolidColorBrush x:Key="ScoopTextBodyBrush" Color="#D1D5DB" />
<SolidColorBrush x:Key="ScoopSurfaceFillBrush" Color="#252528" />
<SolidColorBrush x:Key="ScoopCardFillBrush" Color="#2A2A2E" />
<SolidColorBrush x:Key="ScoopNavInactiveBrush" Color="#9CA3AF" />
<SolidColorBrush x:Key="SoftInputFillBrush" Color="#3A3A3E" />
<SolidColorBrush x:Key="ScoopCaptionHoverBrush" Color="#14FFFFFF" />
<SolidColorBrush x:Key="ScoopCaptionPressedBrush" Color="#26FFFFFF" />
```

- [ ] **Step 2: Remove duplicate non-theme copies**

Delete the old top-level `ScoopAmbientGradient`, `ScoopGlass*`, `ScoopGlassBorderBrush`, `SoftInputFillBrush` from outside ThemeDictionaries (keys must exist only in Light/Dark).

Update styles to ThemeResource:

```xml
<Style x:Key="ScoopGlassCardStyle" TargetType="Border">
    <Setter Property="Background" Value="{ThemeResource ScoopGlassAcrylicBrush}" />
    <Setter Property="BorderBrush" Value="{ThemeResource ScoopGlassBorderBrush}" />
    ...
</Style>

<Style x:Key="DialogFieldLabelStyle" TargetType="TextBlock">
    <Setter Property="Foreground" Value="{ThemeResource ScoopTextPrimaryBrush}" />
    ...
</Style>

<Style x:Key="DialogFieldHintStyle" TargetType="TextBlock">
    <Setter Property="Foreground" Value="{ThemeResource ScoopTextTertiaryBrush}" />
    ...
</Style>

<Style x:Key="SoftCancelButtonStyle" ...>
    <Setter Property="Background" Value="{ThemeResource SoftInputFillBrush}" />
    <Setter Property="Foreground" Value="{ThemeResource ScoopTextBodyBrush}" />
    ...
</Style>
```

Update `TableLinkButtonStyle` ContentPresenter:

```xml
<ContentPresenter
    ...
    Foreground="{TemplateBinding Foreground}"
    Background="Transparent"
    ... />
```

And add setter:

```xml
<Setter Property="Foreground" Value="{ThemeResource AccentTextFillColorPrimaryBrush}" />
```

Caption hover states: use `{ThemeResource ScoopCaptionHoverBrush}` / `ScoopCaptionPressedBrush` instead of `#14000000`.

Keep `ScoopAccentColor` / `ScoopAccentBrush` as shared (non-theme) resources.

- [ ] **Step 3: Build**

Run: `dotnet build ScoopX.csproj -c Debug`  
Expected: SUCCEEDED. If XAML complains missing StaticResource for glass, ensure MainWindow uses `ThemeResource` (Task 3).

---

