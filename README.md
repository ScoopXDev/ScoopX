<div align="center">
  <img src="Assets/ScoopX-icon.png" alt="ScoopX Logo" width="112" height="112" />

  # ScoopX

  **A modern development environment manager for Windows**

  Built on Scoop, ScoopX gives PHP developers a unified and intuitive way to manage their local toolchain.

  [English](README.md) | [中文](README_ZH.md)

  [![Windows 11](https://img.shields.io/badge/Windows-11-0078D4?logo=windows11&logoColor=white)](https://www.microsoft.com/windows/windows-11)
  [![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
  [![WinUI 3](https://img.shields.io/badge/WinUI-3-0078D4)](https://learn.microsoft.com/windows/apps/winui/winui3/)
  [![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
</div>

## Overview

**ScoopX** is a Windows desktop development environment manager built on [Scoop](https://scoop.sh/). It uses WinUI 3 for a native interface and brings PHP version management, configuration, and common developer tools into one place—so you spend less time on repetitive command-line work.

## Features

- **PHP version management**: Manage multiple PHP versions and the default CLI version in one place.
- **PHP configuration & extensions**: Manage each version’s `php.ini`, related environment variables, and common extensions.
- **Developer tools**: Manage MySQL, Redis, Node.js, Composer, and other tools from a single UI.
- **Scoop made visual**: Search, install, update, and uninstall packages through a graphical interface.
- **Native Windows UI**: Follows Windows 11 design and interaction patterns.

> Available package versions, extensions, and install options depend on Scoop buckets and package compatibility.

## Tech stack

| Area | Technology |
| --- | --- |
| Desktop UI | WinUI 3 / Windows App SDK |
| Language | C# |
| Runtime | .NET 10 |
| Package manager | Scoop |
| Target platform | Windows 11 |
| IDE | Visual Studio |

## Install & use

Check the [Releases](../../releases) page for available builds and install instructions. After installing, follow the in-app prompts to set up Scoop and the tools you need.

## Build from source

On Windows 11, install Visual Studio, the .NET 10 SDK, and the Windows App SDK / WinUI workload required by this project. Clone the repository, open the solution in Visual Studio, pick a target architecture, then build and run.

## Contributing

Bug reports and ideas are welcome via [Issues](../../issues). Pull requests are also welcome—please keep changes consistent with the existing architecture and coding style.

## License

ScoopX is released under the [MIT License](LICENSE). Scoop and other third-party dependencies follow their own licenses.

<div align="center">
  <sub>© ScoopX</sub>
</div>
