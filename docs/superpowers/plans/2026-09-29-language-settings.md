# Language Settings Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 设置页增加简体中文 / English 切换；用 `LanguageService` + `Resources.resw` 持久化并在启动生效；本轮仅翻译设置页。

**Architecture:** `LanguageService` 读写 `AppLanguage`（`zh-CN`/`en-US`），启动时设 `ApplicationLanguages.PrimaryLanguageOverride`。设置页文案用 `x:Uid` 绑定 resw；切换后对话框询问是否重启进程。

**Tech Stack:** WinUI 3, `Windows.Globalization.ApplicationLanguages`, `Windows.ApplicationModel.Resources`, `.resw`, LocalSettings

## Global Constraints

- 本轮只翻译设置页；侧栏/首页/网站/对话框/托盘保持中文硬编码。
- 仅 `zh-CN` / `en-US`，无跟随系统。
- 默认 `zh-CN`。
- 切换语言后需重启生效（对话框：立即重启 / 稍后）。
- 用户未要求时不 `git commit`。
- 验证：`dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`

## File Structure

| 文件 | 职责 |
|------|------|
| `Services/LanguageService.cs` | 偏好读写 + PrimaryLanguageOverride |
| `Strings/zh-CN/Resources.resw` | 中文 |
| `Strings/en-US/Resources.resw` | 英文 |
| `App.xaml.cs` | 启动尽早 Apply |
| `Pages/SettingsPage.xaml` | 语言区 + x:Uid |
| `Pages/SettingsPage.xaml.cs` | 选择、重启对话框、版本格式串 |

---

### Task 1: LanguageService

**Files:**
- Create: `Services/LanguageService.cs`

**Interfaces:**
- Produces:
  - `const string ZhCn = "zh-CN"`
  - `const string EnUs = "en-US"`
  - `const string SettingsKey = "AppLanguage"`
  - `string CurrentTag { get; }`
  - `void ApplyBeforeUi()` — 读 LocalSettings，非法则 `zh-CN`，设 `ApplicationLanguages.PrimaryLanguageOverride`
  - `void SetLanguage(string tag)` — 校验后写入并设 Override（不自动重启）

- [ ] **Step 1: 创建 LanguageService.cs**

```csharp
using System;
using Windows.Globalization;
using Windows.Storage;

namespace ScoopX.Services
{
    public sealed class LanguageService
    {
        public const string SettingsKey = "AppLanguage";
        public const string ZhCn = "zh-CN";
        public const string EnUs = "en-US";

        private static readonly Lazy<LanguageService> Lazy = new(() => new LanguageService());
        public static LanguageService Instance => Lazy.Value;

        public string CurrentTag { get; private set; } = ZhCn;

        private LanguageService() { }

        public void ApplyBeforeUi()
        {
            CurrentTag = ReadTag();
            ApplicationLanguages.PrimaryLanguageOverride = CurrentTag;
        }

        public void SetLanguage(string tag)
        {
            var normalized = Normalize(tag);
            CurrentTag = normalized;
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = normalized;
            ApplicationLanguages.PrimaryLanguageOverride = normalized;
        }

        private static string ReadTag()
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(SettingsKey, out var v)
                && v is string s)
            {
                return Normalize(s);
            }

            return ZhCn;
        }

        private static string Normalize(string tag)
        {
            if (string.Equals(tag, EnUs, StringComparison.OrdinalIgnoreCase)
                || string.Equals(tag, "en", StringComparison.OrdinalIgnoreCase))
            {
                return EnUs;
            }

            return ZhCn;
        }
    }
}
```

- [ ] **Step 2: App.xaml.cs — 在 Theme 之后、InitializeComponent 之前调用**

```csharp
ThemeService.Instance.ApplyApplicationThemeBeforeInit(this);
LanguageService.Instance.ApplyBeforeUi();
InitializeComponent();
```

- [ ] **Step 3: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`  
Expected: 0 Error

---

### Task 2: resw 资源

**Files:**
- Create: `Strings/zh-CN/Resources.resw`
- Create: `Strings/en-US/Resources.resw`

**Interfaces:**
- Produces: keys listed below (name → value). For `x:Uid="Foo"` on TextBlock, resw name is `Foo.Text`.

- [ ] **Step 1: 创建两侧 Resources.resw**（可用 Visual Studio resw XML 或手写；最少含下列键）

`zh-CN` 核心键：

| Name | Value |
|------|-------|
| SettingsTitle.Text | 设置 |
| SettingsAppearanceHeader.Text | 外观设置 |
| SettingsThemeLight.Text | 浅色模式 |
| SettingsThemeDark.Text | 深色模式 |
| SettingsThemeSystem.Text | 跟随系统 |
| SettingsLanguageHeader.Text | 语言设置 |
| SettingsLanguageZh.Content | 简体中文 |
| SettingsLanguageEn.Content | English |
| SettingsGeneralHeader.Text | 常规设置 |
| SettingsCloseToTrayTitle.Text | 关闭时最小化到托盘 |
| SettingsCloseToTrayDesc.Text | 关闭窗口时隐藏到系统托盘，而不是退出 |
| SettingsStartWithWindowsTitle.Text | 开机时启动 |
| SettingsStartWithWindowsDesc.Text | 登录 Windows 后自动启动 ScoopX |
| SettingsStartHiddenTitle.Text | 启动时隐藏到托盘 |
| SettingsStartHiddenDesc.Text | 启动后不显示主窗口，仅显示托盘图标 |
| SettingsPathsHeader.Text | 路径设置 |
| SettingsScoopRootLabel.Text | Scoop 根目录 |
| SettingsScoopRootPlaceholder.PlaceholderText | 选择 Scoop 根目录 |
| SettingsWebsitesRootLabel.Text | 网站默认根目录 |
| SettingsWebsitesRootPlaceholder.PlaceholderText | 选择网站默认根目录 |
| SettingsBrowse.Content | 浏览 |
| SettingsAboutHeader.Text | 关于 |
| SettingsGitHub.Content | GitHub 仓库 |
| SettingsVersionFormat | 版本 {0} |
| SettingsLanguageRestartTitle | 切换语言 |
| SettingsLanguageRestartMessage | 语言将在重启应用后生效。是否立即重启？ |
| SettingsLanguageRestartNow | 立即重启 |
| SettingsLanguageLater | 稍后 |

`en-US` 对应英文（Title→Settings，Appearance→Appearance，CloseToTrayDesc→Hide to the system tray instead of quitting，等；语言选项显示名保持「简体中文」/「English」）。

resw 文件头使用标准 ResX schema（`resheader` + `data` 节点）。若手写困难，可用最小合法 ResX 模板复制后只改 `data`。

- [ ] **Step 2: Build（确认 PRI 生成无报错）**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`  
Expected: 0 Error

---

### Task 3: SettingsPage UI — 语言区 + x:Uid

**Files:**
- Modify: `Pages/SettingsPage.xaml`
- Modify: `Pages/SettingsPage.xaml.cs`

**Interfaces:**
- Consumes: `LanguageService.Instance`、`ResourceLoader`（版本格式与对话框）

- [ ] **Step 1: 外观卡片后插入语言设置**

```xml
<!-- 语言设置 -->
<TextBlock x:Uid="SettingsLanguageHeader" FontSize="13" FontWeight="SemiBold"
           Foreground="{ThemeResource ScoopTextSecondaryBrush}" Margin="0,8,0,0" />
<Border Background="{ThemeResource ScoopSurfaceFillBrush}" BorderThickness="0"
        CornerRadius="10" Padding="16,12">
  <StackPanel Orientation="Horizontal" Spacing="24">
    <RadioButton x:Name="LanguageZhRadio" x:Uid="SettingsLanguageZh"
                 GroupName="AppLanguage" Tag="zh-CN"
                 Checked="LanguageRadio_Checked" />
    <RadioButton x:Name="LanguageEnRadio" x:Uid="SettingsLanguageEn"
                 GroupName="AppLanguage" Tag="en-US"
                 Checked="LanguageRadio_Checked" />
  </StackPanel>
</Border>
```

- [ ] **Step 2: 设置页其余可见中文改 x:Uid**

对标题、主题三标签、常规三行、路径标签/占位/浏览、关于标题、GitHub 按钮设置对应 `x:Uid`，**去掉**硬编码 `Text`/`Content`/`PlaceholderText`（由 resw 注入）。主题预览图内部装饰文案可保持硬编码（非用户说明文字）。

页面大标题：`x:Uid="SettingsTitle"`。

- [ ] **Step 3: code-behind**

加载时：

```csharp
_suppressLanguage = true;
try {
  if (LanguageService.Instance.CurrentTag == LanguageService.EnUs)
    LanguageEnRadio.IsChecked = true;
  else
    LanguageZhRadio.IsChecked = true;
} finally { _suppressLanguage = false; }
```

`LanguageRadio_Checked`：若 suppress 或 Tag 与 CurrentTag 相同则 return；否则 `SetLanguage(tag)`，然后弹 ContentDialog（资源串），Primary=立即重启：

```csharp
var path = Environment.ProcessPath;
if (!string.IsNullOrEmpty(path))
{
  System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
  {
    FileName = path,
    UseShellExecute = true
  });
}
if (App.MainWindow is MainWindow mw)
  // 调用现有退出：可通过公开方法或 Window.Close + tray dispose
```

若 `MainWindow.ExitApplication` 为 private：在 `MainWindow` 增加 `public void RequestExit()` 包装现有 `ExitApplication()`，设置页调用之；或 `Application.Current.Exit()` + 先 Dispose 托盘（优先 RequestExit 以清理托盘）。

版本：

```csharp
var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
var fmt = loader.GetString("SettingsVersionFormat");
AboutVersionText.Text = string.Format(fmt, versionDigits);
```

对话框文案同样 `ResourceLoader.GetString(...)`。

- [ ] **Step 4: Build**

Run: `dotnet build ScoopX.csproj -c Debug -p:Platform=x64 --nologo`  
Expected: 0 Error

- [ ] **Step 5: 手动验收**

1. 默认中文设置页  
2. 选 English → 立即重启 → 设置页英文  
3. 选稍后 → 下次启动才变  
4. 其它页仍可为中文  

---

## Spec coverage

| Spec | Task |
|------|------|
| LanguageService + LocalSettings | 1 |
| PrimaryLanguageOverride at startup | 1 |
| resw zh-CN / en-US | 2 |
| Settings language UI | 3 |
| Settings page strings localized | 3 |
| Restart dialog | 3 |
| No follow-system / no other pages | 遵守 |
