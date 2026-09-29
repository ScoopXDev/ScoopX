# Task 4 review

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
