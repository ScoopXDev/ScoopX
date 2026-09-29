# Final whole-branch review package
MERGE_BASE: 0b17adbd
HEAD: working tree (uncommitted on feature/dark-theme)
Plan: docs/superpowers/plans/2026-09-29-dark-theme.md
Spec: docs/superpowers/specs/2026-09-29-dark-theme-design.md

## Minor findings from task reviews (for triage)
- HomePage.xaml.cs chart grid/axis still hardcoded
- WebsitesPage table header TextBlocks no Scoop foreground
- EditSiteDialog nav SelectNav still light hardcoded colors
- Manual dark-mode QA not run in agents
- Build requires -p:Platform=x64

## Status
 M App.xaml
 M App.xaml.cs
 M Controls/StatusGauge.xaml
 M Controls/StatusGauge.xaml.cs
 M MainWindow.xaml
 M MainWindow.xaml.cs
 M Pages/AddSiteDialog.xaml
 M Pages/EditSiteDialog.xaml
 M Pages/HomePage.xaml
 M Pages/WebsitesPage.xaml
?? .superpowers/
?? Pages/SettingsPage.xaml
?? Pages/SettingsPage.xaml.cs
?? Services/
?? docs/

## Diff stat vs HEAD (tracked)

 App.xaml                     | 107 ++++++++++++++++++++++++++++++-------------
 App.xaml.cs                  |   4 ++
 Controls/StatusGauge.xaml    |  10 ++--
 Controls/StatusGauge.xaml.cs |   3 --
 MainWindow.xaml              |  56 ++++++++--------------
 MainWindow.xaml.cs           |  42 +++++++++++++++--
 Pages/AddSiteDialog.xaml     |   5 +-
 Pages/EditSiteDialog.xaml    |   9 ++--
 Pages/HomePage.xaml          |  26 +++++------
 Pages/WebsitesPage.xaml      |  18 ++++++--
 10 files changed, 170 insertions(+), 110 deletions(-)

## Full tracked diff

diff --git a/App.xaml b/App.xaml
index 2f075f6..4a61475 100644
--- a/App.xaml
+++ b/App.xaml
@@ -25,10 +25,42 @@
                     <SolidColorBrush x:Key="AccentFillColorSecondaryBrush" Color="#CC2563EB" />
                     <SolidColorBrush x:Key="AccentFillColorTertiaryBrush" Color="#992563EB" />
                     <SolidColorBrush x:Key="AccentTextFillColorPrimaryBrush" Color="#2563EB" />
                     <SolidColorBrush x:Key="AccentTextFillColorSecondaryBrush" Color="#CC2563EB" />
                     <SolidColorBrush x:Key="AccentTextFillColorTertiaryBrush" Color="#992563EB" />
+                    <LinearGradientBrush x:Key="ScoopAmbientGradient" StartPoint="0,0" EndPoint="1,1">
+                        <GradientStop Color="#EEF2FF" Offset="0" />
+                        <GradientStop Color="#F5F3FF" Offset="0.38" />
+                        <GradientStop Color="#FDF2F8" Offset="0.72" />
+                        <GradientStop Color="#EFF6FF" Offset="1" />
+                    </LinearGradientBrush>
+                    <AcrylicBrush x:Key="ScoopGlassAcrylicBrush" TintColor="#FFFFFF" TintOpacity="0.42" TintLuminosityOpacity="0.92" FallbackColor="#F4F6FB" />
+                    <AcrylicBrush x:Key="ScoopGlassStrongAcrylicBrush" TintColor="#FFFFFF" TintOpacity="0.62" TintLuminosityOpacity="0.95" FallbackColor="#FAFBFE" />
+                    <SolidColorBrush x:Key="ScoopGlassBorderBrush" Color="#B3FFFFFF" />
+                    <SolidColorBrush x:Key="ScoopTextPrimaryBrush" Color="#111827" />
+                    <SolidColorBrush x:Key="ScoopTextSecondaryBrush" Color="#6B7280" />
+                    <SolidColorBrush x:Key="ScoopTextTertiaryBrush" Color="#9CA3AF" />
+                    <SolidColorBrush x:Key="ScoopTextBodyBrush" Color="#374151" />
+                    <SolidColorBrush x:Key="ScoopSurfaceFillBrush" Color="#F7F7F7" />
+                    <SolidColorBrush x:Key="ScoopCardFillBrush" Color="#FFFFFF" />
+                    <SolidColorBrush x:Key="ScoopGaugeTrackBrush" Color="#E5E7EB" />
+                    <SolidColorBrush x:Key="ScoopNavInactiveBrush" Color="#4B5563" />
+                    <SolidColorBrush x:Key="SoftInputFillBrush" Color="#F2F3F5" />
+                    <SolidColorBrush x:Key="ScoopCaptionHoverBrush" Color="#14000000" />
+                    <SolidColorBrush x:Key="ScoopCaptionPressedBrush" Color="#26000000" />
+                    <RadialGradientBrush x:Key="ScoopOrbBlueBrush">
+                        <GradientStop Color="#B3BFDBFE" Offset="0" />
+                        <GradientStop Color="#00BFDBFE" Offset="1" />
+                    </RadialGradientBrush>
+                    <RadialGradientBrush x:Key="ScoopOrbPinkBrush">
+                        <GradientStop Color="#A6FBCFE8" Offset="0" />
+                        <GradientStop Color="#00FBCFE8" Offset="1" />
+                    </RadialGradientBrush>
+                    <RadialGradientBrush x:Key="ScoopOrbVioletBrush">
+                        <GradientStop Color="#99DDD6FE" Offset="0" />
+                        <GradientStop Color="#00DDD6FE" Offset="1" />
+                    </RadialGradientBrush>
                 </ResourceDictionary>
                 <ResourceDictionary x:Key="Dark">
                     <Color x:Key="SystemAccentColor">#3B82F6</Color>
                     <Color x:Key="SystemAccentColorLight1">#60A5FA</Color>
                     <Color x:Key="SystemAccentColorLight2">#93C5FD</Color>
@@ -41,43 +73,52 @@
                     <SolidColorBrush x:Key="AccentFillColorSecondaryBrush" Color="#CC3B82F6" />
                     <SolidColorBrush x:Key="AccentFillColorTertiaryBrush" Color="#993B82F6" />
                     <SolidColorBrush x:Key="AccentTextFillColorPrimaryBrush" Color="#3B82F6" />
                     <SolidColorBrush x:Key="AccentTextFillColorSecondaryBrush" Color="#CC3B82F6" />
                     <SolidColorBrush x:Key="AccentTextFillColorTertiaryBrush" Color="#993B82F6" />
+                    <LinearGradientBrush x:Key="ScoopAmbientGradient" StartPoint="0,0" EndPoint="1,1">
+                        <GradientStop Color="#0B1220" Offset="0" />
+                        <GradientStop Color="#15101F" Offset="0.38" />
+                        <GradientStop Color="#1A1218" Offset="0.72" />
+                        <GradientStop Color="#0F172A" Offset="1" />
+                    </LinearGradientBrush>
+                    <AcrylicBrush x:Key="ScoopGlassAcrylicBrush" TintColor="#1C1C1E" TintOpacity="0.72" TintLuminosityOpacity="0.85" FallbackColor="#1C1C1E" />
+                    <AcrylicBrush x:Key="ScoopGlassStrongAcrylicBrush" TintColor="#2A2A2E" TintOpacity="0.82" TintLuminosityOpacity="0.9" FallbackColor="#2A2A2E" />
+                    <SolidColorBrush x:Key="ScoopGlassBorderBrush" Color="#33FFFFFF" />
+                    <SolidColorBrush x:Key="ScoopTextPrimaryBrush" Color="#F3F4F6" />
+                    <SolidColorBrush x:Key="ScoopTextSecondaryBrush" Color="#9CA3AF" />
+                    <SolidColorBrush x:Key="ScoopTextTertiaryBrush" Color="#6B7280" />
+                    <SolidColorBrush x:Key="ScoopTextBodyBrush" Color="#D1D5DB" />
+                    <SolidColorBrush x:Key="ScoopSurfaceFillBrush" Color="#252528" />
+                    <SolidColorBrush x:Key="ScoopCardFillBrush" Color="#2A2A2E" />
+                    <SolidColorBrush x:Key="ScoopGaugeTrackBrush" Color="#3F3F46" />
+                    <SolidColorBrush x:Key="ScoopNavInactiveBrush" Color="#9CA3AF" />
+                    <SolidColorBrush x:Key="SoftInputFillBrush" Color="#3A3A3E" />
+                    <SolidColorBrush x:Key="ScoopCaptionHoverBrush" Color="#14FFFFFF" />
+                    <SolidColorBrush x:Key="ScoopCaptionPressedBrush" Color="#26FFFFFF" />
+                    <RadialGradientBrush x:Key="ScoopOrbBlueBrush">
+                        <GradientStop Color="#59BFDBFE" Offset="0" />
+                        <GradientStop Color="#00BFDBFE" Offset="1" />
+                    </RadialGradientBrush>
+                    <RadialGradientBrush x:Key="ScoopOrbPinkBrush">
+                        <GradientStop Color="#53FBCFE8" Offset="0" />
+                        <GradientStop Color="#00FBCFE8" Offset="1" />
+                    </RadialGradientBrush>
+                    <RadialGradientBrush x:Key="ScoopOrbVioletBrush">
+                        <GradientStop Color="#4DDDD6FE" Offset="0" />
+                        <GradientStop Color="#00DDD6FE" Offset="1" />
+                    </RadialGradientBrush>
                 </ResourceDictionary>
             </ResourceDictionary.ThemeDictionaries>
 
             <!-- 鍏ㄥ眬涓昏壊 = 渚ф爮閫変腑鑹?#2563EB -->
             <Color x:Key="ScoopAccentColor">#2563EB</Color>
             <SolidColorBrush x:Key="ScoopAccentBrush" Color="{StaticResource ScoopAccentColor}" />
 
-            <!-- Logo 钃濈传绮?鈫?娴呰壊姘涘洿娓愬彉锛圱oolbox 寮忥級 -->
-            <LinearGradientBrush x:Key="ScoopAmbientGradient" StartPoint="0,0" EndPoint="1,1">
-                <GradientStop Color="#EEF2FF" Offset="0" />
-                <GradientStop Color="#F5F3FF" Offset="0.38" />
-                <GradientStop Color="#FDF2F8" Offset="0.72" />
-                <GradientStop Color="#EFF6FF" Offset="1" />
-            </LinearGradientBrush>
-
-            <!-- Apple 寮忔瘺鐜荤拑锛氬簲鐢ㄥ唴 Acrylic 妯＄硦鑳屽悗娓愬彉 -->
-            <AcrylicBrush
-                x:Key="ScoopGlassAcrylicBrush"
-                TintColor="#FFFFFF"
-                TintOpacity="0.42"
-                TintLuminosityOpacity="0.92"
-                FallbackColor="#F4F6FB" />
-            <AcrylicBrush
-                x:Key="ScoopGlassStrongAcrylicBrush"
-                TintColor="#FFFFFF"
-                TintOpacity="0.62"
-                TintLuminosityOpacity="0.95"
-                FallbackColor="#FAFBFE" />
-            <SolidColorBrush x:Key="ScoopGlassBorderBrush" Color="#B3FFFFFF" />
-
             <Style x:Key="ScoopGlassCardStyle" TargetType="Border">
-                <Setter Property="Background" Value="{StaticResource ScoopGlassAcrylicBrush}" />
-                <Setter Property="BorderBrush" Value="{StaticResource ScoopGlassBorderBrush}" />
+                <Setter Property="Background" Value="{ThemeResource ScoopGlassAcrylicBrush}" />
+                <Setter Property="BorderBrush" Value="{ThemeResource ScoopGlassBorderBrush}" />
                 <Setter Property="BorderThickness" Value="1" />
                 <Setter Property="CornerRadius" Value="16" />
             </Style>
 
             <!-- 琛ㄦ牸鍐呴摼鎺ワ細鏃犵伆搴曢€変腑/鎮仠鍧?-->
@@ -90,15 +131,17 @@
                 <Setter Property="CornerRadius" Value="0" />
                 <Setter Property="UseSystemFocusVisuals" Value="False" />
                 <Setter Property="HorizontalAlignment" Value="Stretch" />
                 <Setter Property="HorizontalContentAlignment" Value="Left" />
                 <Setter Property="VerticalAlignment" Value="Center" />
+                <Setter Property="Foreground" Value="{ThemeResource AccentTextFillColorPrimaryBrush}" />
                 <Setter Property="Template">
                     <Setter.Value>
                         <ControlTemplate TargetType="HyperlinkButton">
                             <ContentPresenter
                                 x:Name="ContentPresenter"
+                                Foreground="{TemplateBinding Foreground}"
                                 Background="Transparent"
                                 BorderThickness="0"
                                 Content="{TemplateBinding Content}"
                                 ContentTemplate="{TemplateBinding ContentTemplate}"
                                 ContentTransitions="{TemplateBinding ContentTransitions}"
@@ -111,25 +154,23 @@
             </Style>
 
             <Style x:Key="DialogFieldLabelStyle" TargetType="TextBlock">
                 <Setter Property="FontSize" Value="14" />
                 <Setter Property="FontWeight" Value="SemiBold" />
-                <Setter Property="Foreground" Value="#1F2937" />
+                <Setter Property="Foreground" Value="{ThemeResource ScoopTextPrimaryBrush}" />
             </Style>
 
             <Style x:Key="DialogFieldHintStyle" TargetType="TextBlock">
                 <Setter Property="FontSize" Value="12" />
-                <Setter Property="Foreground" Value="#9CA3AF" />
+                <Setter Property="Foreground" Value="{ThemeResource ScoopTextTertiaryBrush}" />
                 <Setter Property="TextWrapping" Value="Wrap" />
             </Style>
 
-            <SolidColorBrush x:Key="SoftInputFillBrush" Color="#F2F3F5" />
-
             <Style x:Key="SoftCancelButtonStyle" TargetType="Button" BasedOn="{StaticResource DefaultButtonStyle}">
-                <Setter Property="Background" Value="{StaticResource SoftInputFillBrush}" />
+                <Setter Property="Background" Value="{ThemeResource SoftInputFillBrush}" />
                 <Setter Property="BorderThickness" Value="0" />
-                <Setter Property="Foreground" Value="#374151" />
+                <Setter Property="Foreground" Value="{ThemeResource ScoopTextBodyBrush}" />
                 <Setter Property="CornerRadius" Value="6" />
                 <Setter Property="MinHeight" Value="36" />
                 <Setter Property="Padding" Value="20,8" />
             </Style>
 
@@ -193,16 +234,16 @@
                                 <VisualStateManager.VisualStateGroups>
                                     <VisualStateGroup x:Name="CommonStates">
                                         <VisualState x:Name="Normal" />
                                         <VisualState x:Name="PointerOver">
                                             <VisualState.Setters>
-                                                <Setter Target="Root.Background" Value="#14000000" />
+                                                <Setter Target="Root.Background" Value="{ThemeResource ScoopCaptionHoverBrush}" />
                                             </VisualState.Setters>
                                         </VisualState>
                                         <VisualState x:Name="Pressed">
                                             <VisualState.Setters>
-                                                <Setter Target="Root.Background" Value="#26000000" />
+                                                <Setter Target="Root.Background" Value="{ThemeResource ScoopCaptionPressedBrush}" />
                                             </VisualState.Setters>
                                         </VisualState>
                                         <VisualState x:Name="Disabled" />
                                     </VisualStateGroup>
                                 </VisualStateManager.VisualStateGroups>
diff --git a/App.xaml.cs b/App.xaml.cs
index 3b29fcc..aa0a4d7 100644
--- a/App.xaml.cs
+++ b/App.xaml.cs
@@ -1,6 +1,7 @@
 锘縰sing Microsoft.UI.Xaml;
+using ScoopX.Services;
 
 namespace ScoopX
 {
     public partial class App : Application
     {
@@ -9,15 +10,18 @@ namespace ScoopX
         public static Window? MainWindow { get; private set; }
 
         public App()
         {
             InitializeComponent();
+            _ = ThemeService.Instance;
         }
 
         protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
         {
+            ThemeService.Instance.ApplyApplicationThemeOnly();
             _window = new MainWindow();
             MainWindow = _window;
+            ThemeService.Instance.Initialize(_window.DispatcherQueue);
             _window.Activate();
         }
     }
 }
diff --git a/Controls/StatusGauge.xaml b/Controls/StatusGauge.xaml
index 2c94b66..cbb7785 100644
--- a/Controls/StatusGauge.xaml
+++ b/Controls/StatusGauge.xaml
@@ -16,11 +16,11 @@
             Height="108"
             HorizontalAlignment="Center"
             SizeChanged="RingHost_SizeChanged">
             <Ellipse
                 x:Name="TrackEllipse"
-                Stroke="#E5E7EB"
+                Stroke="{ThemeResource ScoopGaugeTrackBrush}"
                 StrokeThickness="7"
                 Fill="Transparent" />
             <Path
                 x:Name="ProgressPath"
                 Stroke="{StaticResource ScoopAccentBrush}"
@@ -41,27 +41,27 @@
         <StackPanel Spacing="2" HorizontalAlignment="Center">
             <TextBlock
                 x:Name="DetailText"
                 Text="鈥?
                 FontSize="14"
-                Foreground="#374151"
+                Foreground="{ThemeResource ScoopTextBodyBrush}"
                 HorizontalAlignment="Center" />
             <StackPanel
                 x:Name="UsagePanel"
                 Orientation="Horizontal"
                 HorizontalAlignment="Center"
                 Visibility="Collapsed"
                 Spacing="0">
                 <TextBlock x:Name="UsedText" FontSize="14" FontWeight="SemiBold" />
-                <TextBlock Text=" / " FontSize="14" Foreground="#6B7280" />
-                <TextBlock x:Name="TotalText" FontSize="14" Foreground="#6B7280" />
+                <TextBlock Text=" / " FontSize="14" Foreground="{ThemeResource ScoopTextSecondaryBrush}" />
+                <TextBlock x:Name="TotalText" FontSize="14" Foreground="{ThemeResource ScoopTextSecondaryBrush}" />
             </StackPanel>
             <TextBlock
                 x:Name="LabelText"
                 Text=""
                 FontSize="13"
-                Foreground="#9CA3AF"
+                Foreground="{ThemeResource ScoopTextTertiaryBrush}"
                 HorizontalAlignment="Center"
                 Margin="0,2,0,0" />
         </StackPanel>
     </StackPanel>
 </UserControl>
diff --git a/Controls/StatusGauge.xaml.cs b/Controls/StatusGauge.xaml.cs
index fc94ae7..4e89135 100644
--- a/Controls/StatusGauge.xaml.cs
+++ b/Controls/StatusGauge.xaml.cs
@@ -16,12 +16,10 @@ namespace ScoopX.Controls
         private const double AnimSnapEpsilon = 0.08;
 
         private static readonly Color Accent = Color.FromArgb(255, 37, 99, 235);    // #2563EB 姝ｅ父
         private static readonly Color Caution = Color.FromArgb(255, 249, 115, 22);  // #F97316 鍋忛珮璀︾ず
         private static readonly Color Critical = Color.FromArgb(255, 244, 63, 94);  // #F43F5E 瓒呰繃90%
-        private static readonly Color DetailDark = Color.FromArgb(255, 55, 65, 81);
-
         private double _displayPercent;
         private double _targetPercent;
         private bool _animating;
         private readonly DispatcherQueueTimer _animTimer;
 
@@ -38,11 +36,10 @@ namespace ScoopX.Controls
         public void SetSimple(double percent, string detail, string label)
         {
             DetailText.Visibility = Visibility.Visible;
             UsagePanel.Visibility = Visibility.Collapsed;
             DetailText.Text = detail;
-            DetailText.Foreground = new SolidColorBrush(DetailDark);
             LabelText.Text = label;
             AnimateTo(percent);
         }
 
         public void SetUsage(double percent, string used, string total, string label)
diff --git a/MainWindow.xaml b/MainWindow.xaml
index 2d735c4..9deec27 100644
--- a/MainWindow.xaml
+++ b/MainWindow.xaml
@@ -14,57 +14,39 @@
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
@@ -123,15 +105,15 @@
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
@@ -217,36 +199,36 @@
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
@@ -255,24 +237,24 @@
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
diff --git a/MainWindow.xaml.cs b/MainWindow.xaml.cs
index 589187d..ce26f8d 100644
--- a/MainWindow.xaml.cs
+++ b/MainWindow.xaml.cs
@@ -7,20 +7,18 @@ using Microsoft.UI.Xaml.Controls;
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
@@ -60,14 +58,47 @@ namespace ScoopX
 
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
@@ -190,10 +221,13 @@ namespace ScoopX
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
@@ -314,11 +348,11 @@ namespace ScoopX
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
diff --git a/Pages/AddSiteDialog.xaml b/Pages/AddSiteDialog.xaml
index 3bb4973..2d38f82 100644
--- a/Pages/AddSiteDialog.xaml
+++ b/Pages/AddSiteDialog.xaml
@@ -20,24 +20,21 @@
         <DataTemplate>
             <TextBlock
                 Text="{Binding}"
                 FontSize="17"
                 FontWeight="SemiBold"
-                Foreground="#111827" />
+                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
         </DataTemplate>
     </ContentDialog.TitleTemplate>
 
     <ContentDialog.Resources>
         <Thickness x:Key="ContentDialogPadding">28,22,28,20</Thickness>
         <Thickness x:Key="ContentDialogTitleMargin">0,0,0,16</Thickness>
         <Thickness x:Key="ContentDialogSeparatorThickness">0</Thickness>
         <Thickness x:Key="ContentDialogCommandSpaceMargin">0,20,0,0</Thickness>
         <x:Double x:Key="ContentDialogMinWidth">480</x:Double>
         <x:Double x:Key="ContentDialogMaxWidth">520</x:Double>
-        <SolidColorBrush x:Key="ContentDialogBackground" Color="#FFFFFF" />
-        <SolidColorBrush x:Key="ContentDialogTopOverlay" Color="Transparent" />
-        <SolidColorBrush x:Key="ContentDialogBorderBrush" Color="#0F000000" />
         <Thickness x:Key="ContentDialogBorderThickness">0</Thickness>
         <SolidColorBrush x:Key="ContentDialogSmokeFill" Color="#99000000" />
     </ContentDialog.Resources>
 
     <ScrollViewer
diff --git a/Pages/EditSiteDialog.xaml b/Pages/EditSiteDialog.xaml
index 61ad6cd..bc3709d 100644
--- a/Pages/EditSiteDialog.xaml
+++ b/Pages/EditSiteDialog.xaml
@@ -14,24 +14,21 @@
         <DataTemplate>
             <TextBlock
                 Text="{Binding}"
                 FontSize="18"
                 FontWeight="SemiBold"
-                Foreground="#1F2937" />
+                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
         </DataTemplate>
     </ContentDialog.TitleTemplate>
 
     <ContentDialog.Resources>
         <Thickness x:Key="ContentDialogPadding">0,12,0,0</Thickness>
         <Thickness x:Key="ContentDialogTitleMargin">24,4,24,4</Thickness>
         <Thickness x:Key="ContentDialogSeparatorThickness">0</Thickness>
         <Thickness x:Key="ContentDialogCommandSpaceMargin">0</Thickness>
         <x:Double x:Key="ContentDialogMinWidth">820</x:Double>
         <x:Double x:Key="ContentDialogMaxWidth">900</x:Double>
-        <SolidColorBrush x:Key="ContentDialogBackground" Color="#FFFFFF" />
-        <SolidColorBrush x:Key="ContentDialogTopOverlay" Color="Transparent" />
-        <SolidColorBrush x:Key="ContentDialogBorderBrush" Color="#0F000000" />
         <Thickness x:Key="ContentDialogBorderThickness">0</Thickness>
         <SolidColorBrush x:Key="ContentDialogSmokeFill" Color="#99000000" />
 
         <Style x:Key="EditNavButtonStyle" TargetType="Button">
             <Setter Property="Background" Value="Transparent" />
@@ -104,17 +101,17 @@
         </Style>
 
         <Style x:Key="EditSectionTitleStyle" TargetType="TextBlock">
             <Setter Property="FontSize" Value="18" />
             <Setter Property="FontWeight" Value="SemiBold" />
-            <Setter Property="Foreground" Value="#111827" />
+            <Setter Property="Foreground" Value="{ThemeResource ScoopTextPrimaryBrush}" />
             <Setter Property="Margin" Value="0,0,0,18" />
         </Style>
 
         <Style x:Key="EditGroupLabelStyle" TargetType="TextBlock">
             <Setter Property="FontSize" Value="13" />
-            <Setter Property="Foreground" Value="#9CA3AF" />
+            <Setter Property="Foreground" Value="{ThemeResource ScoopTextTertiaryBrush}" />
             <Setter Property="Margin" Value="0,0,0,10" />
         </Style>
     </ContentDialog.Resources>
 
     <Grid Width="852">
diff --git a/Pages/HomePage.xaml b/Pages/HomePage.xaml
index c8a9846..489c166 100644
--- a/Pages/HomePage.xaml
+++ b/Pages/HomePage.xaml
@@ -17,11 +17,11 @@
             <!-- 鐘舵€?-->
             <TextBlock
                 Text="鐘舵€?
                 FontSize="18"
                 FontWeight="SemiBold"
-                Foreground="{ThemeResource TextFillColorPrimaryBrush}" />
+                Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
 
             <Grid Margin="0,12,0,4">
                 <Grid.ColumnDefinitions>
                     <ColumnDefinition Width="*" />
                     <ColumnDefinition Width="*" />
@@ -36,11 +36,11 @@
             </Grid>
 
             <!-- 纾佺洏 IO 鐩戞帶 -->
             <Border
                 Margin="0,16,0,0"
-                Background="#F7F7F7"
+                Background="{ThemeResource ScoopSurfaceFillBrush}"
                 CornerRadius="12"
                 Padding="16,14">
                 <Grid RowSpacing="14">
                     <Grid.RowDefinitions>
                         <RowDefinition Height="Auto" />
@@ -49,20 +49,20 @@
                     </Grid.RowDefinitions>
 
                     <!-- 椤舵爮锛氱鐩業O + 纾佺洏绛涢€?-->
                     <Grid>
                         <StackPanel Spacing="6" VerticalAlignment="Center">
-                            <TextBlock Text="纾佺洏IO" FontSize="14" FontWeight="SemiBold" Foreground="#111827" />
+                            <TextBlock Text="纾佺洏IO" FontSize="14" FontWeight="SemiBold" Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
                             <Border Width="42" Height="2" CornerRadius="1" HorizontalAlignment="Left" Background="{StaticResource ScoopAccentBrush}" />
                         </StackPanel>
 
                         <StackPanel
                             Orientation="Horizontal"
                             HorizontalAlignment="Right"
                             VerticalAlignment="Center"
                             Spacing="8">
-                            <TextBlock Text="纾佺洏" FontSize="13" Foreground="#6B7280" VerticalAlignment="Center" />
+                            <TextBlock Text="纾佺洏" FontSize="13" Foreground="{ThemeResource ScoopTextSecondaryBrush}" VerticalAlignment="Center" />
                             <ComboBox
                                 x:Name="DiskFilterComboBox"
                                 MinWidth="88"
                                 CornerRadius="6"
                                 SelectedIndex="0" />
@@ -76,40 +76,40 @@
                             <ColumnDefinition Width="*" />
                             <ColumnDefinition Width="*" />
                             <ColumnDefinition Width="*" />
                         </Grid.ColumnDefinitions>
 
-                        <Border Grid.Column="0" Background="#FFFFFF" CornerRadius="8" Padding="12,10">
+                        <Border Grid.Column="0" Background="{ThemeResource ScoopCardFillBrush}" CornerRadius="8" Padding="12,10">
                             <StackPanel Orientation="Horizontal" Spacing="8" VerticalAlignment="Center">
                                 <Ellipse Width="8" Height="8" Fill="#F472B6" VerticalAlignment="Center" />
-                                <TextBlock FontSize="13" Foreground="#374151" VerticalAlignment="Center">
+                                <TextBlock FontSize="13" Foreground="{ThemeResource ScoopTextBodyBrush}" VerticalAlignment="Center">
                                     <Run Text="璇诲彇 " />
                                     <Run x:Name="DiskReadText" Text="0 B" FontWeight="SemiBold" />
                                 </TextBlock>
                             </StackPanel>
                         </Border>
 
-                        <Border Grid.Column="1" Background="#FFFFFF" CornerRadius="8" Padding="12,10">
+                        <Border Grid.Column="1" Background="{ThemeResource ScoopCardFillBrush}" CornerRadius="8" Padding="12,10">
                             <StackPanel Orientation="Horizontal" Spacing="8" VerticalAlignment="Center">
                                 <Ellipse Width="8" Height="8" Fill="#60A5FA" VerticalAlignment="Center" />
-                                <TextBlock FontSize="13" Foreground="#374151" VerticalAlignment="Center">
+                                <TextBlock FontSize="13" Foreground="{ThemeResource ScoopTextBodyBrush}" VerticalAlignment="Center">
                                     <Run Text="鍐欏叆 " />
                                     <Run x:Name="DiskWriteText" Text="0 B" FontWeight="SemiBold" />
                                 </TextBlock>
                             </StackPanel>
                         </Border>
 
-                        <Border Grid.Column="2" Background="#FFFFFF" CornerRadius="8" Padding="12,10">
-                            <TextBlock FontSize="13" Foreground="#374151" VerticalAlignment="Center">
+                        <Border Grid.Column="2" Background="{ThemeResource ScoopCardFillBrush}" CornerRadius="8" Padding="12,10">
+                            <TextBlock FontSize="13" Foreground="{ThemeResource ScoopTextBodyBrush}" VerticalAlignment="Center">
                                 <Run Text="姣忕璇诲啓 " />
                                 <Run x:Name="DiskOpsText" Text="0" FontWeight="SemiBold" />
                                 <Run Text=" 娆? />
                             </TextBlock>
                         </Border>
 
-                        <Border Grid.Column="3" Background="#FFFFFF" CornerRadius="8" Padding="12,10">
-                            <TextBlock FontSize="13" Foreground="#374151" VerticalAlignment="Center">
+                        <Border Grid.Column="3" Background="{ThemeResource ScoopCardFillBrush}" CornerRadius="8" Padding="12,10">
+                            <TextBlock FontSize="13" Foreground="{ThemeResource ScoopTextBodyBrush}" VerticalAlignment="Center">
                                 <Run Text="IO寤惰繜 " />
                                 <Run x:Name="DiskLatencyText" Text="0 ms" FontWeight="SemiBold" />
                             </TextBlock>
                         </Border>
                     </Grid>
@@ -128,11 +128,11 @@
                         <Canvas x:Name="ChartYLabels" Grid.Row="0" />
 
                         <Border
                             Grid.Column="1"
                             Grid.Row="0"
-                            Background="#FFFFFF"
+                            Background="{ThemeResource ScoopCardFillBrush}"
                             CornerRadius="8"
                             Padding="4">
                             <Canvas
                                 x:Name="ChartCanvas"
                                 SizeChanged="ChartCanvas_SizeChanged" />
diff --git a/Pages/WebsitesPage.xaml b/Pages/WebsitesPage.xaml
index 8268c85..80e88e9 100644
--- a/Pages/WebsitesPage.xaml
+++ b/Pages/WebsitesPage.xaml
@@ -68,11 +68,11 @@
             <Border
                 Grid.Row="1"
                 VerticalAlignment="Top"
                 Style="{StaticResource ScoopGlassCardStyle}"
                 CornerRadius="14"
-                Background="{StaticResource ScoopGlassStrongAcrylicBrush}">
+                Background="{ThemeResource ScoopGlassStrongAcrylicBrush}">
                 <Grid>
                     <Grid.RowDefinitions>
                         <RowDefinition Height="Auto" />
                         <RowDefinition Height="Auto" />
                         <RowDefinition Height="Auto" />
@@ -132,15 +132,17 @@
                                         Checked="ItemCheckBox_Changed"
                                         Unchecked="ItemCheckBox_Changed" />
                                     <HyperlinkButton
                                         Grid.Column="1"
                                         Style="{StaticResource TableLinkButtonStyle}"
+                                        Foreground="{ThemeResource AccentTextFillColorPrimaryBrush}"
                                         Tag="{x:Bind}"
                                         Click="SiteName_Click"
                                         ToolTipService.ToolTip="鐐瑰嚮缂栬緫绔欑偣">
                                         <TextBlock
                                             Text="{x:Bind Name}"
+                                            Foreground="{ThemeResource AccentTextFillColorPrimaryBrush}"
                                             TextWrapping="NoWrap"
                                             TextTrimming="CharacterEllipsis" />
                                     </HyperlinkButton>
                                     <ToggleSwitch
                                         Grid.Column="2"
@@ -153,32 +155,38 @@
                                         Toggled="StatusToggle_Toggled"
                                         ToolTipService.ToolTip="鐐瑰嚮鍚仠绔欑偣" />
                                     <HyperlinkButton
                                         Grid.Column="3"
                                         Style="{StaticResource TableLinkButtonStyle}"
+                                        Foreground="{ThemeResource ScoopTextSecondaryBrush}"
                                         Tag="{x:Bind RootPath}"
                                         Click="RootPath_Click"
                                         ToolTipService.ToolTip="{x:Bind RootPath}">
                                         <TextBlock
                                             Text="{x:Bind RootPath}"
+                                            Foreground="{ThemeResource ScoopTextSecondaryBrush}"
                                             TextWrapping="NoWrap"
                                             TextTrimming="CharacterEllipsis" />
                                     </HyperlinkButton>
-                                    <TextBlock Grid.Column="4" Text="{x:Bind Remark}" VerticalAlignment="Center" TextWrapping="NoWrap" TextTrimming="CharacterEllipsis" />
-                                    <TextBlock Grid.Column="5" Text="{x:Bind PhpVersion}" VerticalAlignment="Center" HorizontalAlignment="Center" TextAlignment="Center" />
+                                    <TextBlock Grid.Column="4" Text="{x:Bind Remark}" Foreground="{ThemeResource ScoopTextPrimaryBrush}" VerticalAlignment="Center" TextWrapping="NoWrap" TextTrimming="CharacterEllipsis" />
+                                    <TextBlock Grid.Column="5" Text="{x:Bind PhpVersion}" Foreground="{ThemeResource ScoopTextPrimaryBrush}" VerticalAlignment="Center" HorizontalAlignment="Center" TextAlignment="Center" />
                                     <StackPanel Grid.Column="6" Orientation="Horizontal" Spacing="8" VerticalAlignment="Center">
                                         <HyperlinkButton
                                             Content="璁剧疆"
-                                            Padding="0"
+                                            Style="{StaticResource TableLinkButtonStyle}"
+                                            Foreground="{ThemeResource AccentTextFillColorPrimaryBrush}"
                                             Tag="{x:Bind}"
                                             Click="SiteName_Click" />
                                         <Border
                                             Width="1"
                                             Height="12"
                                             VerticalAlignment="Center"
                                             Background="{ThemeResource DividerStrokeColorDefaultBrush}" />
-                                        <HyperlinkButton Content="鍒犻櫎" Padding="0" />
+                                        <HyperlinkButton
+                                            Content="鍒犻櫎"
+                                            Style="{StaticResource TableLinkButtonStyle}"
+                                            Foreground="{ThemeResource AccentTextFillColorPrimaryBrush}" />
                                     </StackPanel>
                                 </Grid>
                             </DataTemplate>
                         </ItemsControl.ItemTemplate>
                     </ItemsControl>

## NEW FILE: Services/AppThemePreference.cs

namespace ScoopX.Services
{
    public enum AppThemePreference
    {
        System,
        Light,
        Dark
    }
}

## NEW FILE: Services/ThemeService.cs

using System;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace ScoopX.Services
{
    public sealed class ThemeService
    {
        public const string SettingsKey = "AppTheme";
        private static readonly Lazy<ThemeService> Lazy = new(() => new ThemeService());
        public static ThemeService Instance => Lazy.Value;

        private readonly UISettings _uiSettings = new();
        private DispatcherQueue? _dispatcherQueue;
        private bool _listening;

        public AppThemePreference Preference { get; private set; } = AppThemePreference.System;

        public event EventHandler? ThemeChanged;

        private ThemeService() { }

        public void Initialize(DispatcherQueue dispatcherQueue)
        {
            _dispatcherQueue = dispatcherQueue;
            Preference = ReadPreference();
            Apply();
            EnsureSystemListener();
        }

        public void SetPreference(AppThemePreference preference)
        {
            Preference = preference;
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = preference.ToString();
            Apply();
            EnsureSystemListener();
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ApplyApplicationThemeOnly()
        {
            var app = Application.Current;
            if (app is null)
            {
                return;
            }

            SetApplicationRequestedTheme(ReadPreference());
        }

        public void Apply()
        {
            var app = Application.Current;
            if (app is null)
            {
                return;
            }

            SetApplicationRequestedTheme(Preference);

            if (App.MainWindow?.Content is FrameworkElement root)
            {
                root.RequestedTheme = Preference switch
                {
                    AppThemePreference.Light => ElementTheme.Light,
                    AppThemePreference.Dark => ElementTheme.Dark,
                    _ => ElementTheme.Default
                };
            }
        }

        private static void SetApplicationRequestedTheme(AppThemePreference preference)
        {
            var app = Application.Current;
            if (app is null)
            {
                return;
            }

            app.RequestedTheme = preference switch
            {
                AppThemePreference.Light => ApplicationTheme.Light,
                AppThemePreference.Dark => ApplicationTheme.Dark,
                _ => ResolveSystemTheme()
            };
        }

        private static ApplicationTheme ResolveSystemTheme()
        {
            var color = new UISettings().GetColorValue(UIColorType.Background);
            var isDark = (color.R + color.G + color.B) < (128 * 3);
            return isDark ? ApplicationTheme.Dark : ApplicationTheme.Light;
        }

        private static AppThemePreference ReadPreference()
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(SettingsKey, out var value)
                && value is string s
                && Enum.TryParse<AppThemePreference>(s, ignoreCase: true, out var parsed))
            {
                return parsed;
            }

            return AppThemePreference.System;
        }

        private void EnsureSystemListener()
        {
            if (Preference == AppThemePreference.System)
            {
                if (_listening)
                {
                    return;
                }

                _uiSettings.ColorValuesChanged += OnColorValuesChanged;
                _listening = true;
            }
            else if (_listening)
            {
                _uiSettings.ColorValuesChanged -= OnColorValuesChanged;
                _listening = false;
            }
        }

        private void OnColorValuesChanged(UISettings sender, object args)
        {
            if (Preference != AppThemePreference.System)
            {
                return;
            }

            void ApplyOnUi()
            {
                Apply();
                ThemeChanged?.Invoke(this, EventArgs.Empty);
            }

            if (_dispatcherQueue is null || _dispatcherQueue.HasThreadAccess)
            {
                ApplyOnUi();
            }
            else
            {
                _ = _dispatcherQueue.TryEnqueue(ApplyOnUi);
            }
        }
    }
}

## NEW FILE: Pages/SettingsPage.xaml

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

## NEW FILE: Pages/SettingsPage.xaml.cs

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
