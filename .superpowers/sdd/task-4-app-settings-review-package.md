# Task 4 review package
Untracked new/updated SettingsPage files (working tree). Review these files in full.

## Files to Read (required)
- Pages/SettingsPage.xaml
- Pages/SettingsPage.xaml.cs

## Also compare against brief expectations
Appearance section must remain; new sections: 常规, 路径, 关于.
## Line counts
SettingsPage.xaml: 423 lines
SettingsPage.xaml.cs: 214 lines

## SettingsPage.xaml.cs (full)
```csharp
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using ScoopX.Controls;
using ScoopX.Services;
using Windows.ApplicationModel;
using Windows.Storage.Pickers;
using Windows.UI;
using WinRT.Interop;

namespace ScoopX.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private static readonly SolidColorBrush SelectedBorderBrush =
            new(Color.FromArgb(255, 37, 99, 235)); // #2563EB 涓庝晶鏍忛€変腑涓€鑷?
        private static readonly SolidColorBrush TransparentBrush =
            new(Color.FromArgb(0, 0, 0, 0));

        private bool _suppressToggle;

        public SettingsPage()
        {
            InitializeComponent();
            UpdateSelectionChrome(ThemeService.Instance.Preference);
            ThemeService.Instance.ThemeChanged += OnThemeChanged;
            Unloaded += (_, _) => ThemeService.Instance.ThemeChanged -= OnThemeChanged;

            LoadAppSettings();
            AboutVersionText.Text = GetVersionString();
        }

        private void LoadAppSettings()
        {
            _suppressToggle = true;
            try
            {
                var settings = AppSettings.Instance;
                CloseToTrayToggle.IsOn = settings.CloseToTray;
                StartWithWindowsToggle.IsOn = settings.StartWithWindows;
                StartHiddenToggle.IsOn = settings.StartHiddenToTray;
                ScoopRootTextBox.Text = settings.ScoopRootPath;
                WebsitesRootTextBox.Text = settings.WebsitesRootPath;
            }
            finally
            {
                _suppressToggle = false;
            }
        }

        private static string GetVersionString()
        {
            try
            {
                var v = Package.Current.Id.Version;
                return $"鐗堟湰 {v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
            catch
            {
                var v = typeof(App).Assembly.GetName().Version;
                return v is null ? "鐗堟湰 1.0.0" : $"鐗堟湰 {v.Major}.{v.Minor}.{v.Build}";
            }
        }

        private void OnThemeChanged(object? sender, EventArgs e)
        {
            UpdateSelectionChrome(ThemeService.Instance.Preference);
        }

        private void ThemeOption_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not HandCursorButton { Tag: string tag })
            {
                return;
            }

            var pref = tag switch
            {
                "Light" => AppThemePreference.Light,
                "Dark" => AppThemePreference.Dark,
                _ => AppThemePreference.System
            };

            if (pref == ThemeService.Instance.Preference)
            {
                UpdateSelectionChrome(pref);
                return;
            }

            ThemeService.Instance.SetPreference(pref);
            UpdateSelectionChrome(pref);
        }

        private void UpdateSelectionChrome(AppThemePreference preference)
        {
            ThemeLightFrame.BorderBrush = preference == AppThemePreference.Light
                ? SelectedBorderBrush
                : TransparentBrush;
            ThemeDarkFrame.BorderBrush = preference == AppThemePreference.Dark
                ? SelectedBorderBrush
                : TransparentBrush;
            ThemeSystemFrame.BorderBrush = preference == AppThemePreference.System
                ? SelectedBorderBrush
                : TransparentBrush;
        }

        private void CloseToTrayToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (_suppressToggle)
            {
                return;
            }

            AppSettings.Instance.CloseToTray = CloseToTrayToggle.IsOn;
        }

        private async void StartWithWindowsToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (_suppressToggle)
            {
                return;
            }

            var desired = StartWithWindowsToggle.IsOn;
            var (ok, error) = await AppSettings.Instance.SetStartWithWindowsAsync(desired);
            if (ok)
            {
                return;
            }

            _suppressToggle = true;
            try
            {
                StartWithWindowsToggle.IsOn = AppSettings.Instance.StartWithWindows;
            }
            finally
            {
                _suppressToggle = false;
            }

            var dialog = new ContentDialog
            {
                Title = "鏃犳硶鏇存敼寮€鏈鸿嚜鍚?,
                Content = error ?? "鎿嶄綔澶辫触銆?,
                CloseButtonText = "纭畾",
                XamlRoot = XamlRoot,
                RequestedTheme = ThemeService.Instance.CurrentElementTheme,
            };
            await dialog.ShowAsync();
        }

        private void StartHiddenToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (_suppressToggle)
            {
                return;
            }

            AppSettings.Instance.StartHiddenToTray = StartHiddenToggle.IsOn;
        }

        private void ScoopRootTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.ScoopRootPath = ScoopRootTextBox.Text?.Trim() ?? "";
        }

        private void WebsitesRootTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.WebsitesRootPath = WebsitesRootTextBox.Text?.Trim() ?? "";
        }

        private async void BrowseScoopRootButton_Click(object sender, RoutedEventArgs e)
        {
            var path = await PickFolderAsync();
            if (path is null)
            {
                return;
            }

            ScoopRootTextBox.Text = path;
            AppSettings.Instance.ScoopRootPath = path;
        }

        private async void BrowseWebsitesRootButton_Click(object sender, RoutedEventArgs e)
        {
            var path = await PickFolderAsync();
            if (path is null)
            {
                return;
            }

            WebsitesRootTextBox.Text = path;
            AppSettings.Instance.WebsitesRootPath = path;
        }

        private async Task<string?> PickFolderAsync()
        {
            var picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add("*");

            if (App.MainWindow is not null)
            {
                var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
                InitializeWithWindow.Initialize(picker, hwnd);
            }

            var folder = await picker.PickSingleFolderAsync();
            return folder?.Path;
        }
    }
}
```

## SettingsPage.xaml (full)
```xml
<?xml version="1.0" encoding="utf-8"?>
<Page
    x:Class="ScoopX.Pages.SettingsPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:controls="using:ScoopX.Controls"
    Background="Transparent">

    <Page.Resources>
        <Style x:Key="ThemeOptionButtonStyle" TargetType="controls:HandCursorButton" BasedOn="{StaticResource SidebarNavButtonStyle}">
            <Setter Property="HorizontalAlignment" Value="Center" />
            <Setter Property="HorizontalContentAlignment" Value="Center" />
            <Setter Property="VerticalContentAlignment" Value="Top" />
            <Setter Property="Padding" Value="4" />
        </Style>
    </Page.Resources>

    <ScrollViewer Padding="20,16,24,24">
        <StackPanel Spacing="14" MaxWidth="640" HorizontalAlignment="Left">
            <TextBlock
                Text="璁剧疆"
                FontSize="20"
                FontWeight="SemiBold"
                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />

            <TextBlock
                Text="澶栬璁剧疆"
                FontSize="13"
                FontWeight="SemiBold"
                Foreground="{ThemeResource ScoopTextSecondaryBrush}"
                Margin="0,4,0,0" />

            <!-- 娴呯伆/娣辩伆搴曟澘锛屼笌涓婃柟鏍囬鍜屼富鍐呭鐜荤拑鍖哄垎寮€锛堟棤鎻忚竟锛?-->
            <Border
                Background="{ThemeResource ScoopSurfaceFillBrush}"
                BorderThickness="0"
                CornerRadius="10"
                Padding="20,22,20,18">
                <Grid ColumnSpacing="20">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                    </Grid.ColumnDefinitions>

                    <!-- 娴呰壊妯″紡 -->
                    <controls:HandCursorButton
                        x:Name="ThemeLightButton"
                        Grid.Column="0"
                        Tag="Light"
                        Style="{StaticResource ThemeOptionButtonStyle}"
                        Click="ThemeOption_Click">
                        <StackPanel Spacing="12" HorizontalAlignment="Center">
                            <!-- 閫変腑鐜簳鑹蹭笌鍗＄墖鍚岃壊锛岄伩鍏嶉€忔槑缂濋噷鍑虹幇鐧借竟/姣涜竟 -->
                            <Border
                                x:Name="ThemeLightFrame"
                                Width="118"
                                Height="78"
                                CornerRadius="10"
                                BorderThickness="2"
                                BorderBrush="Transparent"
                                Padding="2"
                                Background="{ThemeResource ScoopSurfaceFillBrush}">
                                <Border CornerRadius="6" Background="#F4F5F7" BorderThickness="0">
                                    <Grid>
                                        <Grid.ColumnDefinitions>
                                            <ColumnDefinition Width="20" />
                                            <ColumnDefinition Width="*" />
                                            <ColumnDefinition Width="1.15*" />
                                        </Grid.ColumnDefinitions>
                                        <!-- 渚ф爮鑹茬敤绐勬潯锛屼笉閾烘弧鏁村垪鏂瑰簳锛屽噺灏戝渾瑙掔櫧杈?-->
                                        <Border Width="20" HorizontalAlignment="Left" Background="#E8EEF9" />
                                        <StackPanel Grid.Column="0" VerticalAlignment="Center" Spacing="5">
                                            <Ellipse Width="5" Height="5" Fill="#2563EB" HorizontalAlignment="Center" />
                                            <Ellipse Width="5" Height="5" Fill="#C5CDD8" HorizontalAlignment="Center" />
                                            <Ellipse Width="5" Height="5" Fill="#C5CDD8" HorizontalAlignment="Center" />
                                        </StackPanel>
                                        <StackPanel Grid.Column="1" Margin="5,8,4,8" Spacing="4" VerticalAlignment="Top">
                                            <Border Height="4" CornerRadius="2" Background="#E0E2E6" />
                                            <Border Height="4" Width="22" CornerRadius="2" HorizontalAlignment="Left" Background="#E0E2E6" />
                                            <Border Height="4" CornerRadius="2" Background="#E8EAED" />
                                            <Border Height="4" Width="18" CornerRadius="2" HorizontalAlignment="Left" Background="#E8EAED" />
                                        </StackPanel>
                                        <Border
                                            Grid.Column="2"
                                            Width="26"
                                            Height="12"
                                            Margin="0,14,8,0"
                                            HorizontalAlignment="Right"
                                            VerticalAlignment="Top"
                                            Background="#2563EB"
                                            CornerRadius="6" />
                                    </Grid>
                                </Border>
                            </Border>
                            <TextBlock
                                Text="娴呰壊妯″紡"
                                FontSize="13"
                                HorizontalAlignment="Center"
                                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                        </StackPanel>
                    </controls:HandCursorButton>

                    <!-- 娣辫壊妯″紡 -->
                    <controls:HandCursorButton
                        x:Name="ThemeDarkButton"
                        Grid.Column="1"
                        Tag="Dark"
                        Style="{StaticResource ThemeOptionButtonStyle}"
                        Click="ThemeOption_Click">
                        <StackPanel Spacing="12" HorizontalAlignment="Center">
                            <Border
                                x:Name="ThemeDarkFrame"
                                Width="118"
                                Height="78"
                                CornerRadius="10"
                                BorderThickness="2"
                                BorderBrush="Transparent"
                                Padding="2"
                                Background="{ThemeResource ScoopSurfaceFillBrush}">
                                <Border CornerRadius="6" Background="#1A1A1C" BorderThickness="0">
                                    <Grid>
                                        <Grid.ColumnDefinitions>
                                            <ColumnDefinition Width="20" />
                                            <ColumnDefinition Width="*" />
                                            <ColumnDefinition Width="1.15*" />
                                        </Grid.ColumnDefinitions>
                                        <Border Width="20" HorizontalAlignment="Left" Background="#111113" />
                                        <StackPanel Grid.Column="0" VerticalAlignment="Center" Spacing="5">
                                            <Ellipse Width="5" Height="5" Fill="#2563EB" HorizontalAlignment="Center" />
                                            <Ellipse Width="5" Height="5" Fill="#4B5563" HorizontalAlignment="Center" />
                                            <Ellipse Width="5" Height="5" Fill="#4B5563" HorizontalAlignment="Center" />
                                        </StackPanel>
                                        <StackPanel Grid.Column="1" Margin="5,8,4,8" Spacing="4" VerticalAlignment="Top">
                                            <Border Height="4" CornerRadius="2" Background="#3A3A3E" />
                                            <Border Height="4" Width="22" CornerRadius="2" HorizontalAlignment="Left" Background="#3A3A3E" />
                                            <Border Height="4" CornerRadius="2" Background="#323236" />
                                            <Border Height="4" Width="18" CornerRadius="2" HorizontalAlignment="Left" Background="#323236" />
                                        </StackPanel>
                                        <Border
                                            Grid.Column="2"
                                            Width="26"
                                            Height="12"
                                            Margin="0,14,8,0"
                                            HorizontalAlignment="Right"
                                            VerticalAlignment="Top"
                                            Background="#2563EB"
                                            CornerRadius="6" />
                                    </Grid>
                                </Border>
                            </Border>
                            <TextBlock
                                Text="娣辫壊妯″紡"
                                FontSize="13"
                                HorizontalAlignment="Center"
                                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                        </StackPanel>
                    </controls:HandCursorButton>

                    <!-- 璺熼殢绯荤粺 -->
                    <controls:HandCursorButton
                        x:Name="ThemeSystemButton"
                        Grid.Column="2"
                        Tag="System"
                        Style="{StaticResource ThemeOptionButtonStyle}"
                        Click="ThemeOption_Click">
                        <StackPanel Spacing="12" HorizontalAlignment="Center">
                            <Border
                                x:Name="ThemeSystemFrame"
                                Width="118"
                                Height="78"
                                CornerRadius="10"
                                BorderThickness="2"
                                BorderBrush="Transparent"
                                Padding="2"
                                Background="{ThemeResource ScoopSurfaceFillBrush}">
                                <!-- 鍗曞眰鍦嗚搴?+ 宸﹀彸娓愬彉锛歐inUI 涓嶄細瑁佸垏瀛愬厓绱犲渾瑙掞紝鍒嗕袱鍧楁柟搴曚細鍦ㄦ繁鑹蹭晶闇插嚭鐧借竟 -->
                                <Border CornerRadius="6" BorderThickness="0">
                                    <Border.Background>
                                        <LinearGradientBrush StartPoint="0,0" EndPoint="1,0">
                                            <GradientStop Color="#F4F5F7" Offset="0" />
                                            <GradientStop Color="#F4F5F7" Offset="0.499" />
                                            <GradientStop Color="#1A1A1C" Offset="0.501" />
                                            <GradientStop Color="#1A1A1C" Offset="1" />
                                        </LinearGradientBrush>
                                    </Border.Background>
                                    <Grid>
                                        <Grid.ColumnDefinitions>
                                            <ColumnDefinition Width="*" />
                                            <ColumnDefinition Width="*" />
                                        </Grid.ColumnDefinitions>
                                        <!-- 浠呰楗帮紝涓嶉摵婊℃柟搴曪紝閬垮厤鍦嗚婧㈠嚭 -->
                                        <StackPanel
                                            Margin="5,0,0,0"
                                            VerticalAlignment="Center"
                                            Spacing="3"
                                            HorizontalAlignment="Left">
                                            <Ellipse Width="3.5" Height="3.5" Fill="#2563EB" />
                                            <Ellipse Width="3.5" Height="3.5" Fill="#C5CDD8" />
                                            <Ellipse Width="3.5" Height="3.5" Fill="#C5CDD8" />
                                        </StackPanel>
                                        <Border
                                            Width="14"
                                            Height="8"
                                            Margin="0,12,8,0"
                                            HorizontalAlignment="Right"
                                            VerticalAlignment="Top"
                                            Background="#2563EB"
                                            CornerRadius="3" />
                                        <StackPanel
                                            Grid.Column="1"
                                            Margin="5,0,0,0"
                                            VerticalAlignment="Center"
                                            Spacing="3"
                                            HorizontalAlignment="Left">
                                            <Ellipse Width="3.5" Height="3.5" Fill="#2563EB" />
                                            <Ellipse Width="3.5" Height="3.5" Fill="#4B5563" />
                                            <Ellipse Width="3.5" Height="3.5" Fill="#4B5563" />
                                        </StackPanel>
                                        <Border
                                            Grid.Column="1"
                                            Width="14"
                                            Height="8"
                                            Margin="0,12,8,0"
                                            HorizontalAlignment="Right"
                                            VerticalAlignment="Top"
                                            Background="#2563EB"
                                            CornerRadius="3" />
                                    </Grid>
                                </Border>
                            </Border>
                            <TextBlock
                                Text="璺熼殢绯荤粺"
                                FontSize="13"
                                HorizontalAlignment="Center"
                                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                        </StackPanel>
                    </controls:HandCursorButton>
                </Grid>
            </Border>

            <!-- 甯歌 -->
            <TextBlock
                Text="甯歌"
                FontSize="13"
                FontWeight="SemiBold"
                Foreground="{ThemeResource ScoopTextSecondaryBrush}"
                Margin="0,8,0,0" />
            <Border
                Background="{ThemeResource ScoopSurfaceFillBrush}"
                BorderThickness="0"
                CornerRadius="10"
                Padding="16,8">
                <StackPanel Spacing="4">
                    <Grid ColumnSpacing="12" MinHeight="52">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>
                        <StackPanel VerticalAlignment="Center" Spacing="2">
                            <TextBlock
                                Text="鍏抽棴鏃舵渶灏忓寲鍒版墭鐩?
                                FontSize="14"
                                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                            <TextBlock
                                Text="鍏抽棴绐楀彛鏃堕殣钘忓埌绯荤粺鎵樼洏锛岃€屼笉鏄€€鍑?
                                FontSize="12"
                                TextWrapping="Wrap"
                                Foreground="{ThemeResource ScoopTextSecondaryBrush}" />
                        </StackPanel>
                        <ToggleSwitch
                            x:Name="CloseToTrayToggle"
                            Grid.Column="1"
                            MinWidth="0"
                            VerticalAlignment="Center"
                            OnContent=""
                            OffContent=""
                            Toggled="CloseToTrayToggle_Toggled" />
                    </Grid>
                    <Grid ColumnSpacing="12" MinHeight="52">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>
                        <StackPanel VerticalAlignment="Center" Spacing="2">
                            <TextBlock
                                Text="寮€鏈烘椂鍚姩"
                                FontSize="14"
                                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                            <TextBlock
                                Text="鐧诲綍 Windows 鍚庤嚜鍔ㄥ惎鍔?ScoopX"
                                FontSize="12"
                                TextWrapping="Wrap"
                                Foreground="{ThemeResource ScoopTextSecondaryBrush}" />
                        </StackPanel>
                        <ToggleSwitch
                            x:Name="StartWithWindowsToggle"
                            Grid.Column="1"
                            MinWidth="0"
                            VerticalAlignment="Center"
                            OnContent=""
                            OffContent=""
                            Toggled="StartWithWindowsToggle_Toggled" />
                    </Grid>
                    <Grid ColumnSpacing="12" MinHeight="52">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>
                        <StackPanel VerticalAlignment="Center" Spacing="2">
                            <TextBlock
                                Text="鍚姩鏃堕殣钘忓埌鎵樼洏"
                                FontSize="14"
                                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                            <TextBlock
                                Text="鍚姩鍚庝笉鏄剧ず涓荤獥鍙ｏ紝浠呮樉绀烘墭鐩樺浘鏍?
                                FontSize="12"
                                TextWrapping="Wrap"
                                Foreground="{ThemeResource ScoopTextSecondaryBrush}" />
                        </StackPanel>
                        <ToggleSwitch
                            x:Name="StartHiddenToggle"
                            Grid.Column="1"
                            MinWidth="0"
                            VerticalAlignment="Center"
                            OnContent=""
                            OffContent=""
                            Toggled="StartHiddenToggle_Toggled" />
                    </Grid>
                </StackPanel>
            </Border>

            <!-- 璺緞 -->
            <TextBlock
                Text="璺緞"
                FontSize="13"
                FontWeight="SemiBold"
                Foreground="{ThemeResource ScoopTextSecondaryBrush}"
                Margin="0,8,0,0" />
            <Border
                Background="{ThemeResource ScoopSurfaceFillBrush}"
                BorderThickness="0"
                CornerRadius="10"
                Padding="16,14">
                <StackPanel Spacing="14">
                    <StackPanel Spacing="6">
                        <TextBlock
                            Text="Scoop 鏍圭洰褰?
                            FontSize="13"
                            Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                        <Grid ColumnSpacing="8">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*" />
                                <ColumnDefinition Width="Auto" />
                            </Grid.ColumnDefinitions>
                            <TextBox
                                x:Name="ScoopRootTextBox"
                                PlaceholderText="閫夋嫨 Scoop 鏍圭洰褰?
                                CornerRadius="6"
                                LostFocus="ScoopRootTextBox_LostFocus" />
                            <Button
                                x:Name="BrowseScoopRootButton"
                                Grid.Column="1"
                                Content="娴忚"
                                MinWidth="72"
                                Style="{StaticResource SoftCancelButtonStyle}"
                                Click="BrowseScoopRootButton_Click" />
                        </Grid>
                    </StackPanel>
                    <StackPanel Spacing="6">
                        <TextBlock
                            Text="缃戠珯榛樿鏍圭洰褰?
                            FontSize="13"
                            Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                        <Grid ColumnSpacing="8">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*" />
                                <ColumnDefinition Width="Auto" />
                            </Grid.ColumnDefinitions>
                            <TextBox
                                x:Name="WebsitesRootTextBox"
                                PlaceholderText="閫夋嫨缃戠珯榛樿鏍圭洰褰?
                                CornerRadius="6"
                                LostFocus="WebsitesRootTextBox_LostFocus" />
                            <Button
                                x:Name="BrowseWebsitesRootButton"
                                Grid.Column="1"
                                Content="娴忚"
                                MinWidth="72"
                                Style="{StaticResource SoftCancelButtonStyle}"
                                Click="BrowseWebsitesRootButton_Click" />
                        </Grid>
                    </StackPanel>
                </StackPanel>
            </Border>

            <!-- 鍏充簬 -->
            <TextBlock
                Text="鍏充簬"
                FontSize="13"
                FontWeight="SemiBold"
                Foreground="{ThemeResource ScoopTextSecondaryBrush}"
                Margin="0,8,0,0" />
            <Border
                Background="{ThemeResource ScoopSurfaceFillBrush}"
                BorderThickness="0"
                CornerRadius="10"
                Padding="16,14">
                <StackPanel Spacing="6">
                    <TextBlock
                        Text="ScoopX"
                        FontSize="15"
                        FontWeight="SemiBold"
                        Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                    <TextBlock
                        x:Name="AboutVersionText"
                        FontSize="12"
                        Foreground="{ThemeResource ScoopTextSecondaryBrush}" />
                </StackPanel>
            </Border>
        </StackPanel>
    </ScrollViewer>
</Page>
```
