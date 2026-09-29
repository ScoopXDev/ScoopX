# Task 6

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
