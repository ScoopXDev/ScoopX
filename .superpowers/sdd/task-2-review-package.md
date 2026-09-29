# Review package — Task 2
BASE: Task1 working tree snapshot conceptually; review App.xaml only for this task

## Diff App.xaml
diff --git a/App.xaml b/App.xaml
index 2f075f6..5494829 100644
--- a/App.xaml
+++ b/App.xaml
@@ -25,10 +25,41 @@
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
@@ -41,43 +72,51 @@
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
@@ -90,15 +129,17 @@
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
@@ -111,25 +152,23 @@
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
 
@@ -193,16 +232,16 @@
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
 M App.xaml
