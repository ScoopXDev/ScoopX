using System;
using System.Security.Cryptography;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using ScoopX.Services;

namespace ScoopX.Pages
{
    public sealed partial class RootPasswordDialog : ContentDialog
    {
        public const string SettingsKey = "MySqlRootPassword";

        private UIElement? _smokeLayer;

        public string Password { get; private set; } = string.Empty;
        public bool IsConfirmed { get; private set; }

        public RootPasswordDialog()
        {
            InitializeComponent();
            RequestedTheme = ThemeService.Instance.CurrentElementTheme;
            ToolTipService.SetToolTip(RegenerateButton, Res.Get("RootPasswordRegenerateTooltip"));
            Opened += RootPasswordDialog_Opened;
            Closed += RootPasswordDialog_Closed;
            LoadInitialPassword();
        }

        private void LoadInitialPassword()
        {
            if (LocalSettingsStore.TryGetString(SettingsKey, out var saved)
                && !string.IsNullOrWhiteSpace(saved))
            {
                PasswordTextBox.Text = saved;
                return;
            }

            PasswordTextBox.Text = GeneratePassword();
        }

        private void RegenerateButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = GeneratePassword();
            PasswordTextBox.Focus(FocusState.Programmatic);
            PasswordTextBox.SelectAll();
        }

        private static string GeneratePassword()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(8)).ToLowerInvariant();
        }

        private void RootPasswordDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
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

        private void RootPasswordDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
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
            var password = PasswordTextBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(password))
            {
                args.Cancel = true;
                PasswordTextBox.Focus(FocusState.Programmatic);
                return;
            }

            Password = password;
            LocalSettingsStore.SetString(SettingsKey, password);
            IsConfirmed = true;
            ToastService.Show(
                Res.Get("RootPasswordSavedTitle"),
                Res.Get("RootPasswordSavedMessage"));
        }
    }
}
