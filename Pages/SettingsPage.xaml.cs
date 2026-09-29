using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using ScoopX.Controls;
using ScoopX.Services;
using Windows.ApplicationModel;
using Microsoft.Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers;
using Windows.UI;
using WinRT.Interop;

namespace ScoopX.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private static readonly SolidColorBrush SelectedBorderBrush =
            new(Color.FromArgb(255, 37, 99, 235)); // #2563EB 与侧栏选中一致
        private static readonly SolidColorBrush TransparentBrush =
            new(Color.FromArgb(0, 0, 0, 0));

        private bool _suppressToggle;
        private bool _suppressLanguage;

        public SettingsPage()
        {
            InitializeComponent();
            UpdateSelectionChrome(ThemeService.Instance.Preference);
            ThemeService.Instance.ThemeChanged += OnThemeChanged;
            Unloaded += (_, _) => ThemeService.Instance.ThemeChanged -= OnThemeChanged;

            LoadAppSettings();
            LoadLanguageSelection();
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

        private void LoadLanguageSelection()
        {
            _suppressLanguage = true;
            try
            {
                if (LanguageService.Instance.CurrentTag == LanguageService.EnUs)
                {
                    LanguageEnRadio.IsChecked = true;
                }
                else
                {
                    LanguageZhRadio.IsChecked = true;
                }
            }
            finally
            {
                _suppressLanguage = false;
            }
        }

        private static string GetVersionString()
        {
            string versionDigits;
            try
            {
                var v = Package.Current.Id.Version;
                versionDigits = $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
            catch
            {
                var v = typeof(App).Assembly.GetName().Version;
                versionDigits = v is null ? "1.0.0" : $"{v.Major}.{v.Minor}.{v.Build}";
            }

            var loader = new ResourceLoader();
            var fmt = loader.GetString("SettingsVersionFormat");
            return string.Format(fmt, versionDigits);
        }

        private async void LanguageRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (_suppressLanguage)
            {
                return;
            }

            if (sender is not RadioButton { IsChecked: true, Tag: string tag })
            {
                return;
            }

            if (string.Equals(tag, LanguageService.Instance.CurrentTag, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var loader = new ResourceLoader();
            if (!LanguageService.Instance.SetLanguage(tag))
            {
                LoadLanguageSelection();
                var failedDialog = new ContentDialog
                {
                    Title = loader.GetString("SettingsLanguageApplyFailedTitle"),
                    Content = loader.GetString("SettingsLanguageApplyFailedMessage"),
                    CloseButtonText = loader.GetString("SettingsLanguageApplyFailedOk"),
                    XamlRoot = XamlRoot,
                    RequestedTheme = ThemeService.Instance.CurrentElementTheme,
                };
                await failedDialog.ShowAsync();
                return;
            }

            var dialog = new ContentDialog
            {
                Title = loader.GetString("SettingsLanguageRestartTitle"),
                Content = loader.GetString("SettingsLanguageRestartMessage"),
                PrimaryButtonText = loader.GetString("SettingsLanguageRestartNow"),
                CloseButtonText = loader.GetString("SettingsLanguageLater"),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot,
                RequestedTheme = ThemeService.Instance.CurrentElementTheme,
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            var path = Environment.ProcessPath;
            var started = false;
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                    started = true;
                }
                catch
                {
                    // Fall through to failure dialog.
                }
            }

            if (started)
            {
                if (App.MainWindow is MainWindow mw)
                {
                    mw.RequestExit();
                }
                return;
            }

            var failDialog = new ContentDialog
            {
                Title = loader.GetString("SettingsLanguageRestartFailedTitle"),
                Content = loader.GetString("SettingsLanguageRestartFailedMessage"),
                CloseButtonText = loader.GetString("SettingsLanguageRestartFailedOk"),
                XamlRoot = XamlRoot,
                RequestedTheme = ThemeService.Instance.CurrentElementTheme,
            };
            await failDialog.ShowAsync();
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

            var loader = new ResourceLoader();
            var dialog = new ContentDialog
            {
                Title = loader.GetString("SettingsStartupFailedTitle"),
                Content = error ?? string.Empty,
                CloseButtonText = loader.GetString("SettingsStartupFailedOk"),
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
