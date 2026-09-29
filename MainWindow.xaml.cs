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
using ScoopX.Services;
using Windows.Foundation;
using Windows.UI;

namespace ScoopX
{
    public sealed partial class MainWindow : Window
    {
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
            ThemeService.Instance.ThemeChanged += (_, _) => RefreshNavColors();
            ApplyNavSelection("Home", animatePill: false);
            NavigateTo("Home");

            if (AppSettings.Instance.StartHiddenToTray)
            {
                // 延迟到窗口激活后再藏，避免闪一下又立刻 Hide 的竞态
                void OnActivated(object sender, WindowActivatedEventArgs args)
                {
                    Activated -= OnActivated;
                    HideToTray();
                }
                Activated += OnActivated;
            }
        }

        private SolidColorBrush ActiveBrush =>
            (SolidColorBrush)Application.Current.Resources["ScoopAccentBrush"];

        private SolidColorBrush InactiveBrush =>
            (SolidColorBrush)GetThemeBrush("ScoopNavInactiveBrush");

        private void RefreshNavColors()
        {
            ApplyNavSelection(_selectedTag, animatePill: false);
        }

        private static string GetThemeDictionaryKey(FrameworkElement root)
        {
            var theme = root.ActualTheme;
            if (theme == ElementTheme.Default)
            {
                theme = Application.Current.RequestedTheme == ApplicationTheme.Dark
                    ? ElementTheme.Dark
                    : ElementTheme.Light;
            }

            return theme == ElementTheme.Dark ? "Dark" : "Light";
        }

        private Brush GetThemeBrush(string key)
        {
            var root = (FrameworkElement)Content;
            var dicts = Application.Current.Resources.ThemeDictionaries;
            var rd = (ResourceDictionary)dicts[GetThemeDictionaryKey(root)];
            return (Brush)rd[key];
        }

        private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            if (_isExitRequested)
            {
                return;
            }

            if (AppSettings.Instance.CloseToTray)
            {
                args.Cancel = true;
                HideToTray();
                return;
            }

            // 允许关闭：先清理托盘
            TrayIcon.Dispose();
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
            if (AppSettings.Instance.CloseToTray)
            {
                HideToTray();
            }
            else
            {
                ExitApplication();
            }
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

        public void RequestExit() => ExitApplication();

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
                case "Settings":
                    ContentFrame.Navigate(typeof(SettingsPage));
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

        private void SetNavItemState(FontIcon icon, TextBlock label, bool selected)
        {
            icon.Foreground = selected ? ActiveBrush : InactiveBrush;
            label.Foreground = selected ? ActiveBrush : InactiveBrush;
        }
    }
}
