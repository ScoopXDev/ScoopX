using System;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using ScoopX.Services;

namespace ScoopX.Pages
{
    public sealed partial class AddDatabaseDialog : ContentDialog
    {
        private UIElement? _smokeLayer;

        public string DatabaseName { get; private set; } = string.Empty;
        public string Charset { get; private set; } = "utf8mb4";
        public string Username { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public string Remark { get; private set; } = string.Empty;
        public bool IsConfirmed { get; private set; }

        public AddDatabaseDialog()
        {
            InitializeComponent();
            RequestedTheme = ThemeService.Instance.CurrentElementTheme;
            Opened += AddDatabaseDialog_Opened;
            Closed += AddDatabaseDialog_Closed;
        }

        private void AddDatabaseDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
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

        private void AddDatabaseDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
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

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var name = DatabaseNameTextBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                args.Cancel = true;
                DatabaseNameTextBox.Focus(FocusState.Programmatic);
                return;
            }

            DatabaseName = name;
            Charset = (CharsetComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString()
                ?? (CharsetComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString()
                ?? "utf8mb4";
            Username = UsernameTextBox.Text?.Trim() ?? string.Empty;
            Password = PasswordBox.Password ?? string.Empty;
            Remark = RemarkTextBox.Text?.Trim() ?? string.Empty;
            IsConfirmed = true;
        }
    }
}
