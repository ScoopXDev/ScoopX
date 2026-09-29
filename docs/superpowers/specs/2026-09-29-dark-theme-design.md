# ScoopX 浅/深色主题设计

**日期：** 2026-09-29  
**状态：** 已审阅通过  
**范围：** 跟随系统 + 设置页可选浅色 / 深色 / 跟随系统；真正的双主题视觉（非锁定浅色）

## 背景

当前 UI 画布（氛围渐变、Acrylic 毛玻璃、大量硬编码 `#` 色）按浅色设计，但应用未设置 `RequestedTheme`，会跟随 Windows。系统切到暗色后，Fluent `ThemeResource`（如 `TextFillColorPrimaryBrush`、ToggleSwitch）变为浅色前景，叠在仍为浅色的自绘背景上，出现标题不可读、路径发白、开关拇指发黑等问题。

## 目标

1. Windows 暗色 / 浅色下界面整体协调可读。
2. 设置中可选：**跟随系统 / 浅色 / 深色**，立即生效并持久化。
3. 保留现有「Toolbox 式浅色毛玻璃」品牌感；暗色提供对应深色氛围玻璃，而非纯系统默认灰。

## 非目标

- 不改应用图标 / unplated 资源。
- 不做自定义强调色盘、高对比主题。
- Database / Store 页面仍可为空，仅保证导航在深色下可读。

## 方案概要

采用 **自有 `Scoop*` 双套色**（`ThemeDictionaries` Light / Dark），页面硬编码色改为 `ThemeResource`。主题模式由 `ThemeService` 管理，默认 `System`。

## 1. 主题资源（`App.xaml`）

将下列资源从全局单例移入（或在）`ResourceDictionary.ThemeDictionaries` 的 `Light` / `Dark` 各一份；页面用 `{ThemeResource ...}` 引用。

| Key | Light | Dark（方向） |
|-----|-------|----------------|
| `ScoopAmbientGradient` | 现有蓝紫粉浅渐变 | 深蓝 / 深紫 / 暗粉低饱和氛围 |
| `ScoopGlassAcrylicBrush` | 白 Tint（现有） | 深灰 Tint（约 `#1C1C1E`） |
| `ScoopGlassStrongAcrylicBrush` | 更实白玻璃 | 更深卡片玻璃 |
| `ScoopGlassBorderBrush` | 半透明白边 | 半透明浅边 |
| `ScoopTextPrimaryBrush` | `#111827` | `#F3F4F6` |
| `ScoopTextSecondaryBrush` | `#6B7280` | `#9CA3AF` |
| `ScoopTextTertiaryBrush` | `#9CA3AF` | `#6B7280` |
| `ScoopCardFillBrush` | `#FFFFFF` / 首页灰底 `#F7F7F7` 可拆 `ScoopSurfaceFillBrush` | `#2A2A2E` 级 |
| `ScoopNavInactiveBrush` | `#4B5563` | `#9CA3AF` |
| `SoftInputFillBrush` | `#F2F3F5` | 深色输入底 |
| 对话框标签/提示 Style | 现有浅色 | 对应深色 Foreground |

强调色字典已有 Light/Dark（`#2563EB` / `#3B82F6`），保持不变。

`ScoopGlassCardStyle`、`DialogFieldLabelStyle`、`DialogFieldHintStyle`、`SoftCancelButtonStyle` 等改为引用上述 ThemeResource，避免写死 `#`。

主窗口光斑（`MainWindow` 内 Ellipse RadialGradient）同步提供暗色更低透明度版本，或抽成主题资源键。

## 2. ThemeService

新建服务（建议路径：`Services/ThemeService.cs`）：

- 枚举 / 字符串：`System` | `Light` | `Dark`
- 存储：`ApplicationData.Current.LocalSettings`，键 `AppTheme`，默认 `System`
- API 草案：
  - `AppTheme Preference { get; }`
  - `void SetPreference(AppTheme theme)` — 写设置并 `Apply()`
  - `void Apply()` — 根据偏好设置 `Application.Current.RequestedTheme`；`System` 时映射当前系统主题
  - 在 `App` 启动早期调用 `Apply()`
- 「跟随系统」时监听 `UISettings.ColorValuesChanged`（或等价），偏好仍为 `System` 时重新 `Apply()`

注意：WinUI 中部分场景改 `Application.RequestedTheme` 需在窗口创建前或配合根元素 `RequestedTheme`；实现时以「切换后主窗口与 ContentDialog 立即一致」为准，必要时同步设置 `MainWindow` 内容根的 `RequestedTheme`。

## 3. 设置页

当前侧栏「设置」导航会清空 `ContentFrame`，无独立页面。

- 新增 `Pages/SettingsPage.xaml`（+ code-behind）
- `MainWindow.NavigateTo("Settings")` → `ContentFrame.Navigate(typeof(SettingsPage))`
- UI：一块「外观」卡片 + 选项 **跟随系统 / 浅色 / 深色**（`ComboBox` 或等价单选）
- 变更 → `ThemeService.SetPreference`，立即生效

视觉使用与其他页相同的玻璃卡片 / `Scoop*` 文字色。

## 4. 页面改造清单

| 文件 | 改动 |
|------|------|
| `App.xaml` | ThemeDictionaries 扩展；Style 改 ThemeResource |
| `App.xaml.cs` | 启动时 `ThemeService.Apply()` |
| `MainWindow.xaml` | 渐变 `ThemeResource`；标题/副标题用 Scoop 文字刷；光斑主题化 |
| `MainWindow.xaml.cs` | `InactiveBrush` / 导航色改为可读主题刷（随主题刷新，避免静态 `SolidColorBrush` 锁死浅色灰） |
| `Pages/HomePage.xaml` | 卡片底、正文硬编码 → Scoop* |
| `Controls/StatusGauge.xaml` | 刻度/标签硬编码 → Scoop* / ThemeResource |
| `Pages/WebsitesPage.xaml` | 路径与「设置/删除」链接前景；表格文字；Toggle 随主题正常（背景修好后一般无需单独锁 Light） |
| `Pages/AddSiteDialog.xaml` / `EditSiteDialog.xaml` | `ContentDialogBackground` 等放入主题或改用 ThemeResource |
| `Pages/SettingsPage.*` | 新建 |
| `Services/ThemeService.cs` | 新建 |

`TableLinkButtonStyle`：`ContentPresenter` 传递 `Foreground`；链接默认 `Foreground="{ThemeResource AccentTextFillColorPrimaryBrush}"`（或 Scoop 链接色），避免内层 `TextBlock` 继承错误前景。

## 5. 验收标准

- Windows 浅色 + 偏好「跟随系统」：与当前浅色观感基本一致。
- Windows 暗色 + 偏好「跟随系统」：背景/玻璃变深，标题、路径、操作链接、Toggle 对比度正常。
- 偏好「浅色」：系统无论深浅，应用保持浅色。
- 偏好「深色」：系统无论深浅，应用保持深色。
- 重启应用后偏好仍生效。
- 设置页可打开并切换主题。

## 6. 风险与注意

- Acrylic 在部分环境下走 `FallbackColor`，Dark 的 Fallback 必须足够深。
- ContentDialog 资源常在对话框本地 `ResourceDictionary` 覆盖，需显式做 Light/Dark 或改用全局键。
- 导航色若在 code-behind 用静态 brush，主题切换后不会自动更新，需在 `Apply` 后刷新或改用 XAML ThemeResource。

## 已确认决策

- 方案：自有 `Scoop*` 双套色（非仅锁 Light、非纯 Fluent 灰）
- 触发：跟随系统 + 设置内 浅色 / 深色 / 跟随系统
- 默认：跟随系统
