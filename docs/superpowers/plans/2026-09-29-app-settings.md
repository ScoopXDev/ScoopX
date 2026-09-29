# App Settings Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 补齐设置页「常规 / 路径 / 关于」，用 `AppSettings` + LocalSettings 持久化，并接线关闭到托盘、开机自启、启动隐藏。

**Architecture:** `ThemeService` 继续只管主题；新建单例 `AppSettings` 读写其余键。`MainWindow` / `App` 读设置改变关闭与启动行为；`Package.appxmanifest` 声明 StartupTask。设置页分区卡片视觉与现有外观区一致。

**Tech Stack:** WinUI 3, Windows App SDK, `ApplicationData.LocalSettings`, `Windows.ApplicationModel.StartupTask`, `FolderPicker`, MSIX

## Global Constraints

- 不改主题预览交互；外观区保持现状。
- 路径本轮只持久化，不改 `AddSiteDialog` 预填。
- unpackaged 下 StartupTask 失败：提示、不崩溃。
- 用户未要求时不自动 `git commit`。
- 验证：`dotnet build ScoopX.csproj -c Debug -p:Platform=x64`

## File Structure

| 文件 | 职责 |
|------|------|
| `Services/AppSettings.cs` | 新建：CloseToTray / StartWithWindows / StartHiddenToTray / ScoopRootPath / WebsitesRootPath |
| `Package.appxmanifest` | 增加 `windows.startupTask` |
| `Pages/SettingsPage.xaml` | 常规 / 路径 / 关于 UI |
| `Pages/SettingsPage.xaml.cs` | Toggle、浏览、版本号接线 |
| `MainWindow.xaml.cs` | 关闭行为读 CloseToTray；启动隐藏 |
| `App.xaml.cs` | 如需协调启动隐藏时机（优先在 MainWindow 末尾） |

---

### Task 1: AppSettings 服务

**Files:**
- Create: `Services/AppSettings.cs`

**Interfaces:**
- Produces: `AppSettings.Instance` with:
  - `bool CloseToTray { get; set; }` 默认 `true`
  - `bool StartWithWindows { get; set; }` 默认 `false`；set 时尝试同步 StartupTask
  - `bool StartHiddenToTray { get; set; }` 默认 `false`
  - `string ScoopRootPath { get; set; }` 默认 `""`
  - `string WebsitesRootPath { get; set; }` 默认 `""`
  - `event EventHandler? Changed`
  - `Task SetStartWithWindowsAsync(bool enabled)`（供 UI await；属性 set 可 fire-and-forget 调用）
  - `const string StartupTaskId = "ScoopXStartup"`

- [ ] **Step 1: 创建 `Services/AppSettings.cs`**

```csharp
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace ScoopX.Services
{
    public sealed class AppSettings
    {
        public const string StartupTaskId = "ScoopXStartup";
        private const string KeyCloseToTray = "CloseToTray";
        private const string KeyStartWithWindows = "StartWithWindows";
        private const string KeyStartHiddenToTray = "StartHiddenToTray";
        private const string KeyScoopRootPath = "ScoopRootPath";
        private const string KeyWebsitesRootPath = "WebsitesRootPath";

        private static readonly Lazy<AppSettings> Lazy = new(() => new AppSettings());
        public static AppSettings Instance => Lazy.Value;

        public event EventHandler? Changed;

        private AppSettings() { }

        public bool CloseToTray
        {
            get => ReadBool(KeyCloseToTray, defaultValue: true);
            set { WriteBool(KeyCloseToTray, value); RaiseChanged(); }
        }

        public bool StartHiddenToTray
        {
            get => ReadBool(KeyStartHiddenToTray, defaultValue: false);
            set { WriteBool(KeyStartHiddenToTray, value); RaiseChanged(); }
        }

        public string ScoopRootPath
        {
            get => ReadString(KeyScoopRootPath);
            set { WriteString(KeyScoopRootPath, value ?? ""); RaiseChanged(); }
        }

        public string WebsitesRootPath
        {
            get => ReadString(KeyWebsitesRootPath);
            set { WriteString(KeyWebsitesRootPath, value ?? ""); RaiseChanged(); }
        }

        public bool StartWithWindows
        {
            get => ReadBool(KeyStartWithWindows, defaultValue: false);
            set => _ = SetStartWithWindowsAsync(value);
        }

        public async Task<(bool ok, string? error)> SetStartWithWindowsAsync(bool enabled)
        {
            try
            {
                var task = await StartupTask.GetAsync(StartupTaskId);
                if (enabled)
                {
                    var state = await task.RequestEnableAsync();
                    if (state is StartupTaskState.Disabled
                        or StartupTaskState.DisabledByUser
                        or StartupTaskState.DisabledByPolicy)
                    {
                        WriteBool(KeyStartWithWindows, false);
                        RaiseChanged();
                        return (false, "未能启用开机自启（可能被系统或用户策略禁用）。");
                    }
                }
                else
                {
                    task.Disable();
                }

                WriteBool(KeyStartWithWindows, enabled);
                RaiseChanged();
                return (true, null);
            }
            catch (Exception ex)
            {
                // unpackaged / missing extension
                WriteBool(KeyStartWithWindows, false);
                RaiseChanged();
                return (false, $"开机自启不可用（需安装包版本）：{ex.Message}");
            }
        }

        private void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);

        private static bool ReadBool(string key, bool defaultValue)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var v) && v is bool b)
                return b;
            return defaultValue;
        }

        private static void WriteBool(string key, bool value) =>
            ApplicationData.Current.LocalSettings.Values[key] = value;

        private static string ReadString(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var v) && v is string s)
                return s;
            return "";
        }

        private static void WriteString(string key, string value) =>
            ApplicationData.Current.LocalSettings.Values[key] = value;
    }
}
```

- [ ] **Step 2: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`
Expected: 成功（0 Error）

---

### Task 2: Package.appxmanifest StartupTask

**Files:**
- Modify: `Package.appxmanifest`

**Interfaces:**
- Consumes: `AppSettings.StartupTaskId` = `"ScoopXStartup"`

- [ ] **Step 1: 在 `Application` 内增加 Extensions**

在 `</uap:VisualElements>` 之后、`</Application>` 之前加入（并在根 `Package` 增加 `xmlns:uap5` 与 IgnorableNamespaces）：

根元素：

```xml
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:mp="http://schemas.microsoft.com/appx/2014/phone/manifest"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:uap5="http://schemas.microsoft.com/appx/manifest/uap/windows10/5"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  xmlns:systemai="http://schemas.microsoft.com/appx/manifest/systemai/windows10"
  IgnorableNamespaces="uap uap5 rescap systemai">
```

Application 内：

```xml
      <Extensions>
        <uap5:Extension Category="windows.startupTask">
          <uap5:StartupTask
            TaskId="ScoopXStartup"
            Enabled="false"
            DisplayName="ScoopX" />
        </uap5:Extension>
      </Extensions>
```

- [ ] **Step 2: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`
Expected: 成功

---

### Task 3: MainWindow / App 接线关闭与启动隐藏

**Files:**
- Modify: `MainWindow.xaml.cs`
- Modify: `App.xaml.cs`（仅当启动隐藏放在 OnLaunched 更清晰时）

**Interfaces:**
- Consumes: `AppSettings.Instance.CloseToTray`, `StartHiddenToTray`

- [ ] **Step 1: 改关闭逻辑**

`AppWindow_Closing`：

```csharp
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
```

`CloseButton_Click`：

```csharp
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
```

- [ ] **Step 2: 启动隐藏**

在 `MainWindow` 构造函数末尾（`NavigateTo("Home")` 之后）：

```csharp
if (AppSettings.Instance.StartHiddenToTray)
{
    // 延迟到窗口激活后再藏，避免闪一下又立刻 Hide 竞态：用 Activated 一次性
    void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        Activated -= OnActivated;
        HideToTray();
    }
    Activated += OnActivated;
}
```

（若 `Activated` 在已激活后不再触发，改为 `DispatcherQueue.TryEnqueue` 在 Loaded/首帧 Hide。）

- [ ] **Step 3: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`
Expected: 成功

---

### Task 4: SettingsPage UI + 接线

**Files:**
- Modify: `Pages/SettingsPage.xaml`
- Modify: `Pages/SettingsPage.xaml.cs`

**Interfaces:**
- Consumes: `AppSettings.Instance` 全部属性与 `SetStartWithWindowsAsync`
- Consumes: 现有主题区不变

- [ ] **Step 1: XAML — 在外观卡片后追加分区**

结构（放在现有外观 `Border` 之后、外层 `StackPanel` 内）；`MaxWidth` 改为 `640`：

```xml
<!-- 常规 -->
<TextBlock Text="常规" FontSize="13" FontWeight="SemiBold"
           Foreground="{ThemeResource ScoopTextSecondaryBrush}" Margin="0,8,0,0" />
<Border Background="{ThemeResource ScoopSurfaceFillBrush}" BorderThickness="0"
        CornerRadius="10" Padding="16,8">
  <StackPanel>
    <!-- 每行：Grid 左标题+说明，右 ToggleSwitch；三行：CloseToTray / StartWithWindows / StartHiddenToTray -->
  </StackPanel>
</Border>

<!-- 路径 -->
<TextBlock Text="路径" ... />
<Border ... Padding="16,14">
  <!-- Scoop 根目录：TextBox + 浏览按钮 -->
  <!-- 网站默认根目录：同上 -->
</Border>

<!-- 关于 -->
<TextBlock Text="关于" ... />
<Border ... Padding="16,14">
  <StackPanel Spacing="6">
    <TextBlock Text="ScoopX" FontSize="15" FontWeight="SemiBold"
               Foreground="{ThemeResource ScoopTextPrimaryBrush}" />
    <TextBlock x:Name="AboutVersionText" FontSize="12"
               Foreground="{ThemeResource ScoopTextSecondaryBrush}" />
  </StackPanel>
</Border>
```

Toggle 命名：`CloseToTrayToggle`、`StartWithWindowsToggle`、`StartHiddenToggle`  
路径：`ScoopRootTextBox`、`WebsitesRootTextBox`、`BrowseScoopRootButton`、`BrowseWebsitesRootButton`

行内文案：
- 关闭时最小化到托盘 / 关闭窗口时隐藏到系统托盘，而不是退出
- 开机时启动 / 登录 Windows 后自动启动 ScoopX
- 启动时隐藏到托盘 / 启动后不显示主窗口，仅显示托盘图标

- [ ] **Step 2: code-behind 接线**

构造函数：加载开关与路径；绑定 `Toggled` / `Click`；写 `AboutVersionText`：

```csharp
private static string GetVersionString()
{
    try
    {
        var v = Package.Current.Id.Version;
        return $"版本 {v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
    }
    catch
    {
        var v = typeof(App).Assembly.GetName().Version;
        return v is null ? "版本 1.0.0" : $"版本 {v.Major}.{v.Minor}.{v.Build}";
    }
}
```

`StartWithWindowsToggle`：`await AppSettings.Instance.SetStartWithWindowsAsync(...)`；失败时用 `ContentDialog` 提示，并把 Toggle 拨回 `AppSettings` 实际值。

路径浏览：复用 EditSiteDialog 的 `FolderPicker` + `InitializeWithWindow` 模式。

注意：加载时设 Toggle 会触发 `Toggled`——用 `_suppressToggle` 标志跳过。

- [ ] **Step 3: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`
Expected: 成功

- [ ] **Step 4: 手动验收清单**

1. 关「关闭到托盘」→ 点 X 应退出进程  
2. 开「关闭到托盘」→ 点 X 进托盘，托盘「退出」可退出  
3. 路径浏览保存后重启仍在  
4. 关于显示版本  
5. 浅色/深色下新分区可读  

---

## Spec coverage (self-review)

| Spec 项 | Task |
|---------|------|
| CloseToTray | 1, 3, 4 |
| StartWithWindows + StartupTask | 1, 2, 4 |
| StartHiddenToTray | 1, 3, 4 |
| ScoopRootPath / WebsitesRootPath | 1, 4 |
| 关于版本 | 4 |
| 路径不接 AddSiteDialog | 遵守 |
| unpackaged 提示 | 1, 4 |

无 TBD/占位符；属性名与 Task 4 一致。
