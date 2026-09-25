using System;
using System.IO;
using System.Windows.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using ScoopX.Controls;
using ScoopX.Pages;
using Windows.Foundation;
using Windows.UI;

namespace ScoopX
{
    public sealed partial class MainWindow : Window
    {
        private static readonly SolidColorBrush ActiveBrush = new(Color.FromArgb(255, 37, 99, 235)); // #2563EB 全局主色
        private static readonly SolidColorBrush InactiveBrush = new(Color.FromArgb(255, 75, 85, 99));

        private string _selectedTag = "Home";
        private Storyboard? _pillStoryboard;
        private bool _railReady;
        private double _pillY;
        private bool _isExitRequested;

        public ICommand ShowWindowCommand { get; }

        public MainWindow()
        {
            ShowWindowCommand = new RelayCommand(ShowFromTray);

            InitializeComponent();

            // 固定合适尺寸，禁止随意缩放
            const int windowWidth = 1060;
            const int windowHeight = 660;
            AppWindow.Resize(new Windows.Graphics.SizeInt32(windowWidth, windowHeight));

            var displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest);
            if (displayArea is not null)
            {
                var work = displayArea.WorkArea;
                AppWindow.Move(new Windows.Graphics.PointInt32(
                    work.X + (work.Width - windowWidth) / 2,
                    work.Y + (work.Height - windowHeight) / 2));
            }

            // 去掉系统标题栏按钮（含灰色最大化占位），改用自定义最小化 / 关闭
            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsMaximizable = false;
                presenter.IsResizable = false;
                presenter.SetBorderAndTitleBar(true, false);
            }

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            AppWindow.Closing += AppWindow_Closing;

            SetWindowIcon();
            WireNavHover();
            ApplyNavSelection("Home", animatePill: false);
            NavigateTo("Home");
        }

        private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            // 非主动退出时，关闭只是藏到托盘
            if (_isExitRequested)
            {
                return;
            }

            args.Cancel = true;
            HideToTray();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Minimize();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            HideToTray();
        }

        private void TrayShow_Click(object sender, RoutedEventArgs e)
        {
            ShowFromTray();
        }

        private void TrayExit_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication();
        }

        private void HideToTray()
        {
            AppWindow.Hide();
        }

        private void ShowFromTray()
        {
            AppWindow.Show();
            Activate();
        }

        private void ExitApplication()
        {
            _isExitRequested = true;
            TrayIcon.Dispose();
            Close();
        }

        private void SetWindowIcon()
        {
            var iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "ScoopX.ico");
            if (File.Exists(iconPath))
            {
                AppWindow.SetIcon(iconPath);
            }
        }

        private void WireNavHover()
        {
            foreach (var button in new[] { NavHome, NavWebsites, NavDatabase, NavStore, NavSettings })
            {
                button.PointerEntered += NavItem_PointerEntered;
                button.PointerExited += NavItem_PointerExited;
            }
        }

        private void RailHost_Loaded(object sender, RoutedEventArgs e)
        {
            _railReady = true;
            MoveSelectionPill(GetNavElement(_selectedTag), animate: false);
        }

        private void RailHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_railReady)
            {
                MoveSelectionPill(GetNavElement(_selectedTag), animate: false);
            }
        }

        private void NavItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not HandCursorButton button || button.Tag is not string tag)
            {
                return;
            }

            ApplyNavSelection(tag, animatePill: true);
            NavigateTo(tag);
        }

        private void NavItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is not HandCursorButton button || button.Tag is not string tag)
            {
                return;
            }

            SetNavColors(tag, highlighted: true);
        }

        private void NavItem_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is not HandCursorButton button || button.Tag is not string tag)
            {
                return;
            }

            SetNavColors(tag, highlighted: tag == _selectedTag);
        }

        private void NavigateTo(string tag)
        {
            switch (tag)
            {
                case "Home":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;
                case "Websites":
                    ContentFrame.Navigate(typeof(WebsitesPage));
                    break;
                default:
                    ContentFrame.Content = null;
                    break;
            }
        }

        public void NavigateNav(string tag)
        {
            ApplyNavSelection(tag, animatePill: true);
            NavigateTo(tag);
        }

        private void ApplyNavSelection(string tag, bool animatePill)
        {
            _selectedTag = tag;
            SetNavItemState(NavHomeIcon, NavHomeText, tag == "Home");
            SetNavItemState(NavWebsitesIcon, NavWebsitesText, tag == "Websites");
            SetNavItemState(NavDatabaseIcon, NavDatabaseText, tag == "Database");
            SetNavItemState(NavStoreIcon, NavStoreText, tag == "Store");
            SetNavItemState(NavSettingsIcon, NavSettingsText, tag == "Settings");

            if (_railReady)
            {
                MoveSelectionPill(GetNavElement(tag), animatePill);
            }
        }

        private FrameworkElement? GetNavElement(string tag) => tag switch
        {
            "Home" => NavHome,
            "Websites" => NavWebsites,
            "Database" => NavDatabase,
            "Store" => NavStore,
            "Settings" => NavSettings,
            _ => null
        };

        private void MoveSelectionPill(FrameworkElement? target, bool animate)
        {
            if (target is null || SelectionPill is null || RailContent is null)
            {
                return;
            }

            target.UpdateLayout();
            RailContent.UpdateLayout();

            var itemHeight = target.ActualHeight > 1
                ? target.ActualHeight
                : (target.Height > 1 ? target.Height : 56);

            // 与选中条同一父容器坐标系，直接对齐垂直中心
            var targetTop = target.TransformToVisual(RailContent).TransformPoint(new Point(0, 0)).Y;
            var targetY = targetTop + (itemHeight - SelectionPill.Height) / 2.0;
            if (targetY < 0)
            {
                targetY = 0;
            }

            // Stop() 会把属性弹回动画开始前的基值，先把当前位置写回本地值再停
            CommitPillOffset(_pillY);
            _pillStoryboard?.Stop();
            _pillStoryboard = null;

            if (!animate)
            {
                CommitPillOffset(targetY);
                return;
            }

            var fromY = _pillY;
            var animation = new DoubleAnimation
            {
                From = fromY,
                To = targetY,
                Duration = new Duration(TimeSpan.FromMilliseconds(280)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                EnableDependentAnimation = true,
                FillBehavior = FillBehavior.HoldEnd
            };
            Storyboard.SetTarget(animation, SelectionPillTransform);
            Storyboard.SetTargetProperty(animation, "Y");

            _pillStoryboard = new Storyboard();
            _pillStoryboard.Children.Add(animation);
            _pillStoryboard.Completed += (_, _) => CommitPillOffset(targetY);
            _pillY = targetY;
            _pillStoryboard.Begin();
        }

        private void CommitPillOffset(double y)
        {
            _pillY = y;
            SelectionPillTransform.Y = y;
        }

        private void SetNavColors(string tag, bool highlighted)
        {
            var brush = highlighted ? ActiveBrush : InactiveBrush;
            switch (tag)
            {
                case "Home":
                    NavHomeIcon.Foreground = brush;
                    NavHomeText.Foreground = brush;
                    break;
                case "Websites":
                    NavWebsitesIcon.Foreground = brush;
                    NavWebsitesText.Foreground = brush;
                    break;
                case "Database":
                    NavDatabaseIcon.Foreground = brush;
                    NavDatabaseText.Foreground = brush;
                    break;
                case "Store":
                    NavStoreIcon.Foreground = brush;
                    NavStoreText.Foreground = brush;
                    break;
                case "Settings":
                    NavSettingsIcon.Foreground = brush;
                    NavSettingsText.Foreground = brush;
                    break;
            }
        }

        private static void SetNavItemState(FontIcon icon, TextBlock label, bool selected)
        {
            icon.Foreground = selected ? ActiveBrush : InactiveBrush;
            label.Foreground = selected ? ActiveBrush : InactiveBrush;
        }
    }
}
