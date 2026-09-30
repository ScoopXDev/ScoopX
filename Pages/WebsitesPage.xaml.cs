using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Windows.System;
using DispatcherQueueTimer = Microsoft.UI.Dispatching.DispatcherQueueTimer;

namespace ScoopX.Pages
{
    public sealed class WebsiteItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string _name = string.Empty;
        private string _status = string.Empty;
        private string _rootPath = string.Empty;
        private string _runPath = string.Empty;
        private string _remark = string.Empty;
        private string _phpVersion = string.Empty;
        private string _projectType = ProjectTypePhp;

        public const string ProjectTypePhp = "php";
        public const string ProjectTypeProxy = "proxy";
        public const string ProjectTypeHtml = "html";

        public string ProjectType
        {
            get => _projectType;
            set => SetField(ref _projectType, value);
        }

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public string Status
        {
            get => _status;
            set
            {
                if (Equals(_status, value))
                {
                    return;
                }

                _status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
            }
        }

        public const string StatusRunning = "running";
        public const string StatusStopped = "stopped";

        public bool IsRunning
        {
            get => _status == StatusRunning;
            set => Status = value ? StatusRunning : StatusStopped;
        }

        public string RootPath
        {
            get => _rootPath;
            set => SetField(ref _rootPath, value);
        }

        public string RunPath
        {
            get => _runPath;
            set => SetField(ref _runPath, value);
        }

        public string Remark
        {
            get => _remark;
            set => SetField(ref _remark, value);
        }

        public string PhpVersion
        {
            get => _phpVersion;
            set => SetField(ref _phpVersion, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetField(ref _isSelected, value);
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

    public sealed partial class WebsitesPage : Page
    {
        private int _pageSize = 10;
        private readonly List<WebsiteItem> _allItems = CreateDemoData();
        private int _currentPage = 1;
        private int _totalPages = 1;
        private bool _suppressSelectionEvents;
        private bool _suppressPageSizeEvent = true;
        private bool _suppressStatusToggle;
        private DispatcherQueueTimer? _nginxHoverHideTimer;
        private CompositionScopedBatch? _nginxHoverAnimBatch;
        private int _nginxHoverEpoch;
        private string _selectedTab = WebsiteItem.ProjectTypePhp;
        private Storyboard? _indicatorStoryboard;
        private double _indicatorX;
        private bool _tabBarReady;

        public WebsitesPage()
        {
            InitializeComponent();
            InitLocalizedCombos();
            _suppressPageSizeEvent = false;
            if (PageSizeComboBox.SelectedItem is ComboBoxItem selected
                && int.TryParse(selected.Tag?.ToString(), out var size)
                && size > 0)
            {
                _pageSize = size;
            }

            RefreshPage();
            ApplyTabColors(_selectedTab);
            ShowPanel(_selectedTab);
            ThemeService.Instance.ThemeChanged += OnThemeChanged;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ThemeService.Instance.ThemeChanged -= OnThemeChanged;
            _nginxHoverHideTimer?.Stop();
            _nginxHoverAnimBatch?.Dispose();
            _nginxHoverAnimBatch = null;
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

        private void SitesTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: string tag } || tag == _selectedTab)
            {
                return;
            }

            _selectedTab = tag;
            _currentPage = 1;
            ApplyTabColors(tag);
            MoveIndicator(tag, animate: true);
            ShowPanel(tag);
            if (tag == WebsiteItem.ProjectTypePhp)
            {
                RefreshPage();
            }
        }

        private void ShowPanel(string tag)
        {
            PhpPanel.Visibility = tag == WebsiteItem.ProjectTypePhp ? Visibility.Visible : Visibility.Collapsed;
            ProxyPanel.Visibility = tag == WebsiteItem.ProjectTypeProxy ? Visibility.Visible : Visibility.Collapsed;
            HtmlPanel.Visibility = tag == WebsiteItem.ProjectTypeHtml ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ApplyTabColors(string tag)
        {
            var accent = (Brush)Application.Current.Resources["ScoopAccentBrush"];
            var secondary = GetThemeBrush("ScoopTextSecondaryBrush");
            PhpTabText.Foreground = tag == WebsiteItem.ProjectTypePhp ? accent : secondary;
            ProxyTabText.Foreground = tag == WebsiteItem.ProjectTypeProxy ? accent : secondary;
            HtmlTabText.Foreground = tag == WebsiteItem.ProjectTypeHtml ? accent : secondary;
        }

        private HandCursorButton? GetTabButton(string tag) => tag switch
        {
            WebsiteItem.ProjectTypePhp => PhpTabButton,
            WebsiteItem.ProjectTypeProxy => ProxyTabButton,
            WebsiteItem.ProjectTypeHtml => HtmlTabButton,
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

        private void NginxServiceChipHost_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CancelNginxHoverHide();
            ShowNginxServiceHoverMenu();
        }

        private void NginxServiceChipHost_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ScheduleNginxHoverHide();
        }

        private void NginxServiceHoverMenu_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CancelNginxHoverHide();
        }

        private void NginxServiceHoverMenu_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ScheduleNginxHoverHide();
        }

        private void NginxServiceAction_Click(object sender, RoutedEventArgs e)
        {
            HideNginxServiceHoverMenu(animate: true);
            if (sender is not FrameworkElement { Tag: string action })
            {
                return;
            }

            // 后续接入真实 Nginx 控制；先给 Windows 通知反馈
            switch (action)
            {
                case "Stop":
                    ToastService.Show(Res.Get("SitesNginxStoppedTitle"), Res.Get("SitesNginxStoppedMessage"));
                    NginxServiceStatusIcon.Fill = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Windows.UI.Color.FromArgb(255, 156, 163, 175));
                    break;
                case "Restart":
                    ToastService.Show(Res.Get("SitesNginxRestartedTitle"), Res.Get("SitesNginxRestartedMessage"));
                    NginxServiceStatusIcon.Fill = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Windows.UI.Color.FromArgb(255, 34, 197, 94));
                    break;
                case "Reload":
                    ToastService.Show(Res.Get("SitesNginxReloadedTitle"), Res.Get("SitesNginxReloadedMessage"));
                    NginxServiceStatusIcon.Fill = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Windows.UI.Color.FromArgb(255, 34, 197, 94));
                    break;
            }
        }

        private void ShowNginxServiceHoverMenu()
        {
            CancelNginxHoverHide();
            _nginxHoverEpoch++;
            var epoch = _nginxHoverEpoch;

            NginxServiceHoverMenu.IsHitTestVisible = true;
            AnimateNginxHoverMenuOpacity(
                GetNginxHoverMenuOpacity(),
                1f,
                TimeSpan.FromMilliseconds(220),
                epoch,
                () =>
                {
                    if (epoch != _nginxHoverEpoch)
                    {
                        return;
                    }

                    SetNginxHoverMenuOpacity(1);
                });
        }

        private void HideNginxServiceHoverMenu(bool animate)
        {
            CancelNginxHoverHide();
            _nginxHoverEpoch++;
            var epoch = _nginxHoverEpoch;

            if (!animate)
            {
                SetNginxHoverMenuOpacity(0);
                NginxServiceHoverMenu.IsHitTestVisible = false;
                return;
            }

            AnimateNginxHoverMenuOpacity(
                GetNginxHoverMenuOpacity(),
                0f,
                TimeSpan.FromMilliseconds(150),
                epoch,
                () =>
                {
                    if (epoch != _nginxHoverEpoch)
                    {
                        return;
                    }

                    NginxServiceHoverMenu.IsHitTestVisible = false;
                });
        }

        private Visual GetNginxHoverMenuVisual() =>
            ElementCompositionPreview.GetElementVisual(NginxServiceHoverMenu);

        private float GetNginxHoverMenuOpacity()
        {
            var visualOpacity = GetNginxHoverMenuVisual().Opacity;
            var elementOpacity = (float)NginxServiceHoverMenu.Opacity;
            return Math.Max(visualOpacity, elementOpacity);
        }

        private void SetNginxHoverMenuOpacity(float value)
        {
            var visual = GetNginxHoverMenuVisual();
            visual.StopAnimation("Opacity");
            visual.Opacity = value;
            NginxServiceHoverMenu.Opacity = value;
        }

        private void AnimateNginxHoverMenuOpacity(
            float from,
            float to,
            TimeSpan duration,
            int epoch,
            Action? onCompleted = null)
        {
            _nginxHoverAnimBatch?.Dispose();
            _nginxHoverAnimBatch = null;

            var visual = GetNginxHoverMenuVisual();
            var compositor = visual.Compositor;
            visual.StopAnimation("Opacity");
            visual.Opacity = from;
            NginxServiceHoverMenu.Opacity = from;

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
            _nginxHoverAnimBatch = batch;
            batch.Completed += (_, _) =>
            {
                if (ReferenceEquals(_nginxHoverAnimBatch, batch))
                {
                    _nginxHoverAnimBatch = null;
                }

                batch.Dispose();
                if (epoch != _nginxHoverEpoch)
                {
                    return;
                }

                SetNginxHoverMenuOpacity(to);
                onCompleted();
            };
            visual.StartAnimation("Opacity", anim);
            batch.End();
        }

        private void ScheduleNginxHoverHide()
        {
            _nginxHoverHideTimer ??= DispatcherQueue.CreateTimer();
            _nginxHoverHideTimer.IsRepeating = false;
            _nginxHoverHideTimer.Interval = TimeSpan.FromMilliseconds(120);
            _nginxHoverHideTimer.Tick -= NginxHoverHideTimer_Tick;
            _nginxHoverHideTimer.Tick += NginxHoverHideTimer_Tick;
            _nginxHoverHideTimer.Start();
        }

        private void CancelNginxHoverHide()
        {
            _nginxHoverHideTimer?.Stop();
        }

        private void NginxHoverHideTimer_Tick(DispatcherQueueTimer sender, object args)
        {
            sender.Stop();
            HideNginxServiceHoverMenu(animate: true);
        }

        private void InitLocalizedCombos()
        {
            var phpFmt = Res.Get("SitesBatchPhpFormat");
            BatchActionComboBox.Items.Clear();
            BatchActionComboBox.Items.Add(new ComboBoxItem { Content = Res.Get("SitesBatchStart.Content"), Tag = "start" });
            BatchActionComboBox.Items.Add(new ComboBoxItem { Content = Res.Get("SitesBatchStop.Content"), Tag = "stop" });
            foreach (var version in new[] { "7.4", "8.0", "8.1", "8.2", "8.3" })
            {
                BatchActionComboBox.Items.Add(new ComboBoxItem
                {
                    Content = string.Format(phpFmt, version),
                    Tag = $"php:{version}"
                });
            }

            BatchActionComboBox.Items.Add(new ComboBoxItem { Content = Res.Get("SitesBatchDelete.Content"), Tag = "delete" });

            var pageFmt = Res.Get("SitesPageSizeFormat");
            PageSizeComboBox.Items.Clear();
            foreach (var size in new[] { 5, 10, 20, 50 })
            {
                PageSizeComboBox.Items.Add(new ComboBoxItem
                {
                    Content = string.Format(pageFmt, size),
                    Tag = size.ToString(),
                    IsSelected = size == 10
                });
            }
        }

        private static List<WebsiteItem> CreateDemoData()
        {
            const string root = @"C:\ScoopX\wwwroot";
            return new List<WebsiteItem>
            {
                new()
                {
                    Name = "www.company.local",
                    Status = WebsiteItem.StatusRunning,
                    RootPath = $@"{root}\www.company.local",
                    RunPath = $@"{root}\www.company.local",
                    Remark = "公司官网 WordPress",
                    PhpVersion = "8.2",
                    ProjectType = WebsiteItem.ProjectTypePhp,
                },
                new()
                {
                    Name = "shop.company.local",
                    Status = WebsiteItem.StatusRunning,
                    RootPath = $@"{root}\shop.company.local",
                    RunPath = $@"{root}\shop.company.local\public",
                    Remark = "商城 Laravel 11",
                    PhpVersion = "8.3",
                    ProjectType = WebsiteItem.ProjectTypePhp,
                },
                new()
                {
                    Name = "crm.company.local",
                    Status = WebsiteItem.StatusRunning,
                    RootPath = $@"{root}\crm.company.local",
                    RunPath = $@"{root}\crm.company.local\public",
                    Remark = "客户管理 ThinkPHP 8",
                    PhpVersion = "8.1",
                    ProjectType = WebsiteItem.ProjectTypePhp,
                },
                new()
                {
                    Name = "forum.company.local",
                    Status = WebsiteItem.StatusStopped,
                    RootPath = $@"{root}\forum.company.local",
                    RunPath = $@"{root}\forum.company.local",
                    Remark = "内部论坛 Discuz",
                    PhpVersion = "7.4",
                    ProjectType = WebsiteItem.ProjectTypePhp,
                },
                new()
                {
                    Name = "api.company.local",
                    Status = WebsiteItem.StatusRunning,
                    RootPath = $@"{root}\api.company.local",
                    RunPath = $@"{root}\api.company.local\public",
                    Remark = "开放 API 网关",
                    PhpVersion = "8.3",
                    ProjectType = WebsiteItem.ProjectTypePhp,
                },
                new()
                {
                    Name = "docs.company.local",
                    Status = WebsiteItem.StatusRunning,
                    RootPath = $@"{root}\docs.company.local",
                    RunPath = $@"{root}\docs.company.local",
                    Remark = "产品文档站",
                    PhpVersion = "8.2",
                    ProjectType = WebsiteItem.ProjectTypePhp,
                },
                new()
                {
                    Name = "admin.company.local",
                    Status = WebsiteItem.StatusStopped,
                    RootPath = $@"{root}\admin.company.local",
                    RunPath = $@"{root}\admin.company.local",
                    Remark = "运营后台",
                    PhpVersion = "8.1",
                    ProjectType = WebsiteItem.ProjectTypePhp,
                },
                new()
                {
                    Name = "staging.shop.local",
                    Status = WebsiteItem.StatusStopped,
                    RootPath = $@"{root}\staging.shop.local",
                    RunPath = $@"{root}\staging.shop.local\public",
                    Remark = "商城预发环境",
                    PhpVersion = "8.2",
                    ProjectType = WebsiteItem.ProjectTypePhp,
                },
                new()
                {
                    Name = "legacy.erp.local",
                    Status = WebsiteItem.StatusStopped,
                    RootPath = $@"{root}\legacy.erp.local",
                    RunPath = $@"{root}\legacy.erp.local",
                    Remark = "旧版 ERP（待迁移）",
                    PhpVersion = "7.4",
                    ProjectType = WebsiteItem.ProjectTypePhp,
                },
                new()
                {
                    Name = "sandbox.dev.local",
                    Status = WebsiteItem.StatusRunning,
                    RootPath = $@"{root}\sandbox.dev.local",
                    RunPath = $@"{root}\sandbox.dev.local\public",
                    Remark = "联调沙箱",
                    PhpVersion = "8.3",
                    ProjectType = WebsiteItem.ProjectTypePhp,
                },
            };
        }

        private async void AddSiteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddSiteDialog
            {
                XamlRoot = XamlRoot,
                RequestedTheme = ThemeService.Instance.CurrentElementTheme,
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Secondary || dialog.CreatedSites.Count == 0)
            {
                return;
            }

            _allItems.InsertRange(0, dialog.CreatedSites);
            foreach (var site in dialog.CreatedSites)
            {
                site.ProjectType = _selectedTab;
                if (_selectedTab != WebsiteItem.ProjectTypePhp)
                {
                    site.PhpVersion = "-";
                }
            }
            _currentPage = 1;
            RefreshPage();
        }

        private async void SiteName_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement { Tag: WebsiteItem site })
            {
                return;
            }

            var dialog = new EditSiteDialog(site)
            {
                XamlRoot = XamlRoot,
                RequestedTheme = ThemeService.Instance.CurrentElementTheme,
            };

            await dialog.ShowAsync();
            if (!dialog.IsConfirmed)
            {
                return;
            }

            RefreshPage();
        }

        private async void RootPath_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement element
                || element.Tag is not string path
                || string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (!Directory.Exists(path))
            {
                var missingDialog = new ContentDialog
                {
                    Title = Res.Get("SitesOpenFailedTitle"),
                    Content = string.Format(Res.Get("SitesOpenFailedMessage"), path),
                    CloseButtonText = Res.Get("SitesOpenFailedOk"),
                    XamlRoot = XamlRoot,
                    RequestedTheme = ThemeService.Instance.CurrentElementTheme,
                };
                await missingDialog.ShowAsync();
                return;
            }

            await Launcher.LaunchFolderPathAsync(path);
        }

        private IEnumerable<WebsiteItem> GetFilteredItems()
        {
            IEnumerable<WebsiteItem> query = _allItems
                .Where(item => item.ProjectType == _selectedTab);

            if (CategoryFilterComboBox?.SelectedItem is ComboBoxItem category
                && category.Tag is string tag
                && tag != "all")
            {
                query = tag switch
                {
                    "running" => query.Where(item => item.Status == WebsiteItem.StatusRunning),
                    "stopped" => query.Where(item => item.Status == WebsiteItem.StatusStopped),
                    _ => query
                };
            }

            var keyword = SearchBox?.Text?.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(item =>
                    item.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                    || item.Remark.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            return query;
        }

        private IEnumerable<WebsiteItem> CurrentPageItems()
        {
            return GetFilteredItems()
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize);
        }

        private List<WebsiteItem> GetSelectedItems()
        {
            return _allItems.Where(item => item.IsSelected).ToList();
        }

        private void RefreshPage()
        {
            var filteredCount = GetFilteredItems().Count();
            _totalPages = Math.Max(1, (int)Math.Ceiling(filteredCount / (double)_pageSize));
            if (_currentPage > _totalPages)
            {
                _currentPage = _totalPages;
            }

            if (_currentPage < 1)
            {
                _currentPage = 1;
            }

            _suppressStatusToggle = true;
            try
            {
                WebsiteList.ItemsSource = CurrentPageItems().ToList();
            }
            finally
            {
                _suppressStatusToggle = false;
            }

            TotalCountText.Text = string.Format(Res.Get("SitesTotalFormat"), filteredCount);
            JumpPageTextBox.Text = _currentPage.ToString();
            PrevPageButton.IsEnabled = _currentPage > 1;
            NextPageButton.IsEnabled = _currentPage < _totalPages;
            RebuildPageNumbers();
            UpdateSelectionUi();
        }

        private void StatusToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (_suppressStatusToggle || !IsLoaded)
            {
                return;
            }

            if (sender is ToggleSwitch { Tag: WebsiteItem site } toggle)
            {
                if (toggle.IsOn)
                {
                    ShowStatusFeedback(
                        Res.Get("SitesStartedTitle"),
                        string.Format(Res.Get("SitesStartedMessage"), site.Name));
                }
                else
                {
                    ShowStatusFeedback(
                        Res.Get("SitesStoppedTitle"),
                        string.Format(Res.Get("SitesStoppedMessage"), site.Name));
                }
            }

            if (CategoryFilterComboBox?.SelectedItem is ComboBoxItem { Tag: string tag }
                && (tag == "running" || tag == "stopped"))
            {
                RefreshPage();
            }
        }

        private static void ShowStatusFeedback(string title, string message)
        {
            ToastService.Show(title, message);
        }

        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            _currentPage = 1;
            RefreshPage();
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            {
                return;
            }

            _currentPage = 1;
            RefreshPage();
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _currentPage = 1;
            RefreshPage();
        }

        private void RebuildPageNumbers()
        {
            PageNumbersPanel.Children.Clear();

            foreach (var page in GetVisiblePages())
            {
                if (page < 0)
                {
                    PageNumbersPanel.Children.Add(new TextBlock
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

                if (pageNumber == _currentPage
                    && Application.Current.Resources.ContainsKey("SoftAccentButtonStyle"))
                {
                    button.Style = (Style)Application.Current.Resources["SoftAccentButtonStyle"];
                }

                button.Click += PageNumberButton_Click;
                PageNumbersPanel.Children.Add(button);
            }
        }

        private IEnumerable<int> GetVisiblePages()
        {
            if (_totalPages <= 7)
            {
                for (var i = 1; i <= _totalPages; i++)
                {
                    yield return i;
                }

                yield break;
            }

            yield return 1;

            var start = Math.Max(2, _currentPage - 1);
            var end = Math.Min(_totalPages - 1, _currentPage + 1);

            if (start > 2)
            {
                yield return -1;
            }

            for (var i = start; i <= end; i++)
            {
                yield return i;
            }

            if (end < _totalPages - 1)
            {
                yield return -1;
            }

            yield return _totalPages;
        }

        private void PageNumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not int page)
            {
                return;
            }

            _currentPage = page;
            RefreshPage();
        }

        private void PageSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressPageSizeEvent || PageSizeComboBox.SelectedItem is not ComboBoxItem item)
            {
                return;
            }

            if (!int.TryParse(item.Tag?.ToString(), out var size) || size <= 0)
            {
                return;
            }

            _pageSize = size;
            _currentPage = 1;
            RefreshPage();
        }

        private void JumpPageTextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key != Windows.System.VirtualKey.Enter)
            {
                return;
            }

            if (!int.TryParse(JumpPageTextBox.Text, out var page))
            {
                JumpPageTextBox.Text = _currentPage.ToString();
                return;
            }

            page = Math.Clamp(page, 1, _totalPages);
            _currentPage = page;
            RefreshPage();
        }

        private void UpdateSelectionUi()
        {
            var pageItems = CurrentPageItems().ToList();
            var selectedOnPage = pageItems.Count(item => item.IsSelected);
            var selectedTotal = _allItems.Count(item => item.IsSelected);

            _suppressSelectionEvents = true;
            bool? state;
            if (pageItems.Count == 0 || selectedOnPage == 0)
            {
                state = false;
            }
            else if (selectedOnPage == pageItems.Count)
            {
                state = true;
            }
            else
            {
                state = null;
            }

            HeaderSelectAllCheckBox.IsChecked = state;
            SelectAllCheckBox.IsChecked = state;
            _suppressSelectionEvents = false;

            BatchActionButton.Content = string.Format(
                Res.Get("SitesBatchSelectedFormat"),
                selectedTotal);
            BatchActionButton.IsEnabled = selectedTotal > 0 && BatchActionComboBox.SelectedItem != null;
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (_suppressSelectionEvents)
            {
                return;
            }

            var source = sender as CheckBox;
            var selectAll = source?.IsChecked == true;
            foreach (var item in CurrentPageItems())
            {
                item.IsSelected = selectAll;
            }

            UpdateSelectionUi();
        }

        private void ItemCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (_suppressSelectionEvents)
            {
                return;
            }

            UpdateSelectionUi();
        }

        private void BatchActionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectionUi();
        }

        private void BatchActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (BatchActionComboBox.SelectedItem is not ComboBoxItem selected
                || selected.Tag is not string action)
            {
                return;
            }

            var items = GetSelectedItems();
            if (items.Count == 0)
            {
                return;
            }

            if (action == "start")
            {
                foreach (var item in items)
                {
                    item.Status = WebsiteItem.StatusRunning;
                }
            }
            else if (action == "stop")
            {
                foreach (var item in items)
                {
                    item.Status = WebsiteItem.StatusStopped;
                }
            }
            else if (action.StartsWith("php:", StringComparison.Ordinal))
            {
                var version = action["php:".Length..];
                foreach (var item in items)
                {
                    item.PhpVersion = version;
                }
            }
            else if (action == "delete")
            {
                foreach (var item in items)
                {
                    _allItems.Remove(item);
                }
            }

            RefreshPage();
        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage <= 1)
            {
                return;
            }

            _currentPage--;
            RefreshPage();
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage >= _totalPages)
            {
                return;
            }

            _currentPage++;
            RefreshPage();
        }
    }
}
