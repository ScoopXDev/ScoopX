using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace ScoopX.Pages
{
    public sealed partial class EditSiteDialog : ContentDialog
    {
        private readonly WebsiteItem _site;
        private UIElement? _smokeLayer;
        private string _currentNav = "Domains";

        private static readonly Dictionary<string, string> RewriteTemplates = new(StringComparer.OrdinalIgnoreCase)
        {
            ["thinkphp"] =
                "location / {\n" +
                "    if (!-e $request_filename) {\n" +
                "        rewrite ^(.*)$ /index.php?s=$1 last;\n" +
                "        break;\n" +
                "    }\n" +
                "}",
            ["laravel"] =
                "location / {\n" +
                "    try_files $uri $uri/ /index.php?$query_string;\n" +
                "}",
            ["wordpress"] =
                "location / {\n" +
                "    try_files $uri $uri/ /index.php?$args;\n" +
                "}",
        };

        public bool IsConfirmed { get; private set; }

        public EditSiteDialog(WebsiteItem site)
        {
            _site = site;
            InitializeComponent();
            Title = "编辑站点";
            LoadSite(site);
            Opened += EditSiteDialog_Opened;
            Closed += EditSiteDialog_Closed;
            Loaded += (_, _) => SelectNav("Domains");
        }

        private void LoadSite(WebsiteItem site)
        {
            DomainTextBox.Text = site.Name;
            RemarkTextBox.Text = site.Remark;
            RootPathTextBox.Text = site.RootPath;
            RunPathTextBox.Text = string.IsNullOrWhiteSpace(site.RunPath) ? site.RootPath : site.RunPath;
            StatusComboBox.SelectedIndex = site.Status == "已停止" ? 1 : 0;
            SelectPhpVersion(site.PhpVersion);
            ConfigTextBox.Text = BuildDefaultConfig(site);
            RefreshLogs();
        }

        private static string BuildDefaultConfig(WebsiteItem site)
        {
            var root = (string.IsNullOrWhiteSpace(site.RunPath) ? site.RootPath : site.RunPath).Replace('\\', '/');
            return
                "server {\n" +
                "    listen 80;\n" +
                $"    server_name {site.Name};\n" +
                $"    root {root};\n" +
                "    index index.php index.html index.htm;\n" +
                "\n" +
                "    location / {\n" +
                "        try_files $uri $uri/ /index.php?$query_string;\n" +
                "    }\n" +
                "\n" +
                "    location ~ \\.php$ {\n" +
                "        fastcgi_pass 127.0.0.1:9000;\n" +
                "        fastcgi_index index.php;\n" +
                "        include fastcgi_params;\n" +
                "        fastcgi_param SCRIPT_FILENAME $document_root$fastcgi_script_name;\n" +
                "    }\n" +
                "}";
        }

        private void SelectPhpVersion(string version)
        {
            foreach (var item in PhpVersionComboBox.Items.OfType<ComboBoxItem>())
            {
                if (string.Equals(item.Content?.ToString(), version, StringComparison.OrdinalIgnoreCase))
                {
                    PhpVersionComboBox.SelectedItem = item;
                    return;
                }
            }
        }

        private void SelectNav(string tag)
        {
            _currentNav = tag;

            PanelDomains.Visibility = tag == "Domains" ? Visibility.Visible : Visibility.Collapsed;
            PanelDirectory.Visibility = tag == "Directory" ? Visibility.Visible : Visibility.Collapsed;
            PanelRewrite.Visibility = tag == "Rewrite" ? Visibility.Visible : Visibility.Collapsed;
            PanelDefaultDoc.Visibility = tag == "DefaultDoc" ? Visibility.Visible : Visibility.Collapsed;
            PanelConfig.Visibility = tag == "Config" ? Visibility.Visible : Visibility.Collapsed;
            PanelPhp.Visibility = tag == "Php" ? Visibility.Visible : Visibility.Collapsed;
            PanelLogs.Visibility = tag == "Logs" ? Visibility.Visible : Visibility.Collapsed;

            var selectedBg = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xEF, 0xF6, 0xFF));
            var transparent = new SolidColorBrush(Windows.UI.Color.FromArgb(0x00, 0x00, 0x00, 0x00));
            var accent = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x25, 0x63, 0xEB));
            var normalFg = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x4B, 0x55, 0x63));

            foreach (var child in NavPanel.Children.OfType<Button>())
            {
                var selected = string.Equals(child.Tag?.ToString(), tag, StringComparison.Ordinal);
                child.Background = selected ? selectedBg : transparent;
                child.BorderBrush = selected ? accent : transparent;
                child.Foreground = selected ? accent : normalFg;

                if (child.Content is Panel contentPanel)
                {
                    foreach (var visual in contentPanel.Children)
                    {
                        if (visual is FontIcon icon)
                        {
                            icon.Foreground = selected ? accent : normalFg;
                        }
                        else if (visual is TextBlock text)
                        {
                            text.Foreground = selected ? accent : normalFg;
                            text.FontWeight = selected
                                ? Microsoft.UI.Text.FontWeights.SemiBold
                                : Microsoft.UI.Text.FontWeights.Normal;
                        }
                    }
                }
            }
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: string tag })
            {
                SelectNav(tag);
            }
        }

        private void RewriteTemplateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RewriteRulesTextBox is null || RewriteTemplateComboBox.SelectedItem is not ComboBoxItem item)
            {
                return;
            }

            var tag = item.Tag?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(tag) || tag == "custom")
            {
                if (string.IsNullOrEmpty(tag))
                {
                    RewriteRulesTextBox.Text = string.Empty;
                }

                return;
            }

            if (RewriteTemplates.TryGetValue(tag, out var rules))
            {
                RewriteRulesTextBox.Text = rules.Trim();
            }
        }

        private void LogTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LogsTextBlock is null)
            {
                return;
            }

            RefreshLogs();
        }

        private void RefreshLogsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshLogs();
        }

        private void RefreshLogs()
        {
            var type = (LogTypeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "access";
            var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var text = type == "error"
                ? $"[{now}] [error] FastCGI: server \"127.0.0.1:9000\" timed out\n" +
                  $"[{now}] [error] open() \"{_site.RootPath}\\favicon.ico\" failed (2: No such file or directory)\n" +
                  $"[{now}] [warn]  *1 upstream prematurely closed connection"
                : $"127.0.0.1 - - [{now}] \"GET / HTTP/1.1\" 200 3124 \"-\" \"Mozilla/5.0\"\n" +
                  $"127.0.0.1 - - [{now}] \"GET /assets/app.css HTTP/1.1\" 200 18442 \"-\" \"Mozilla/5.0\"\n" +
                  $"127.0.0.1 - - [{now}] \"POST /api/login HTTP/1.1\" 302 0 \"-\" \"Mozilla/5.0\"";

            LogsTextBlock.Text = text;
            if (LogsFullscreenTextBlock is not null)
            {
                LogsFullscreenTextBlock.Text = text;
            }

            LogFullscreenTitle.Text = type == "error" ? "错误日志" : "访问日志";
        }

        private void LogFullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            LogsFullscreenTextBlock.Text = LogsTextBlock.Text;
            LogFullscreenOverlay.Visibility = Visibility.Visible;
        }

        private void ExitLogFullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            LogFullscreenOverlay.Visibility = Visibility.Collapsed;
        }

        private async void BrowseRootButton_Click(object sender, RoutedEventArgs e)
        {
            var path = await PickFolderAsync();
            if (path is not null)
            {
                RootPathTextBox.Text = path;
                if (string.IsNullOrWhiteSpace(RunPathTextBox.Text))
                {
                    RunPathTextBox.Text = path;
                }
            }
        }

        private async void BrowseRunButton_Click(object sender, RoutedEventArgs e)
        {
            var path = await PickFolderAsync();
            if (path is not null)
            {
                RunPathTextBox.Text = path;
            }
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            Hide();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var domain = DomainTextBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(domain))
            {
                SelectNav("Domains");
                DomainTextBox.Focus(FocusState.Programmatic);
                return;
            }

            _site.Name = domain;
            _site.Remark = RemarkTextBox.Text?.Trim() ?? string.Empty;
            _site.RootPath = string.IsNullOrWhiteSpace(RootPathTextBox.Text)
                ? _site.RootPath
                : RootPathTextBox.Text.Trim().TrimEnd('\\', '/');
            _site.RunPath = string.IsNullOrWhiteSpace(RunPathTextBox.Text)
                ? _site.RootPath
                : RunPathTextBox.Text.Trim().TrimEnd('\\', '/');
            _site.Status = StatusComboBox.SelectedIndex == 1 ? "已停止" : "运行中";
            _site.PhpVersion = (PhpVersionComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? _site.PhpVersion;
            IsConfirmed = true;
            Hide();
        }

        private void EditSiteDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, HookSmokeLayer);
        }

        private void HookSmokeLayer()
        {
            UnhookSmokeLayer();

            _smokeLayer = GetTemplateChild("SmokeLayerBackground") as UIElement
                ?? FindDescendantByName(this, "SmokeLayerBackground");

            if (_smokeLayer is null)
            {
                return;
            }

            _smokeLayer.PointerPressed += SmokeLayer_PointerPressed;
            _smokeLayer.Tapped += SmokeLayer_Tapped;
        }

        private void EditSiteDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            UnhookSmokeLayer();
        }

        private void UnhookSmokeLayer()
        {
            if (_smokeLayer is null)
            {
                return;
            }

            _smokeLayer.PointerPressed -= SmokeLayer_PointerPressed;
            _smokeLayer.Tapped -= SmokeLayer_Tapped;
            _smokeLayer = null;
        }

        private void SmokeLayer_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            Hide();
        }

        private void SmokeLayer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            Hide();
        }

        private static FrameworkElement? FindDescendantByName(DependencyObject root, string name)
        {
            var count = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is FrameworkElement fe && fe.Name == name)
                {
                    return fe;
                }

                var nested = FindDescendantByName(child, name);
                if (nested is not null)
                {
                    return nested;
                }
            }

            return null;
        }
    }
}
