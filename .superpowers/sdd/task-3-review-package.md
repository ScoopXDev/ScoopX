# Review package Task 3

diff --git a/MainWindow.xaml b/MainWindow.xaml
index 2d735c4..9deec27 100644
--- a/MainWindow.xaml
+++ b/MainWindow.xaml
@@ -11,63 +11,45 @@
     mc:Ignorable="d"
     Title="ScoopX">
 
     <!-- 鑷粯姘涘洿娓愬彉锛屼笉鍐嶇敤 Mica 瀹炲簳 -->
     <Grid>
         <!-- 搴曞眰锛歀ogo 钃濈传绮夋煍鍜屾笎鍙?+ 鍏夋枒 -->
         <Grid IsHitTestVisible="False">
             <Grid.Background>
-                <StaticResource ResourceKey="ScoopAmbientGradient" />
+                <ThemeResource ResourceKey="ScoopAmbientGradient" />
             </Grid.Background>
 
             <Ellipse
                 Width="520"
                 Height="520"
                 HorizontalAlignment="Right"
                 VerticalAlignment="Top"
                 Margin="0,-160,-120,0"
-                Opacity="0.85">
-                <Ellipse.Fill>
-                    <RadialGradientBrush>
-                        <GradientStop Color="#B3BFDBFE" Offset="0" />
-                        <GradientStop Color="#00BFDBFE" Offset="1" />
-                    </RadialGradientBrush>
-                </Ellipse.Fill>
-            </Ellipse>
+                Opacity="0.85"
+                Fill="{ThemeResource ScoopOrbBlueBrush}" />
 
             <Ellipse
                 Width="460"
                 Height="460"
                 HorizontalAlignment="Left"
                 VerticalAlignment="Bottom"
                 Margin="-160,0,0,-120"
-                Opacity="0.8">
-                <Ellipse.Fill>
-                    <RadialGradientBrush>
-                        <GradientStop Color="#A6FBCFE8" Offset="0" />
-                        <GradientStop Color="#00FBCFE8" Offset="1" />
-                    </RadialGradientBrush>
-                </Ellipse.Fill>
-            </Ellipse>
+                Opacity="0.8"
+                Fill="{ThemeResource ScoopOrbPinkBrush}" />
 
             <Ellipse
                 Width="380"
                 Height="380"
                 HorizontalAlignment="Center"
                 VerticalAlignment="Center"
                 Margin="-40,80,180,0"
-                Opacity="0.7">
-                <Ellipse.Fill>
-                    <RadialGradientBrush>
-                        <GradientStop Color="#99DDD6FE" Offset="0" />
-                        <GradientStop Color="#00DDD6FE" Offset="1" />
-                    </RadialGradientBrush>
-                </Ellipse.Fill>
-            </Ellipse>
+                Opacity="0.7"
+                Fill="{ThemeResource ScoopOrbVioletBrush}" />
         </Grid>
 
         <Grid Background="Transparent">
             <Grid.RowDefinitions>
                 <RowDefinition Height="64" />
                 <RowDefinition Height="*" />
             </Grid.RowDefinitions>
             <Grid.ColumnDefinitions>
@@ -120,21 +102,21 @@
                                 HorizontalAlignment="Center"
                                 VerticalAlignment="Center" />
                         </Border>
                         <StackPanel VerticalAlignment="Center" Spacing="2">
                             <TextBlock
                                 Text="ScoopX Workspace"
                                 FontSize="16"
                                 FontWeight="SemiBold"
-                                Foreground="{ThemeResource TextFillColorPrimaryBrush}" />
+                                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                             <TextBlock
                                 Text="涓烘瘡涓€浣峆HP寮€鍙戣€呮墦閫犻珮鏁堝紑鍙戠幆澧冿紝涓撴敞姣忎竴娆″垱閫犮€?
                                 FontSize="12"
-                                Foreground="#6B7280" />
+                                Foreground="{ThemeResource ScoopTextSecondaryBrush}" />
                         </StackPanel>
                     </StackPanel>
                 </Grid>
 
                 <StackPanel
                     x:Name="CaptionButtons"
                     Orientation="Horizontal"
                     HorizontalAlignment="Right"
@@ -214,68 +196,68 @@
 
                         <controls:HandCursorButton
                             x:Name="NavWebsites"
                             Tag="Websites"
                             Height="56"
                             Style="{StaticResource SidebarNavButtonStyle}"
                             Click="NavItem_Click">
                             <StackPanel Spacing="2" HorizontalAlignment="Center" VerticalAlignment="Center">
-                                <FontIcon x:Name="NavWebsitesIcon" Glyph="&#xE774;" FontSize="20" Foreground="#4B5563" />
-                                <TextBlock x:Name="NavWebsitesText" Text="缃戠珯" FontSize="11" HorizontalAlignment="Center" Foreground="#4B5563" />
+                                <FontIcon x:Name="NavWebsitesIcon" Glyph="&#xE774;" FontSize="20" Foreground="{ThemeResource ScoopNavInactiveBrush}" />
+                                <TextBlock x:Name="NavWebsitesText" Text="缃戠珯" FontSize="11" HorizontalAlignment="Center" Foreground="{ThemeResource ScoopNavInactiveBrush}" />
                             </StackPanel>
                         </controls:HandCursorButton>
 
                         <controls:HandCursorButton
                             x:Name="NavDatabase"
                             Tag="Database"
                             Height="56"
                             Style="{StaticResource SidebarNavButtonStyle}"
                             Click="NavItem_Click">
                             <StackPanel Spacing="2" HorizontalAlignment="Center" VerticalAlignment="Center">
-                                <FontIcon x:Name="NavDatabaseIcon" Glyph="&#xEE94;" FontSize="20" Foreground="#4B5563" />
-                                <TextBlock x:Name="NavDatabaseText" Text="鏁版嵁搴? FontSize="11" HorizontalAlignment="Center" Foreground="#4B5563" />
+                                <FontIcon x:Name="NavDatabaseIcon" Glyph="&#xEE94;" FontSize="20" Foreground="{ThemeResource ScoopNavInactiveBrush}" />
+                                <TextBlock x:Name="NavDatabaseText" Text="鏁版嵁搴? FontSize="11" HorizontalAlignment="Center" Foreground="{ThemeResource ScoopNavInactiveBrush}" />
                             </StackPanel>
                         </controls:HandCursorButton>
 
                         <controls:HandCursorButton
                             x:Name="NavStore"
                             Tag="Store"
                             Height="56"
                             Style="{StaticResource SidebarNavButtonStyle}"
                             Click="NavItem_Click">
                             <StackPanel Spacing="2" HorizontalAlignment="Center" VerticalAlignment="Center">
-                                <FontIcon x:Name="NavStoreIcon" Glyph="&#xE74C;" FontSize="20" Foreground="#4B5563" />
-                                <TextBlock x:Name="NavStoreText" Text="搴旂敤鍟嗗簵" FontSize="11" HorizontalAlignment="Center" Foreground="#4B5563" />
+                                <FontIcon x:Name="NavStoreIcon" Glyph="&#xE74C;" FontSize="20" Foreground="{ThemeResource ScoopNavInactiveBrush}" />
+                                <TextBlock x:Name="NavStoreText" Text="搴旂敤鍟嗗簵" FontSize="11" HorizontalAlignment="Center" Foreground="{ThemeResource ScoopNavInactiveBrush}" />
                             </StackPanel>
                         </controls:HandCursorButton>
                     </StackPanel>
 
                     <controls:HandCursorButton
                         x:Name="NavSettings"
                         Grid.Row="1"
                         Tag="Settings"
                         Height="56"
                         Style="{StaticResource SidebarNavButtonStyle}"
                         Click="NavItem_Click">
                         <StackPanel Spacing="2" HorizontalAlignment="Center" VerticalAlignment="Center">
-                            <FontIcon x:Name="NavSettingsIcon" Glyph="&#xE713;" FontSize="20" Foreground="#4B5563" />
-                            <TextBlock x:Name="NavSettingsText" Text="璁剧疆" FontSize="11" HorizontalAlignment="Center" Foreground="#4B5563" />
+                            <FontIcon x:Name="NavSettingsIcon" Glyph="&#xE713;" FontSize="20" Foreground="{ThemeResource ScoopNavInactiveBrush}" />
+                            <TextBlock x:Name="NavSettingsText" Text="璁剧疆" FontSize="11" HorizontalAlignment="Center" Foreground="{ThemeResource ScoopNavInactiveBrush}" />
                         </StackPanel>
                     </controls:HandCursorButton>
                 </Grid>
             </Grid>
 
             <!-- 鍐呭鍖猴細璐村彸璐村簳锛屼粎宸︿笂鍦嗚锛汚pple 姣涚幓鐠?-->
             <Border
                 Grid.Row="1"
                 Grid.Column="1"
                 Margin="0"
-                Background="{StaticResource ScoopGlassAcrylicBrush}"
-                BorderBrush="{StaticResource ScoopGlassBorderBrush}"
+                Background="{ThemeResource ScoopGlassAcrylicBrush}"
+                BorderBrush="{ThemeResource ScoopGlassBorderBrush}"
                 BorderThickness="1,1,0,0"
                 CornerRadius="16,0,0,0">
                 <Frame
                     x:Name="ContentFrame"
                     Background="Transparent"
                     BorderThickness="0" />
             </Border>
         </Grid>
diff --git a/MainWindow.xaml.cs b/MainWindow.xaml.cs
index 589187d..5739cd2 100644
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
 
@@ -311,15 +342,15 @@ namespace ScoopX
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
