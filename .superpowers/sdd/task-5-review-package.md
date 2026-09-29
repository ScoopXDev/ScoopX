# Task 5

diff --git a/Pages/WebsitesPage.xaml b/Pages/WebsitesPage.xaml
index 8268c85..80e88e9 100644
--- a/Pages/WebsitesPage.xaml
+++ b/Pages/WebsitesPage.xaml
@@ -65,17 +65,17 @@
                 </StackPanel>
             </Grid>
 
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
                     </Grid.RowDefinitions>
 
                     <Grid
@@ -129,59 +129,67 @@
                                         MinWidth="0"
                                         VerticalAlignment="Center"
                                         IsChecked="{x:Bind IsSelected, Mode=TwoWay}"
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
                                         MinWidth="0"
                                         VerticalAlignment="Center"
                                         OnContent=""
                                         OffContent=""
                                         IsOn="{x:Bind IsRunning, Mode=TwoWay}"
                                         Tag="{x:Bind}"
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
 
                     <Grid
                         Grid.Row="2"
