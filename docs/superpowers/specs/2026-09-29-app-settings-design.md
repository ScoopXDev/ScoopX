# ScoopX — 应用设置补齐设计

**日期：** 2026-09-29  
**状态：** 待用户审阅  
**范围：** 设置页在现有「外观」之外，补齐「常规 / 路径 / 关于」；统一用 `AppSettings` + `LocalSettings` 持久化

## 背景

当前 `SettingsPage` 仅有主题三选一。应用已具备托盘隐藏、网站管理、Scoop/PHP 工具方向，缺少对应的常规行为与路径配置入口。

## 目标

1. 设置页分区完整：**外观 / 常规 / 路径 / 关于**。
2. 开关与路径立即写入并尽量立即生效；路径供后续业务读取。
3. 视觉延续现有 Surface 底板卡片风格（无描边）。

## 非目标

- 不实现 Scoop 路径可用性校验、自动探测、PHP 版本联动。
- 不做多语言切换、代理、更新通道等高级项。
- 不改主题预览交互。

## 方案

采用 **单一 `AppSettings` 服务**（与 `ThemeService` 并列），主题仍由 `ThemeService` 管理；其余键统一进 `AppSettings`。

## 1. 设置项与存储

| 分组 | 项 | 键 | 类型 | 默认 | 行为 |
|------|----|----|------|------|------|
| 外观 | 主题 | `AppTheme`（现有） | enum | System | `ThemeService` |
| 常规 | 关闭时最小化到托盘 | `CloseToTray` | bool | `true` | `true`：关闭/点 X → 藏托盘（现有）；`false`：真正退出 |
| 常规 | 开机自启 | `StartWithWindows` | bool | `false` | 切换 MSIX `StartupTask` 状态 |
| 常规 | 启动时隐藏到托盘 | `StartHiddenToTray` | bool | `false` | 启动后若不显示窗口则直接 `Hide`；仅在「开机自启」或冷启动时有意义，设置页仍可独立开关 |
| 路径 | Scoop 根目录 | `ScoopRootPath` | string | `""` | 文本 +「浏览」选文件夹；空表示未配置 |
| 路径 | 网站默认根目录 | `WebsitesRootPath` | string | `""` | 同上；后续「添加网站」可预填 |
| 关于 | — | — | — | — | 只读展示，无存储 |

`AppSettings` API 草案：

- `bool CloseToTray { get; set; }`
- `bool StartWithWindows { get; set; }`（set 时同步 StartupTask）
- `bool StartHiddenToTray { get; set; }`
- `string ScoopRootPath { get; set; }`
- `string WebsitesRootPath { get; set; }`
- `event EventHandler? Changed`（可选，MainWindow 监听关闭行为）

读写：`ApplicationData.Current.LocalSettings`。

## 2. 行为接线

### 关闭 → 托盘 / 退出

`MainWindow` 的 `AppWindow_Closing`、`CloseButton_Click`：

- `CloseToTray == true` → 现有 `HideToTray()`
- `CloseToTray == false` → `ExitApplication()`

托盘菜单「退出」始终真正退出，不受该开关影响。

### 开机自启

- `Package.appxmanifest` 增加 `uap5:Extension` → `windows.startupTask`（TaskId 如 `ScoopXStartup`）。
- `AppSettings.StartWithWindows` setter：`StartupTask.GetAsync` → `RequestEnableAsync` / `Disable`。
- Debug unpackaged 运行时若 StartupTask 不可用：开关可点，失败时 Toast/对话框轻提示「需安装包版本」，不崩溃。

### 启动隐藏

- `App.OnLaunched` / `MainWindow` 构造末尾：若 `StartHiddenToTray`，则 `HideToTray()`（仍创建窗口与托盘，只是不显示）。
- 与「关闭到托盘」独立；默认关闭。

### 路径

- 设置页 TextBox 显示路径；「浏览」用 `FolderPicker`（与 EditSiteDialog 同类）。
- 本轮 **不** 改 `AddSiteDialog` 预填（可在后续一小步接 `WebsitesRootPath`）；本轮只保证可读写持久化，并在 `AppSettings` 上提供公开属性供后续调用。

## 3. 设置页 UI

`SettingsPage` 纵向分区（`MaxWidth` 可略放宽至 ~640）：

1. **外观设置** — 保持现有三预览卡片。
2. **常规** — Surface 卡片内若干行：标题 + 说明 + `ToggleSwitch`。
3. **路径** — Surface 卡片内两行：标签 + TextBox（只读或可编辑）+ 浏览按钮。
4. **关于** — Surface 卡片：应用名「ScoopX」、版本（读 `Package.Current.Id.Version`，unpackaged 回退程序集/`1.0.0`）、可选 GitHub 链接（`HyperlinkButton`，仓库 URL 用 README 同款或占位 `https://github.com/` 待填；若仓库地址未定时可只显示版本）。

行样式：左文案、右开关/按钮，与 WinUI Settings 常见布局接近，继续用 `ScoopTextPrimary/Secondary`。

## 4. 文件改动清单

| 文件 | 改动 |
|------|------|
| `Services/AppSettings.cs` | 新建 |
| `Package.appxmanifest` | 增加 StartupTask |
| `Pages/SettingsPage.xaml` / `.cs` | 常规 / 路径 / 关于 UI + 接线 |
| `MainWindow.xaml.cs` | 关闭行为读 `CloseToTray`；启动隐藏 |
| `App.xaml.cs` | 如需，启动隐藏时机协调 |

## 5. 验收标准

- 关闭到托盘开/关切换后，点窗口关闭行为立即符合设置。
- 开机自启在已打包安装环境下可启用/禁用；unpackaged 失败有提示、不崩。
- 启动隐藏开启后，下次启动只见托盘不见窗（可从托盘唤出）。
- 两个路径可浏览、保存、重启后仍在。
- 关于区显示可读版本号。
- 浅色/深色下新分区卡片对比度正常。

## 已确认决策

- 范围：B（常规 + 路径 + 关于）
- 实现：方案 1（单一 `AppSettings` + 设置页分区）
- 主题继续由 `ThemeService` 负责
