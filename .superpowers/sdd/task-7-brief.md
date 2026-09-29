### Task 7: SettingsPage appearance control

**Files:**
- Create: `Pages/SettingsPage.xaml`
- Create: `Pages/SettingsPage.xaml.cs`
- Modify: `MainWindow.xaml.cs` (`NavigateTo`)

**Interfaces:**
- Consumes: `ThemeService.Instance.Preference`, `SetPreference`
- Produces: Settings navigation target

- [ ] **Step 1: SettingsPage XAML**

```xml
<?xml version="1.0" encoding="utf-8"?>
<Page
    x:Class="ScoopX.Pages.SettingsPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    Background="Transparent">

    <ScrollViewer Padding="20,16,24,24">
        <StackPanel Spacing="16" MaxWidth="560" HorizontalAlignment="Left">
            <TextBlock
                Text="璁剧疆"
                FontSize="18"
                FontWeight="SemiBold"
                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />

            <Border
                Background="{ThemeResource ScoopGlassStrongAcrylicBrush}"
                BorderBrush="{ThemeResource ScoopGlassBorderBrush}"
                BorderThickness="1"
                CornerRadius="16"
                Padding="20,16">
                <Grid ColumnSpacing="16">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="Auto" />
                    </Grid.ColumnDefinitions>
                    <StackPanel Spacing="4" VerticalAlignment="Center">
                        <TextBlock Text="澶栬" FontSize="14" FontWeight="SemiBold"
                                   Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                        <TextBlock Text="閫夋嫨娴呰壊銆佹繁鑹诧紝鎴栬窡闅?Windows 绯荤粺璁剧疆"
                                   FontSize="12"
                                   Foreground="{ThemeResource ScoopTextSecondaryBrush}"
                                   TextWrapping="Wrap" />
                    </StackPanel>
                    <ComboBox
                        x:Name="ThemeComboBox"
                        Grid.Column="1"
                        MinWidth="140"
                        VerticalAlignment="Center"
                        SelectionChanged="ThemeComboBox_SelectionChanged">
                        <ComboBoxItem Content="璺熼殢绯荤粺" Tag="System" />
                        <ComboBoxItem Content="娴呰壊" Tag="Light" />
                        <ComboBoxItem Content="娣辫壊" Tag="Dark" />
                    </ComboBox>
                </Grid>
            </Border>
        </StackPanel>
    </ScrollViewer>
</Page>
```

- [ ] **Step 2: Code-behind**

```csharp
using Microsoft.UI.Xaml.Controls;
using ScoopX.Services;

namespace ScoopX.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private bool _suppressThemeEvent;

        public SettingsPage()
        {
            InitializeComponent();
            _suppressThemeEvent = true;
            ThemeComboBox.SelectedIndex = ThemeService.Instance.Preference switch
            {
                AppThemePreference.Light => 1,
                AppThemePreference.Dark => 2,
                _ => 0
            };
            _suppressThemeEvent = false;
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressThemeEvent || ThemeComboBox.SelectedItem is not ComboBoxItem item)
            {
                return;
            }

            var tag = item.Tag as string ?? "System";
            var pref = tag switch
            {
                "Light" => AppThemePreference.Light,
                "Dark" => AppThemePreference.Dark,
                _ => AppThemePreference.System
            };
            ThemeService.Instance.SetPreference(pref);
        }
    }
}
```

- [ ] **Step 3: Navigate**

In `MainWindow.xaml.cs` `NavigateTo`:

```csharp
case "Settings":
    ContentFrame.Navigate(typeof(SettingsPage));
    break;
```

- [ ] **Step 4: Full manual acceptance**

1. Windows Light + 璺熼殢绯荤粺 鈫?looks like today鈥檚 light UI  
2. Windows Dark + 璺熼殢绯荤粺 鈫?dark glass, readable title/paths/toggles  
3. Force 娴呰壊 while Windows Dark 鈫?app stays light  
4. Force 娣辫壊 while Windows Light 鈫?app stays dark  
5. Restart 鈫?last preference restored  
6. Settings page opens from sidebar  

---

## Spec coverage check

| Spec item | Task |
|-----------|------|
| Scoop* ThemeDictionaries | Task 2 |
| ThemeService + LocalSettings | Task 1 |
| Default System | Task 1 |
| Settings 璺熼殢/娴?娣?| Task 7 |
| MainWindow / Home / Gauge / Websites / Dialogs | Tasks 3鈥? |
| No icon changes | (out of scope) |
| Nav readable in Dark | Tasks 2鈥? |

## Placeholder / consistency self-review

- `Apply()` double-assign bug called out; implementer must use the corrected switch.
- `ApplyApplicationThemeOnly` named in Task 1 preferred startup path 鈥?implement as a public method on `ThemeService`.
- Brush key names consistent: `ScoopTextBodyBrush`, `ScoopSurfaceFillBrush`, `ScoopCardFillBrush`, `ScoopNavInactiveBrush`.
