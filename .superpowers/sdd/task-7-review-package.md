# Task 7

 M MainWindow.xaml.cs
?? Pages/SettingsPage.xaml
?? Pages/SettingsPage.xaml.cs

## MainWindow.xaml.cs diff

diff --git a/MainWindow.xaml.cs b/MainWindow.xaml.cs
index 589187d..ce26f8d 100644
--- a/MainWindow.xaml.cs
+++ b/MainWindow.xaml.cs
@@ -4,26 +4,24 @@ using System.Windows.Input;
 using Microsoft.UI.Windowing;
 using Microsoft.UI.Xaml;
 using Microsoft.UI.Xaml.Controls;
 using Microsoft.UI.Xaml.Input;
 using Microsoft.UI.Xaml.Media;
 using Microsoft.UI.Xaml.Media.Animation;
 using ScoopX.Controls;
 using ScoopX.Pages;
+using ScoopX.Services;
 using Windows.Foundation;
 using Windows.UI;
 
 namespace ScoopX
 {
     public sealed partial class MainWindow : Window
     {
-        private static readonly SolidColorBrush ActiveBrush = new(Color.FromArgb(255, 37, 99, 235)); // #2563EB 鍏ㄥ眬涓昏壊
-        private static readonly SolidColorBrush InactiveBrush = new(Color.FromArgb(255, 75, 85, 99));
-
         private string _selectedTag = "Home";
         private Storyboard? _pillStoryboard;
         private bool _railReady;
         private double _pillY;
         private bool _isExitRequested;
 
         public ICommand ShowWindowCommand { get; }
 
@@ -57,20 +55,53 @@ namespace ScoopX
 
             ExtendsContentIntoTitleBar = true;
             SetTitleBar(AppTitleBar);
 
             AppWindow.Closing += AppWindow_Closing;
 
             SetWindowIcon();
             WireNavHover();
+            ThemeService.Instance.ThemeChanged += (_, _) => RefreshNavColors();
             ApplyNavSelection("Home", animatePill: false);
             NavigateTo("Home");
         }
 
+        private SolidColorBrush ActiveBrush =>
+            (SolidColorBrush)Application.Current.Resources["ScoopAccentBrush"];
+
+        private SolidColorBrush InactiveBrush =>
+            (SolidColorBrush)GetThemeBrush("ScoopNavInactiveBrush");
+
+        private void RefreshNavColors()
+        {
+            ApplyNavSelection(_selectedTag, animatePill: false);
+        }
+
+        private static string GetThemeDictionaryKey(FrameworkElement root)
+        {
+            var theme = root.ActualTheme;
+            if (theme == ElementTheme.Default)
+            {
+                theme = Application.Current.RequestedTheme == ApplicationTheme.Dark
+                    ? ElementTheme.Dark
+                    : ElementTheme.Light;
+            }
+
+            return theme == ElementTheme.Dark ? "Dark" : "Light";
+        }
+
+        private Brush GetThemeBrush(string key)
+        {
+            var root = (FrameworkElement)Content;
+            var dicts = Application.Current.Resources.ThemeDictionaries;
+            var rd = (ResourceDictionary)dicts[GetThemeDictionaryKey(root)];
+            return (Brush)rd[key];
+        }
+
         private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
         {
             // 闈炰富鍔ㄩ€€鍑烘椂锛屽叧闂彧鏄棌鍒版墭鐩?             if (_isExitRequested)
             {
                 return;
             }
 
@@ -187,16 +218,19 @@ namespace ScoopX
             switch (tag)
             {
                 case "Home":
                     ContentFrame.Navigate(typeof(HomePage));
                     break;
                 case "Websites":
                     ContentFrame.Navigate(typeof(WebsitesPage));
                     break;
+                case "Settings":
+                    ContentFrame.Navigate(typeof(SettingsPage));
+                    break;
                 default:
                     ContentFrame.Content = null;
                     break;
             }
         }
 
         public void NavigateNav(string tag)
         {
@@ -311,15 +345,15 @@ namespace ScoopX
                     break;
                 case "Settings":
                     NavSettingsIcon.Foreground = brush;
                     NavSettingsText.Foreground = brush;
                     break;
             }
         }
 
-        private static void SetNavItemState(FontIcon icon, TextBlock label, bool selected)
+        private void SetNavItemState(FontIcon icon, TextBlock label, bool selected)
         {
             icon.Foreground = selected ? ActiveBrush : InactiveBrush;
             label.Foreground = selected ? ActiveBrush : InactiveBrush;
         }
     }
 }

## SettingsPage.xaml

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

## SettingsPage.xaml.cs

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
