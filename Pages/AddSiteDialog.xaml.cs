using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace ScoopX.Pages
{
    public sealed partial class AddSiteDialog : ContentDialog
    {
        private UIElement? _smokeLayer;

        public IReadOnlyList<WebsiteItem> CreatedSites { get; private set; } = Array.Empty<WebsiteItem>();

        public AddSiteDialog()
        {
            InitializeComponent();
            Opened += AddSiteDialog_Opened;
            Closed += AddSiteDialog_Closed;
        }

        private void AddSiteDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            // 等一帧，确保模板视觉状态 DialogShowing 已应用
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, HookSmokeLayer);
        }

        private void HookSmokeLayer()
        {
            UnhookSmokeLayer();

            // 模板里的遮罩矩形（不是 BackgroundElement，那是弹窗本体）
            _smokeLayer = GetTemplateChild("SmokeLayerBackground") as UIElement
                ?? FindDescendantByName(this, "SmokeLayerBackground");

            if (_smokeLayer is null)
            {
                return;
            }

            _smokeLayer.PointerPressed += SmokeLayer_PointerPressed;
            _smokeLayer.Tapped += SmokeLayer_Tapped;
        }

        private void AddSiteDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
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

        private void DatabaseOption_Changed(object sender, RoutedEventArgs e)
        {
            if (MysqlCredentialsPanel == null)
            {
                return;
            }

            MysqlCredentialsPanel.Visibility = DbMysqlRadio.IsChecked == true
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private async void BrowseRootButton_Click(object sender, RoutedEventArgs e)
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
            if (folder != null)
            {
                RootPathTextBox.Text = folder.Path;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var domains = (DomainsTextBox.Text ?? string.Empty)
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(domain => !string.IsNullOrWhiteSpace(domain))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (domains.Count == 0)
            {
                args.Cancel = true;
                DomainsTextBox.Focus(FocusState.Programmatic);
                return;
            }

            var remark = RemarkTextBox.Text?.Trim() ?? string.Empty;
            var phpVersion = (PhpVersionComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "8.2";
            var rootInput = string.IsNullOrWhiteSpace(RootPathTextBox.Text)
                ? @"C:\ScoopX\wwwroot"
                : RootPathTextBox.Text.Trim().TrimEnd('\\', '/');

            CreatedSites = domains.Select(domain => new WebsiteItem
            {
                Name = domain,
                Status = "运行中",
                RootPath = Path.Combine(rootInput, domain),
                RunPath = Path.Combine(rootInput, domain),
                Remark = remark,
                PhpVersion = phpVersion,
            }).ToList();
        }
    }
}
