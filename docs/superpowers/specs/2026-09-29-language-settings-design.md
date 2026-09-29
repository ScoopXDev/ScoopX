# ScoopX — 语言切换（设置页先行）设计

**日期：** 2026-09-29  
**状态：** 待用户审阅  
**范围：** 设置内简体中文 / English 切换并持久化；本轮仅翻译设置页文案；其它页面仍中文硬编码

## 背景

设置页已有外观 / 常规 / 路径 / 关于。产品需要语言切换，但全应用尚无 `.resw` / `x:Uid`。本轮先搭标准本地化骨架，并只覆盖设置页。

## 目标

1. 设置中可选 **简体中文** / **English**，写入 `LocalSettings`，启动生效。
2. 采用 WinUI 标准 `Strings/<lang>/Resources.resw` + `ApplicationLanguages.PrimaryLanguageOverride`。
3. 本轮设置页可见文案（分区标题、开关标题/说明、路径标签、关于、语言区自身）可切换。
4. 切换语言后：**提示重启应用**（最稳；避免半切状态）。

## 非目标

- 本轮不翻译：侧栏、首页、网站页、对话框、托盘菜单、标题栏 slogan。
- 不做「跟随系统」第三档。
- 不做运行时无重启的全页热切换（后续可加）。

## 方案

### 1. 语言偏好

| 项 | 值 |
|----|-----|
| 存储键 | `AppLanguage`（`AppSettings` 或独立 `LanguageService`） |
| 取值 | `zh-CN`、`en-US` |
| 默认 | `zh-CN`（与当前产品默认中文一致） |
| 启动 | `App()` 尽早：`ApplicationLanguages.PrimaryLanguageOverride = saved` |

推荐新建 `Services/LanguageService.cs`（与 `ThemeService` 同级），避免把语言塞进主题服务；`AppSettings` 亦可只存键，但独立服务更清晰。

API 草案：

- `string CurrentTag { get; }`（`zh-CN` / `en-US`）
- `void InitializeBeforeUi()` — 读设置并设 `PrimaryLanguageOverride`
- `void SetLanguage(string tag)` — 写设置 + 设 Override；返回后由 UI 提示重启
- 常量：`ZhCn = "zh-CN"`、`EnUs = "en-US"`

### 2. 资源文件

```
Strings/
  zh-CN/Resources.resw
  en-US/Resources.resw
```

键命名示例（`x:Uid` 对应）：

| 键 | zh-CN | en-US |
|----|-------|-------|
| SettingsPage.Title | 设置 | Settings |
| Settings.Appearance.Header | 外观设置 | Appearance |
| Settings.Theme.Light | 浅色模式 | Light |
| Settings.Theme.Dark | 深色模式 | Dark |
| Settings.Theme.System | 跟随系统 | System |
| Settings.General.Header | 常规设置 | General |
| Settings.CloseToTray.Title | 关闭时最小化到托盘 | Minimize to tray on close |
| Settings.CloseToTray.Desc | … | … |
| Settings.StartWithWindows.Title | 开机时启动 | Start with Windows |
| Settings.StartWithWindows.Desc | … | … |
| Settings.StartHidden.Title | 启动时隐藏到托盘 | Hide to tray on startup |
| Settings.StartHidden.Desc | … | … |
| Settings.Paths.Header | 路径设置 | Paths |
| Settings.ScoopRoot.Label | Scoop 根目录 | Scoop root |
| Settings.WebsitesRoot.Label | 网站默认根目录 | Default websites root |
| Settings.Browse | 浏览 | Browse |
| Settings.About.Header | 关于 | About |
| Settings.GitHub | GitHub 仓库 | GitHub |
| Settings.Language.Header | 语言设置 | Language |
| Settings.Language.Zh | 简体中文 | 简体中文 |
| Settings.Language.En | English | English |
| Settings.Language.RestartTitle | 切换语言 | Change language |
| Settings.Language.RestartMessage | 语言将在重启应用后生效，是否立即重启？ | Language applies after restart. Restart now? |
| Settings.Language.RestartNow | 立即重启 | Restart |
| Settings.Language.Later | 稍后 | Later |

版本号行仍由 code-behind 拼：`版本 {0}` / `Version {0}`（资源格式串）。

### 3. 设置页 UI

在「外观」与「常规」之间（或「常规」之前）新增 **语言设置** Surface 卡片：

- 两选项：简体中文 / English（可用 `RadioButton` 组，或与主题类似的两按钮；推荐 **RadioButton 横向**，比再做一套预览卡更轻）
- 选中项反映 `LanguageService.CurrentTag`
- 变更 → `SetLanguage` → `ContentDialog` 询问是否重启
  - 立即重启：写设置后 `Microsoft.Windows.AppLifecycle.AppInstance` 重启或简单 `Process` 再启动 + 退出（WinUI 常用：保存后 `Application.Current.Exit()` 并依赖用户手动开；更优：`Windows.ApplicationModel.Core` / 启动自身 exe 再 Exit）
  - 稍后：仅保存，当前 UI 仍为旧语言直至重启

本轮采用：**保存 + 对话框「立即重启 / 稍后」**；立即重启用启动当前 exe 新进程再 `ExitApplication`（与托盘退出一致清理）。

### 4. 应用接线

| 文件 | 改动 |
|------|------|
| `Services/LanguageService.cs` | 新建 |
| `App.xaml.cs` | `InitializeBeforeUi` 在 `InitializeComponent` 前或紧后尽早调用 |
| `Strings/zh-CN/Resources.resw` | 新建 |
| `Strings/en-US/Resources.resw` | 新建 |
| `ScoopX.csproj` | 如需显式包含 PRI（SDK 风格通常自动） |
| `Pages/SettingsPage.xaml` | 语言区 + 关键文案 `x:Uid` |
| `Pages/SettingsPage.xaml.cs` | 语言选择、重启对话框、版本格式串 |

### 5. 验收

- 默认中文；设置页为中文。
- 选 English → 确认重启 → 重启后设置页为英文，选项仍为 English。
- 选「稍后」→ 不重启则界面仍中文，但下次启动为英文。
- 其它页面本轮仍可为中文（已知限制）。
- 浅色/深色下语言区布局正常。

## 已确认决策

- 范围：A（设置入口 + 本轮仅设置页翻译）
- 实现：方案 1（resw + LanguageService + 重启生效）
- 选项：仅简体中文 / English，无跟随系统
