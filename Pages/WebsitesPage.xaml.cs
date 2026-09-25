using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

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

        public bool IsRunning
        {
            get => _status == "运行中";
            set => Status = value ? "运行中" : "已停止";
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
        private readonly DispatcherTimer _feedbackTimer = new() { Interval = TimeSpan.FromSeconds(2.4) };

        public WebsitesPage()
        {
            InitializeComponent();
            _feedbackTimer.Tick += (_, _) =>
            {
                _feedbackTimer.Stop();
                StatusFeedbackBar.IsOpen = false;
            };
            _suppressPageSizeEvent = false;
            if (PageSizeComboBox.SelectedItem is ComboBoxItem selected
                && int.TryParse(selected.Tag?.ToString(), out var size)
                && size > 0)
            {
                _pageSize = size;
            }

            RefreshPage();
        }

        private static List<WebsiteItem> CreateDemoData()
        {
            return new List<WebsiteItem>
            {
                new() { Name = "example.com", Status = "运行中", RootPath = @"C:\www\example.com", RunPath = @"C:\www\example.com", Remark = "主站", PhpVersion = "8.2" },
                new() { Name = "blog.local", Status = "运行中", RootPath = @"C:\www\blog", RunPath = @"C:\www\blog", Remark = "博客", PhpVersion = "8.1" },
                new() { Name = "shop.test", Status = "已停止", RootPath = @"C:\www\shop", RunPath = @"C:\www\shop\public", Remark = "商城测试", PhpVersion = "8.0" },
                new() { Name = "api.dev", Status = "运行中", RootPath = @"C:\www\api", RunPath = @"C:\www\api\public", Remark = "接口服务", PhpVersion = "8.3" },
                new() { Name = "docs.local", Status = "运行中", RootPath = @"C:\www\docs", RunPath = @"C:\www\docs", Remark = "文档站", PhpVersion = "8.2" },
                new() { Name = "admin.panel", Status = "已停止", RootPath = @"C:\www\admin", RunPath = @"C:\www\admin", Remark = "后台", PhpVersion = "7.4" },
                new() { Name = "cdn.assets", Status = "运行中", RootPath = @"C:\www\cdn", RunPath = @"C:\www\cdn", Remark = "静态资源", PhpVersion = "8.1" },
                new() { Name = "forum.local", Status = "运行中", RootPath = @"C:\www\forum", RunPath = @"C:\www\forum", Remark = "论坛", PhpVersion = "8.2" },
                new() { Name = "mail.web", Status = "已停止", RootPath = @"C:\www\mail", RunPath = @"C:\www\mail", Remark = "邮箱面板", PhpVersion = "8.0" },
                new() { Name = "demo.site", Status = "运行中", RootPath = @"C:\www\demo", RunPath = @"C:\www\demo\public", Remark = "演示环境", PhpVersion = "8.3" },
                new() { Name = "old.legacy", Status = "已停止", RootPath = @"C:\www\legacy", RunPath = @"C:\www\legacy", Remark = "旧项目", PhpVersion = "7.4" },
                new() { Name = "portal.app", Status = "运行中", RootPath = @"C:\www\portal", RunPath = @"C:\www\portal", Remark = "门户", PhpVersion = "8.2" },
            };
        }

        private async void AddSiteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddSiteDialog
            {
                XamlRoot = XamlRoot,
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Secondary || dialog.CreatedSites.Count == 0)
            {
                return;
            }

            _allItems.InsertRange(0, dialog.CreatedSites);
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
                    Title = "无法打开",
                    Content = $"目录不存在：\n{path}",
                    CloseButtonText = "确定",
                    XamlRoot = XamlRoot,
                };
                await missingDialog.ShowAsync();
                return;
            }

            await Launcher.LaunchFolderPathAsync(path);
        }

        private IEnumerable<WebsiteItem> GetFilteredItems()
        {
            IEnumerable<WebsiteItem> query = _allItems;

            if (CategoryFilterComboBox?.SelectedItem is ComboBoxItem category
                && category.Tag is string tag
                && tag != "all")
            {
                query = tag switch
                {
                    "running" => query.Where(item => item.Status == "运行中"),
                    "stopped" => query.Where(item => item.Status == "已停止"),
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

            TotalCountText.Text = $"共 {filteredCount} 条";
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
                    ShowStatusFeedback("已启动", $"站点 {site.Name} 已启动", InfoBarSeverity.Success);
                }
                else
                {
                    ShowStatusFeedback("已停止", $"站点 {site.Name} 已停止", InfoBarSeverity.Informational);
                }
            }

            if (CategoryFilterComboBox?.SelectedItem is ComboBoxItem { Tag: string tag }
                && (tag == "running" || tag == "stopped"))
            {
                RefreshPage();
            }
        }

        private void ShowStatusFeedback(string title, string message, InfoBarSeverity severity)
        {
            StatusFeedbackBar.Title = title;
            StatusFeedbackBar.Message = message;
            StatusFeedbackBar.Severity = severity;
            StatusFeedbackBar.IsOpen = true;
            _feedbackTimer.Stop();
            _feedbackTimer.Start();
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
                var button = new Button
                {
                    Content = pageNumber.ToString(),
                    MinWidth = 32,
                    Padding = new Thickness(8, 4, 8, 4),
                    Tag = pageNumber,
                };

                if (pageNumber == _currentPage
                    && Application.Current.Resources.ContainsKey("AccentButtonStyle"))
                {
                    button.Style = (Style)Application.Current.Resources["AccentButtonStyle"];
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

            BatchActionButton.Content = $"批量操作 (已选中{selectedTotal}项)";
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
                    item.Status = "运行中";
                }
            }
            else if (action == "stop")
            {
                foreach (var item in items)
                {
                    item.Status = "已停止";
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
