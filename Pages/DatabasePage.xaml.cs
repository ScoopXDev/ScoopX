using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Composition;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using ScoopX.Controls;
using ScoopX.Services;
using Windows.Foundation;
using VirtualKey = Windows.System.VirtualKey;

namespace ScoopX.Pages
{
    public sealed class DatabaseItem : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _remark = string.Empty;
        private string _charset = "utf8mb4";

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public string Username
        {
            get => _username;
            set => SetField(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetField(ref _password, value);
        }

        public string Remark
        {
            get => _remark;
            set => SetField(ref _remark, value);
        }

        public string Charset
        {
            get => _charset;
            set => SetField(ref _charset, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed partial class DatabasePage : Page
    {
        private readonly List<DatabaseItem> _mysqlDatabases = CreateMySqlDemoData();
        private int _mysqlPageSize = 10;
        private int _mysqlCurrentPage = 1;
        private int _mysqlTotalPages = 1;
        private bool _suppressMySqlPageSizeEvent = true;

        private string _selectedTab = "MySQL";
        private Storyboard? _indicatorStoryboard;
        private double _indicatorX;
        private bool _tabBarReady;
        private DispatcherQueueTimer? _hoverHideTimer;
        private CompositionScopedBatch? _hoverAnimBatch;
        private bool _hoverMenuVisible;
        private int _hoverEpoch;

        public DatabasePage()
        {
            InitializeComponent();
            InitMySqlPageSizeCombo();
            _suppressMySqlPageSizeEvent = false;
            if (MySqlPageSizeComboBox.SelectedItem is ComboBoxItem selected
                && int.TryParse(selected.Tag?.ToString(), out var size)
                && size > 0)
            {
                _mysqlPageSize = size;
            }

            RefreshMySqlPage();
            ApplyTabColors(_selectedTab);
            ThemeService.Instance.ThemeChanged += OnThemeChanged;
            Unloaded += OnUnloaded;
        }

        private void InitMySqlPageSizeCombo()
        {
            var pageFmt = Res.Get("SitesPageSizeFormat");
            MySqlPageSizeComboBox.Items.Clear();
            foreach (var size in new[] { 5, 10, 20, 50 })
            {
                MySqlPageSizeComboBox.Items.Add(new ComboBoxItem
                {
                    Content = string.Format(pageFmt, size),
                    Tag = size.ToString(),
                    IsSelected = size == 10,
                });
            }
        }

        private static List<DatabaseItem> CreateMySqlDemoData()
        {
            return new List<DatabaseItem>
            {
                new() { Name = "wordpress_blog", Username = "wp_user", Password = "Wp#8f2c1a9", Charset = "utf8mb4", Remark = "公司官网 WordPress" },
                new() { Name = "laravel_shop", Username = "shop_app", Password = "Sh0p!e4b7d2", Charset = "utf8mb4", Remark = "商城 Laravel" },
                new() { Name = "thinkphp_crm", Username = "crm_admin", Password = "Crm@9d3e6f1", Charset = "utf8mb4", Remark = "客户管理 ThinkPHP" },
                new() { Name = "discuz_forum", Username = "forum_user", Password = "Frum#2a7c88", Charset = "utf8mb4", Remark = "内部论坛 Discuz" },
                new() { Name = "api_gateway", Username = "api_rw", Password = "Api$5c1b90e", Charset = "utf8mb4", Remark = "接口服务库" },
                new() { Name = "mall_order", Username = "order_svc", Password = "Ord!7e2a4b6", Charset = "utf8mb4", Remark = "订单子系统" },
                new() { Name = "cms_docs", Username = "docs_user", Password = "Docs#3f9a12", Charset = "utf8mb4", Remark = "文档站 CMS" },
                new() { Name = "test_sandbox", Username = "tester", Password = "Test@1b8d44", Charset = "utf8mb4", Remark = "联调沙箱" },
            };
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ThemeService.Instance.ThemeChanged -= OnThemeChanged;
            _hoverHideTimer?.Stop();
            _hoverAnimBatch?.Dispose();
            _hoverAnimBatch = null;
            _hoverEpoch++;
            _hoverMenuVisible = false;
        }

        private void OnThemeChanged(object? sender, EventArgs e) => ApplyTabColors(_selectedTab);

        private void TabBarHost_Loaded(object sender, RoutedEventArgs e)
        {
            _tabBarReady = true;
            MoveIndicator(_selectedTab, animate: false);
        }

        private void TabBarHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_tabBarReady)
            {
                MoveIndicator(_selectedTab, animate: false);
            }
        }

        private void DatabaseTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: string tag } || tag == _selectedTab)
            {
                return;
            }

            _selectedTab = tag;
            ApplyTabColors(tag);
            MoveIndicator(tag, animate: true);
            ShowPanel(tag);
        }

        private async void MySqlAddDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            HideMySqlServiceHoverMenu(animate: false);

            var dialog = new AddDatabaseDialog
            {
                XamlRoot = XamlRoot,
                RequestedTheme = ThemeService.Instance.CurrentElementTheme,
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Secondary || !dialog.IsConfirmed)
            {
                return;
            }

            _mysqlDatabases.Insert(0, new DatabaseItem
            {
                Name = dialog.DatabaseName,
                Username = dialog.Username,
                Password = dialog.Password,
                Charset = dialog.Charset,
                Remark = dialog.Remark,
            });
            _mysqlCurrentPage = 1;
            RefreshMySqlPage();
        }

        private async void MySqlRootPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            HideMySqlServiceHoverMenu(animate: false);

            var dialog = new RootPasswordDialog
            {
                XamlRoot = XamlRoot,
                RequestedTheme = ThemeService.Instance.CurrentElementTheme,
            };

            await dialog.ShowAsync();
        }

        private void DatabaseManage_Click(object sender, RoutedEventArgs e)
        {
            // 后续接入管理页
        }

        private void DatabaseChangePassword_Click(object sender, RoutedEventArgs e)
        {
            // 后续接入改密弹窗
        }

        private async void DatabaseDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement { Tag: DatabaseItem item })
            {
                return;
            }

            var confirm = new ContentDialog
            {
                XamlRoot = XamlRoot,
                RequestedTheme = ThemeService.Instance.CurrentElementTheme,
                Title = item.Name,
                Content = Res.Get("DbDeleteConfirm"),
                PrimaryButtonText = Res.Get("DbDeleteCancel"),
                SecondaryButtonText = Res.Get("DbDeleteOk"),
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonStyle = (Style)Application.Current.Resources["SoftCancelButtonStyle"],
                SecondaryButtonStyle = (Style)Application.Current.Resources["SoftConfirmButtonStyle"],
                CornerRadius = new CornerRadius(12),
            };

            if (await confirm.ShowAsync() != ContentDialogResult.Secondary)
            {
                return;
            }

            _mysqlDatabases.Remove(item);
            RefreshMySqlPage();
        }

        private void RefreshMySqlPage()
        {
            var total = _mysqlDatabases.Count;
            _mysqlTotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)_mysqlPageSize));
            if (_mysqlCurrentPage > _mysqlTotalPages)
            {
                _mysqlCurrentPage = _mysqlTotalPages;
            }

            if (_mysqlCurrentPage < 1)
            {
                _mysqlCurrentPage = 1;
            }

            MySqlDatabaseList.ItemsSource = _mysqlDatabases
                .Skip((_mysqlCurrentPage - 1) * _mysqlPageSize)
                .Take(_mysqlPageSize)
                .ToList();

            MySqlEmptyHint.Visibility = total == 0 ? Visibility.Visible : Visibility.Collapsed;
            MySqlTotalCountText.Text = string.Format(Res.Get("SitesTotalFormat"), total);
            MySqlJumpPageTextBox.Text = _mysqlCurrentPage.ToString();
            MySqlPrevPageButton.IsEnabled = _mysqlCurrentPage > 1;
            MySqlNextPageButton.IsEnabled = _mysqlCurrentPage < _mysqlTotalPages;
            RebuildMySqlPageNumbers();
        }

        private void RebuildMySqlPageNumbers()
        {
            MySqlPageNumbersPanel.Children.Clear();

            foreach (var page in GetMySqlVisiblePages())
            {
                if (page < 0)
                {
                    MySqlPageNumbersPanel.Children.Add(new TextBlock
                    {
                        Text = "…",
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(4, 0, 4, 0),
                    });
                    continue;
                }

                var pageNumber = page;
                var button = new HandCursorButton
                {
                    Content = pageNumber.ToString(),
                    MinWidth = 32,
                    Padding = new Thickness(8, 4, 8, 4),
                    Tag = pageNumber,
                };

                if (pageNumber == _mysqlCurrentPage
                    && Application.Current.Resources.ContainsKey("SoftAccentButtonStyle"))
                {
                    button.Style = (Style)Application.Current.Resources["SoftAccentButtonStyle"];
                }

                button.Click += MySqlPageNumberButton_Click;
                MySqlPageNumbersPanel.Children.Add(button);
            }
        }

        private IEnumerable<int> GetMySqlVisiblePages()
        {
            if (_mysqlTotalPages <= 7)
            {
                for (var i = 1; i <= _mysqlTotalPages; i++)
                {
                    yield return i;
                }

                yield break;
            }

            yield return 1;

            var start = Math.Max(2, _mysqlCurrentPage - 1);
            var end = Math.Min(_mysqlTotalPages - 1, _mysqlCurrentPage + 1);

            if (start > 2)
            {
                yield return -1;
            }

            for (var i = start; i <= end; i++)
            {
                yield return i;
            }

            if (end < _mysqlTotalPages - 1)
            {
                yield return -1;
            }

            yield return _mysqlTotalPages;
        }

        private void MySqlPageNumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: int page })
            {
                return;
            }

            _mysqlCurrentPage = page;
            RefreshMySqlPage();
        }

        private void MySqlPageSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressMySqlPageSizeEvent
                || MySqlPageSizeComboBox.SelectedItem is not ComboBoxItem item
                || !int.TryParse(item.Tag?.ToString(), out var size)
                || size <= 0)
            {
                return;
            }

            _mysqlPageSize = size;
            _mysqlCurrentPage = 1;
            RefreshMySqlPage();
        }

        private void MySqlJumpPageTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter)
            {
                return;
            }

            if (!int.TryParse(MySqlJumpPageTextBox.Text, out var page))
            {
                MySqlJumpPageTextBox.Text = _mysqlCurrentPage.ToString();
                return;
            }

            _mysqlCurrentPage = Math.Clamp(page, 1, _mysqlTotalPages);
            RefreshMySqlPage();
        }

        private void MySqlPrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mysqlCurrentPage <= 1)
            {
                return;
            }

            _mysqlCurrentPage--;
            RefreshMySqlPage();
        }

        private void MySqlNextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mysqlCurrentPage >= _mysqlTotalPages)
            {
                return;
            }

            _mysqlCurrentPage++;
            RefreshMySqlPage();
        }

        private void MySqlServiceChipHost_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CancelHoverHide();
            ShowMySqlServiceHoverMenu();
        }

        private void MySqlServiceChipHost_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ScheduleHoverHide();
        }

        private void MySqlServiceHoverMenu_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CancelHoverHide();
        }

        private void MySqlServiceHoverMenu_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ScheduleHoverHide();
        }

        private void MySqlServiceAction_Click(object sender, RoutedEventArgs e)
        {
            // 后续接入实际服务控制；先关闭悬停菜单
            HideMySqlServiceHoverMenu(animate: true);
        }

        private void ShowMySqlServiceHoverMenu()
        {
            CancelHoverHide();
            _hoverEpoch++;
            var epoch = _hoverEpoch;

            MySqlServiceHoverMenu.IsHitTestVisible = true;
            _hoverMenuVisible = true;
            AnimateHoverMenuOpacity(
                GetHoverMenuOpacity(),
                1f,
                TimeSpan.FromMilliseconds(220),
                epoch,
                () =>
                {
                    if (epoch != _hoverEpoch)
                    {
                        return;
                    }

                    SetHoverMenuOpacity(1);
                });
        }

        private void HideMySqlServiceHoverMenu(bool animate)
        {
            CancelHoverHide();
            _hoverEpoch++;
            var epoch = _hoverEpoch;

            if (!animate)
            {
                SetHoverMenuOpacity(0);
                MySqlServiceHoverMenu.IsHitTestVisible = false;
                _hoverMenuVisible = false;
                return;
            }

            AnimateHoverMenuOpacity(
                GetHoverMenuOpacity(),
                0f,
                TimeSpan.FromMilliseconds(150),
                epoch,
                () =>
                {
                    if (epoch != _hoverEpoch)
                    {
                        return;
                    }

                    MySqlServiceHoverMenu.IsHitTestVisible = false;
                    _hoverMenuVisible = false;
                });
        }

        private Visual GetHoverMenuVisual() =>
            ElementCompositionPreview.GetElementVisual(MySqlServiceHoverMenu);

        private float GetHoverMenuOpacity()
        {
            // Composition 与 XAML Opacity 取较大值，避免动画中途读到 0 导致“这次不显示”
            var visualOpacity = GetHoverMenuVisual().Opacity;
            var elementOpacity = (float)MySqlServiceHoverMenu.Opacity;
            return Math.Max(visualOpacity, elementOpacity);
        }

        private void SetHoverMenuOpacity(float value)
        {
            var visual = GetHoverMenuVisual();
            visual.StopAnimation("Opacity");
            visual.Opacity = value;
            MySqlServiceHoverMenu.Opacity = value;
        }

        private void AnimateHoverMenuOpacity(
            float from,
            float to,
            TimeSpan duration,
            int epoch,
            Action? onCompleted = null)
        {
            _hoverAnimBatch?.Dispose();
            _hoverAnimBatch = null;

            var visual = GetHoverMenuVisual();
            var compositor = visual.Compositor;
            visual.StopAnimation("Opacity");
            visual.Opacity = from;
            MySqlServiceHoverMenu.Opacity = from;

            var anim = compositor.CreateScalarKeyFrameAnimation();
            anim.InsertKeyFrame(0f, from);
            anim.InsertKeyFrame(
                1f,
                to,
                compositor.CreateCubicBezierEasingFunction(new Vector2(0.16f, 1f), new Vector2(0.3f, 1f)));
            anim.Duration = duration;
            anim.StopBehavior = AnimationStopBehavior.SetToFinalValue;

            if (onCompleted is null)
            {
                visual.StartAnimation("Opacity", anim);
                return;
            }

            var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _hoverAnimBatch = batch;
            batch.Completed += (_, _) =>
            {
                if (ReferenceEquals(_hoverAnimBatch, batch))
                {
                    _hoverAnimBatch = null;
                }

                batch.Dispose();
                if (epoch != _hoverEpoch)
                {
                    return;
                }

                SetHoverMenuOpacity(to);
                onCompleted();
            };
            visual.StartAnimation("Opacity", anim);
            batch.End();
        }

        private void ScheduleHoverHide()
        {
            _hoverHideTimer ??= DispatcherQueue.CreateTimer();
            _hoverHideTimer.IsRepeating = false;
            _hoverHideTimer.Interval = TimeSpan.FromMilliseconds(120);
            _hoverHideTimer.Tick -= HoverHideTimer_Tick;
            _hoverHideTimer.Tick += HoverHideTimer_Tick;
            _hoverHideTimer.Start();
        }

        private void CancelHoverHide()
        {
            _hoverHideTimer?.Stop();
        }

        private void HoverHideTimer_Tick(DispatcherQueueTimer sender, object args)
        {
            sender.Stop();
            HideMySqlServiceHoverMenu(animate: true);
        }

        private void ApplyTabColors(string tag)
        {
            var accent = (Brush)Application.Current.Resources["ScoopAccentBrush"];
            var secondary = GetThemeBrush("ScoopTextSecondaryBrush");

            MySqlTabText.Foreground = tag == "MySQL" ? accent : secondary;
            SqlServerTabText.Foreground = tag == "SQLServer" ? accent : secondary;
            RedisTabText.Foreground = tag == "Redis" ? accent : secondary;
            MongoDbTabText.Foreground = tag == "MongoDB" ? accent : secondary;
            PgSqlTabText.Foreground = tag == "PgSQL" ? accent : secondary;

            ShowPanel(tag);
        }

        private void ShowPanel(string tag)
        {
            MySqlPanel.Visibility = tag == "MySQL" ? Visibility.Visible : Visibility.Collapsed;
            SqlServerPanel.Visibility = tag == "SQLServer" ? Visibility.Visible : Visibility.Collapsed;
            RedisPanel.Visibility = tag == "Redis" ? Visibility.Visible : Visibility.Collapsed;
            MongoDbPanel.Visibility = tag == "MongoDB" ? Visibility.Visible : Visibility.Collapsed;
            PgSqlPanel.Visibility = tag == "PgSQL" ? Visibility.Visible : Visibility.Collapsed;

            if (tag != "MySQL")
            {
                HideMySqlServiceHoverMenu(animate: false);
            }
        }

        private HandCursorButton? GetTabButton(string tag) => tag switch
        {
            "MySQL" => MySqlTabButton,
            "SQLServer" => SqlServerTabButton,
            "Redis" => RedisTabButton,
            "MongoDB" => MongoDbTabButton,
            "PgSQL" => PgSqlTabButton,
            _ => null
        };

        private void MoveIndicator(string tag, bool animate)
        {
            var target = GetTabButton(tag);
            if (target is null || TabIndicator is null || TabBarHost is null)
            {
                return;
            }

            target.UpdateLayout();
            TabBarHost.UpdateLayout();

            var origin = target.TransformToVisual(TabBarHost).TransformPoint(new Point(0, 0));
            var targetX = origin.X + (target.ActualWidth - TabIndicator.Width) / 2.0;
            if (targetX < 0)
            {
                targetX = 0;
            }

            CommitIndicatorOffset(_indicatorX);
            _indicatorStoryboard?.Stop();
            _indicatorStoryboard = null;

            if (!animate)
            {
                CommitIndicatorOffset(targetX);
                return;
            }

            var fromX = _indicatorX;
            var animation = new DoubleAnimation
            {
                From = fromX,
                To = targetX,
                Duration = new Duration(TimeSpan.FromMilliseconds(280)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                EnableDependentAnimation = true,
                FillBehavior = FillBehavior.HoldEnd
            };
            Storyboard.SetTarget(animation, TabIndicatorTransform);
            Storyboard.SetTargetProperty(animation, "X");

            _indicatorStoryboard = new Storyboard();
            _indicatorStoryboard.Children.Add(animation);
            _indicatorStoryboard.Completed += (_, _) => CommitIndicatorOffset(targetX);
            _indicatorX = targetX;
            _indicatorStoryboard.Begin();
        }

        private void CommitIndicatorOffset(double x)
        {
            _indicatorX = x;
            TabIndicatorTransform.X = x;
        }

        private Brush GetThemeBrush(string key)
        {
            var theme = ActualTheme;
            if (theme == ElementTheme.Default)
            {
                theme = Application.Current.RequestedTheme == ApplicationTheme.Dark
                    ? ElementTheme.Dark
                    : ElementTheme.Light;
            }

            var dictKey = theme == ElementTheme.Dark ? "Dark" : "Light";
            var dicts = Application.Current.Resources.ThemeDictionaries;
            var rd = (ResourceDictionary)dicts[dictKey];
            return (Brush)rd[key];
        }
    }
}
