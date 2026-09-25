<div align="center">
  <img src="Assets/ScoopX-icon.png" alt="ScoopX Logo" width="112" height="112" />

  # ScoopX

  **现代化 Windows 开发环境管理工具**

  基于 Scoop，为 PHP 开发者提供统一、直观的开发环境管理体验。

  [![Windows 11](https://img.shields.io/badge/Windows-11-0078D4?logo=windows11&logoColor=white)](https://www.microsoft.com/windows/windows-11)
  [![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
  [![WinUI 3](https://img.shields.io/badge/WinUI-3-0078D4)](https://learn.microsoft.com/windows/apps/winui/winui3/)
  [![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
</div>

## 简介

**ScoopX** 是基于 [Scoop](https://scoop.sh/) 的 Windows 桌面开发环境管理工具，采用 WinUI 3 构建原生界面，将 PHP 版本管理、配置管理及常用开发工具管理整合至统一的操作界面，减少重复的命令行操作。

## 主要功能

- **PHP 版本管理**：集中管理多个 PHP 版本及命令行默认版本。
- **PHP 配置与扩展**：管理各版本 `php.ini`、相关环境变量和常用扩展。
- **开发工具管理**：统一管理 MySQL、Redis、Node.js、Composer 等开发工具。
- **Scoop 图形化操作**：通过图形界面完成软件搜索、安装、更新与卸载。
- **Windows 原生界面**：遵循 Windows 11 的界面与交互设计。

> 具体可用的软件版本、扩展和安装方式取决于 Scoop 软件源及相关软件的兼容性。

## 技术栈

| 项目 | 技术 |
| --- | --- |
| 桌面框架 | WinUI 3 / Windows App SDK |
| 开发语言 | C# |
| 运行框架 | .NET 10 |
| 包管理 | Scoop |
| 目标平台 | Windows 11 |
| 开发环境 | Visual Studio |

## 安装与使用

在仓库的 [Releases](../../releases) 页面查看可用版本和对应的安装说明。安装后，按照应用界面提示配置 Scoop 及所需的开发工具。

## 从源码构建

在 Windows 11 上安装 Visual Studio、.NET 10 SDK 和项目所需的 Windows App SDK / WinUI 开发组件。克隆本仓库后，使用 Visual Studio 打开解决方案，选择目标架构并编译运行。

## 参与贡献

欢迎通过 [Issues](../../issues) 反馈问题或提出建议，也欢迎提交 Pull Request。提交前请确保修改与项目现有架构及代码风格保持一致。

## 开源许可

ScoopX 采用 [MIT License](LICENSE)。Scoop 及其他第三方依赖遵循各自的开源许可证。

<div align="center">
  <sub>© ScoopX</sub>
</div>
