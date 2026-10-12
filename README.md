# RHI — ReShade HDR 安装器（中文版）

**一个工具。所有游戏。从一个窗口管理全部 HDR、DLSS 和 NVIDIA 驱动设置。**

RHI 自动检测所有主流平台上的整个游戏库，一键处理 ReShade、HDR 模组、帧率限制器、DLSS 管理、OptiScaler、着色器包和 NVIDIA 驱动配置。

---

## 下载

**[最新版本](https://github.com/RankFTW/RenoDXChecker/releases/latest)** · **[Discord](https://discord.gg/ultraplace)**

---

## 功能介绍

### 为每个游戏提供一键 HDR

- **[RenoDX](https://github.com/clshortfuse/renodx)** — 自动检测并为每个游戏安装正确的 HDR 模组。数百个游戏专属定制模组，并为没有专属模组的所有虚幻引擎和 Unity 游戏提供通用 HDR 支持。
- **UE-Extended** — 所有没有专属模组的虚幻引擎游戏的默认选项。自动使用正确的 HDR 设置配置 `reshade.ini` 和 `Engine.ini`。无需手动编辑 ini。
- **[Luma 框架](https://github.com/Filoppi/Luma-Framework)** — 游戏库中的每个 DX11 虚幻引擎游戏都会自动获得 Luma 行。受支持的游戏有专属模组，其他游戏提供通用支持。安装时自动应用 wiki 上的 Engine.ini 键值、启动参数和 TAA 设置。
- **[DXVK + Lilium HDR](https://github.com/EndlesslyFlowering/dxvk)** — 为通常没有 HDR 输出路径的老旧 DX9 游戏带来 scRGB HDR 输出。
- **[RenoFX HDR 工具包](https://github.com/clshortfuse/renofx)** — 为还没有专属 RenoDX 模组的任何游戏提供 SDR 转 HDR。

### ReShade — 全托管

RHI 完全取代 ReShade 安装器。

- 通过 PE 二进制扫描检测每个游戏的图形 API 并自动正确命名 DLL — `dxgi.dll`、`d3d9.dll`、`opengl32.dll`。
- 每次启动时暂存最新的稳定版和夜间版。
- 每游戏通道：稳定版、夜间版、自定义、旧版或无插件。可在游戏库中自由混合。
- Vulkan 游戏使用全局隐式层 — 一键安装，无需手动步骤。
- 外来 DLL 检测：如果游戏文件夹中已有其他工具（DXVK、Special K、ENB），覆盖前会发出警告。
- `reshade.ini` 自动初始化和合并 — 快捷键、截图路径、峰值亮度和 DLSS 路径全部自动设置，无需任何手动配置。

### DLSS 5 与神经渲染

- **DLSS5 Feeder** — 一键完整流水线安装：部署 feeder 插件、DLSS5 消费端、`nvngx_dlss.dll`、`nvngx_dlssnr.dll`、预配置的 `ReShadePreset.ini` 和所需着色器包。为您的游戏自动选择正确的方法（32位、DX11、DX12）。
- **ShortFuse DLSS Tool** — DX12 游戏的替代神经渲染实现。
- **DX11 Bridge 变体** — DLSS5 Tool + 桥接，适用于需要 DX11 兼容性的游戏。
- **NR Cost Scaler** — 包装 `nvngx_dlssnr.dll` 以降低成本运行神经渲染（默认75%）。在安装方法前切换可自动部署。

### DLSS 和 Streamline 版本管理器

- 从下拉菜单按游戏切换 SR、RR、FG 和 Streamline 版本。从 2.x 到当前的所有版本，外加自定义。
- 自动备份原始 DLL。随时可恢复原文件。
- 设置一次全局默认值 — 快速应用一键推送到任意游戏。
- 批量部署：从清单中选择多个游戏，一次推送版本、预设和渲染比例。
- DLSS 预设（SR 用 J/K/L/M，RR 用 D/E，FG 用 A/B）直接写入 NVIDIA 驱动配置。无需 Profile Inspector。
- **DLSS SR 渲染比例覆盖** — 按游戏强制 33–100% 渲染分辨率。
- **NVIDIA 覆盖** — 直接从版本下拉菜单设置驱动的"最新 DLL"标志，等同于在 NVIDIA App 或 Profile Inspector 中启用。
- 从 RHI 清单自动更新 DLSS SR、RR 和 FG。按游戏选择启用。

### 多帧生成

- **RTX 40 MFG 解锁** — 通过 Ultimate ASI Loader 在 RTX 40 系列上启用超过2倍的 DLSS MFG 倍率（最高6倍）。从附加功能部分一键安装。
- **MFG Ada Unlock** — 通过插件管理器为 RTX 40 系列提供内存中 MFG 解锁插件。
- **RTX 50 系列 MFG** — 从 DLSS 部分配置固定或动态 MFG 模式、帧数（2倍–6倍）和动态目标帧率。

### OptiScaler

- 将 DLSS API 调用重定向到 FSR4/FSR3/XeSS（适用于 AMD 和 Intel 显卡），并在游戏没有帧生成支持时添加帧生成。
- 每游戏可选**稳定版和夜间版通道**，在齿轮中选择。
- **每 GPU INI 模板** — 六个预配置模板（NVIDIA、AMD+DLSS、AMD 无DLSS，各分稳定版和夜间版）。首次启动时初始化，用户可编辑，永不覆盖。
- **Streamline + DLSS Enabler 部署**（夜间版）— 按游戏切换，将 Streamline DLL 和 DLSS Enabler 部署到 OptiScaler 文件夹。按游戏选择 Streamline 版本。
- 帧生成设置：FG 输入、FG 输出、FG Nvngx 替换、HUD 修复 — 全部可在齿轮中配置。
- **OptiPatcher** 自动部署到所有安装了 OptiScaler 的游戏。静默自动更新。
- 自动处理 ReShade 共存 — 自动调整文件名，设置 `LoadReshade=true`。

### 帧率限制器

两个每游戏帧率限制器，互斥：

- **[ReLimiter](https://github.com/RankFTW/ReLimiter)** — 精确帧率调节，具备预测休眠、锁相计时网格和闭环校正。VRR 感知、DLSS FG 自适应、集成 Reflex。
- **[Display Commander](https://github.com/pmnoxx/display-commander)** — 支持同时用于32位和64位游戏的替代限制器。

两者都作为 ReShade 插件部署。两者都参与全部更新。

### NVIDIA 驱动配置管理 — 取代 NVIDIA App 和 Profile Inspector

所有每游戏驱动设置直接通过 NvAPI 写入。无需 NVIDIA Profile Inspector，无需 NVIDIA App。

**每游戏覆盖：**
- 垂直同步模式、撕裂控制、低延迟（关/开/Ultra）
- 平滑运动：启用、允许的 API、翻转调节 — 启用时自动将低延迟设置为 Ultra
- 电源管理模式
- 每游戏 G-Sync 开关
- ReBAR：启用（关/开/自动）、模式（标准/优化）、大小限制
- RTX HDR：启用/配置，带峰值亮度、对比度、饱和度、中间灰、去色带
- DLSS 预设和渲染比例覆盖（无需管理员）

**全局设置（基础配置）：**
- 着色器缓存大小和预编译
- G-Sync 模式和首选刷新率
- 全局 ReBAR
- 帧率限制（帧率限制器 V3）
- 每显示器数字 vibrance — 每次应用启动时自动恢复

**导出/导入：**将所有每游戏 NVIDIA 配置备份到 JSON 文件。驱动重装后导入恢复所有内容。

### HDR 自动开关

通过 RHI 启动游戏时启用 Windows HDR，游戏关闭时禁用。适用于 Steam、Epic、Xbox 和直接 exe 启动。启动按钮旁有每游戏覆盖按钮 — 激活时显示紫色。

### 着色器包管理 — 46 个包

您想要的所有着色器包，全部在一处管理：

- **必备**：Lilium HDR 着色器（EndlesslyFlowering）— HDR 色调映射所需
- **推荐**：crosire reshade-shaders、PumboAutoHDR、MaxG2D Simple HDR、clshortfuse 着色器、RenoFX HDR 工具包、smolbbsoop 着色器
- **附加**：39 个额外的包 — SweetFX、OtisFX、qUINT、iMMERSE、Prod80、CorgiFX、ZenteonFX、CRT-Royale、VRToolkit、GShade-Shaders、QD-OLED APL Fixer、LumaBoost 等

全局选择包或按游戏覆盖。展开任意包可挑选单个 `.fx` 文件 — 三态复选框支持部分选择。将选择保存为命名配置，导出为 zip，直接分享到 Discord。

自定义着色器放在 `%LocalAppData%\RHI\Custom\reshade\Shaders\` — RHI 将其与托管的包一起部署。

### ReShade 插件管理

工具栏中的全局插件管理器。按游戏或全局启用/禁用。`%LocalAppData%\RHI\Custom\Addons\` 中的自定义插件自动出现在选择器中。

### 全部更新

工具栏中的一个按钮可更新整个游戏库的 ReShade、RenoDX、ReLimiter、Display Commander、OptiScaler 和 RE Framework。遵守每游戏更新排除设置。有可用更新时显示紫色。

运行时每4小时后台检查 — 可从系统托盘工作，无需保持窗口打开。

### 清单驱动的更新

游戏数据、模组链接、Engine.ini 配置、启动修复和功能标志都由每次启动时从 GitHub 获取的清单驱动。这意味着：

- 新游戏支持、修复和调整立即部署到所有用户 — 无需更新应用
- 每游戏 Engine.ini 覆盖（如《黑神话：悟空》的自定义 HDR 键值）通过服务端交付
- 功能标志通过清单切换为所有用户启用开发者预览功能

---

## 支持的平台

Steam · Epic Games · GOG · Xbox / Game Pass · EA App · Ubisoft Connect · Battle.net · Rockstar · itch.io

如果游戏未被自动检测到，可将任意游戏的 `.exe` 拖放到窗口上手动添加。

---

## 系统要求

- Windows 10 / 11
- [.NET 8 桌面运行时](https://dotnet.microsoft.com/download/dotnet/8.0)
- 驱动配置功能推荐 NVIDIA 显卡（ReShade 和 RenoDX 支持所有显卡）
- 以下功能需要管理员模式：ReBAR、平滑运动、低延迟（Ultra）、Vulkan ReShade、驱动导出/导入

---

## 第三方组件

| 组件 | 作者 |
|-----------|--------|
| [ReShade](https://reshade.me) | Crosire |
| [RenoDX](https://github.com/clshortfuse/renodx) | clshortfuse 及贡献者 |
| [ReLimiter](https://github.com/RankFTW/ReLimiter) | RankFTW |
| [Display Commander](https://github.com/pmnoxx/display-commander) | pmnoxx |
| [RE Framework](https://github.com/praydog/REFramework-nightly) | praydog |
| [Luma 框架](https://github.com/Filoppi/Luma-Framework) | Pumbo (Filoppi) |
| [OptiScaler](https://github.com/optiscaler/OptiScaler) | OptiScaler 贡献者 |
| [DXVK](https://github.com/doitsujin/dxvk) | doitsujin 及贡献者 |
| [DXVK HDR-mod](https://github.com/EndlesslyFlowering/dxvk) | EndlesslyFlowering (Lilium) |
| [Ultimate ASI Loader](https://github.com/ThirteenAG/Ultimate-ASI-Loader) | ThirteenAG |
