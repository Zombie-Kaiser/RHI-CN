// MainWindow.FaqBuilder.cs — Builds the FAQ/Quick Start guide content dynamically.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace RenoDXCommander;

public sealed partial class MainWindow
{
    private bool _faqBuilt;

    /// <summary>
    /// Builds the FAQ panel content. Called once when FAQ is first opened.
    /// </summary>
    private void BuildFaqContent()
    {
        if (_faqBuilt) return;
        _faqBuilt = true;

        var panel = FaqContentPanel;
        panel.Children.Clear();

        // Welcome section
        panel.Children.Add(BuildFaqSection(
            null, "欢迎使用 RHI", "AccentTealBrush",
            "RHI 自动检测您的游戏，让您在一个地方安装HDR模组、着色器、帧率限制器并管理NVIDIA驱动设置。以下是使用入门方法。",
            null));

        // Step 1: Select a Game
        panel.Children.Add(BuildFaqStep(1,
            "选择游戏",
            "您的游戏列在左侧边栏中。点击任意游戏查看其详细信息和可用操作。使用筛选标签（全部游戏、已安装、虚幻引擎等）和搜索框查找特定游戏。",
            "提示：双击游戏可直接启动。将游戏的 .exe 文件拖放到 RHI 上可添加未自动检测到的游戏。"));

        // Step 2: Install ReShade
        panel.Children.Add(BuildFaqStep(2,
            "安装 ReShade",
            "RenoDX HDR模组需要 ReShade 才能工作。在游戏详情面板点击\"安装 ReShade\"。RHI 会自动下载并安装支持完整插件的正确版本。",
            "可通过游戏覆盖设置部分按游戏更改 ReShade 版本 — 可选择 Stable、Nightly、Legacy 或自定义 ReShade DLL。\nVulkan 游戏：Vulkan 游戏（如 Doom Eternal）需要管理员权限。RHI 会在需要时提示提权。\n将 ReShade 预设文件（.ini）拖放到游戏上可自动安装。"));

        // Step 3a: RenoDX
        panel.Children.Add(BuildFaqSection(
            "3a", "RenoDX", "AccentTealBrush",
            "RenoDX 是 RHI 的主要HDR模组框架。每个游戏的 RenoDX 行显示可用内容：\n\n• 专属模组 — 来自 RenoDX wiki 的游戏专属HDR模组，由社区制作。\n• UE-Extended — 适用于没有专属模组的虚幻引擎游戏。通过通用UE插件提供原生HDR输出。\n• Unity 插件 — 同样适用于Unity引擎游戏。为没有专属模组的Unity游戏提供HDR输出。\n• RTX HDR — 如果没有可用模组，可通过 RenoDX ⚙ 齿轮启用 NVIDIA 驱动级SDR转HDR。",
            "RenoDX 旁边的齿轮图标打开高级设置：峰值尼特、UE-Extended 开关和 RTX HDR 配置。\nEngine.ini 设置（仅限虚幻引擎游戏）：切换写入游戏 Engine.ini 的HDR键值和LUT更新频率以实现准确的HDR渲染。\n对于不在 wiki 上的游戏，可从 RenoDX Discord 将 .addon64 文件直接拖放到 RHI 中的游戏上。"));

        // Step 3b: Luma
        panel.Children.Add(BuildFaqSection(
            "3b", "Luma", "AccentTealBrush",
            "Luma 是由 Pumbo（HDR Den）开发的替代模组框架。根据游戏不同，Luma 模组可提供HDR、DLAA（深度学习抗锯齿）、游戏专属渲染修复或这些功能的组合 — 点击信息按钮了解每个模组的详情。\n\n• 完成版模组 — 支持游戏的专属 Luma 模组，显示在 Luma 行。\n• 通用 Luma — 适用于所有DX11虚幻引擎游戏。RHI 自动安装并应用 Luma wiki 上列出的游戏专属 Engine.ini 调整或启动参数。\n\n您可以在同一游戏上同时安装 RenoDX 和 Luma — 但不保证它们在每个游戏上都能兼容。首次尝试同时安装时 RHI 会发出警告。",
            "Luma 需要 ReShade — 在安装 ReShade 之前会显示为灰色。\nLuma ⚙ 齿轮可为需要的游戏切换 TAA Engine.ini 设置。\n如果游戏需要特定启动参数（如 -dx11），RHI 会在安装时自动设置并在卸载时移除。启动参数仅在通过 RHI 启动游戏时生效。\n点击 Luma 行的信息按钮查看游戏专属说明 — 完成版模组通常包含模组新增内容或所需游戏内设置的详情。"));

        // Step 4: Choose Shaders
        panel.Children.Add(BuildFaqStep(4,
            "选择着色器（可选）",
            "点击工具栏的\"着色器/插件\"按钮，然后选择\"全局着色器\"以选择着色器包。Lilium 的HDR着色器包默认选中。这些应用于所有已安装 ReShade 的游戏。\n\n展开任意包可挑选单个着色器 — 仅选择部分文件时包会显示短横线。使用右侧的配置面板保存、加载、重命名和共享命名的着色器选择。可将配置导出为 zip 通过 Discord 分享。",
            "提示：已安装 ReShade 时，可通过每个游戏详情卡上的着色器按钮设置每游戏着色器。\n提示：使用全部展开/全部折叠可一次浏览所有包。全部取消选择可清空当前选择。导出会将所选着色器的 zip 复制到剪贴板 — 直接粘贴到 Discord 即可分享。"));

        // Step 5: DOF Fix
        panel.Children.Add(BuildFaqStep(5,
            "景深修复（UE5推荐）",
            "景深修复将虚幻引擎 5.7 的景深渲染修复反向移植到运行 UE 5.0–5.6 的游戏。受支持时它会出现在游戏详情面板的推荐部分。与 RenoDX 一起安装可获得最佳效果。",
            "景深修复仅显示在符合条件的 UE 5.0–5.6 游戏上 — 不会出现在 UE4 或 UE 5.7+ 游戏上。\n安装和卸载方式与其他组件相同。"));

        // Step 6: Frame Limiters
        panel.Children.Add(BuildFaqStep(6,
            "帧率限制器（可选）",
            "ReLimiter 和 Display Commander 是提供精确帧率限制的 ReShade 插件，适用于VRR显示器。从游戏详情面板安装。推荐 ReLimiter，因为它由 RHI 同一团队开发。在设置中、通过齿轮图标按游戏或直接在游戏中设置目标FPS。",
            "按刷新率的VRR上限预设（低于最大值留出余量以获得流畅VRR）：\n• 60Hz → 59 FPS\n• 120Hz → 116 FPS\n• 144Hz → 138 FPS\n• 165Hz → 157 FPS\n• 240Hz → 224 FPS\n• 360Hz → 324 FPS\n这些值已在 RHI 的FPS下拉菜单中预配置。"));

        // Step 7: DLSS/Streamline
        panel.Children.Add(BuildFaqStep(7,
            "更新 DLSS / Streamline（可选）",
            "包含 DLSS 或 Streamline DLL 的游戏在详情面板有专用区域显示版本信息。点击更新到最新版本。推荐使用最新版本以获得最佳性能和质量。RHI 自动备份原文件，可随时恢复。",
            "当新的 DLSS 或 Streamline 版本发布时，它们会自动出现在 RHI 中。在设置中设置默认 DLSS 预设。每游戏预设可在每个游戏详情面板的 DLSS 部分更改。"));

        // OptiScaler
        panel.Children.Add(BuildFaqSpecialSection("⚙", "AccentAmberBrush",
            "OptiScaler（可选）",
            "OptiScaler 将 DLSS/XeSS 替换为替代升采样器（FSR、XeSS、Intel Arc），或在任何显卡上添加/修补帧生成。当游戏支持 OptiScaler 时从游戏详情面板安装。\n\nOptiScaler 行上的 ⚙ 齿轮打开每游戏设置。Nightly 更新通道的设置包括：\n• Streamline/DLSS 启用器 — 将 Streamline 和 DLSS Enabler 部署到游戏文件夹以支持DLSS帧生成。\n• 帧生成 — 设置 FG 输入、FG 输出、FG Nvngx 覆盖和HUD修复。\n• 其他设置 — DLSS SR/RR 预设、渲染比例和翻转测光。\n• 预设 — 跨游戏保存和应用命名设置预设。\n• Engine.ini 设置（虚幻引擎游戏） — 膨胀运动矢量、FSR崩溃修复、FSR-FG交换链、升采样器插件。",
            "在齿轮中按游戏切换 Stable 和 Nightly 通道 — Nightly 增加帧生成和额外设置。\nOptiScaler 和 ReShade 可以共存。如果同时安装两者出现崩溃，可尝试在游戏覆盖面板中使用DLL命名覆盖将 ReShade 重命名为其他DLL名称。\nGPU类型和DLSS输入设置（仅限AMD/Intel）在安装前于 设置 → OptiScaler 设置 中配置。\n齿轮中的\"部署 OptiScaler.ini\"按钮将您配置的INI模板重新部署到游戏文件夹。"));

        // Settings Overview
        panel.Children.Add(BuildFaqInfoSection("设置",
            "点击工具栏的\"设置\"为所有游戏配置默认值：",
            new[]
            {
                "ReLimiter FPS：默认帧率目标",
                "DLSS 预设：默认升采样预设",
                "NVIDIA 驱动设置：垂直同步、低延迟、电源模式",
                "峰值尼特：显示器的HDR峰值亮度",
                "ReShade 快捷键：自定义覆盖层和截图按键"
            }));

        // NVIDIA Driver Settings
        panel.Children.Add(BuildFaqInfoSection("NVIDIA 驱动设置",
            "RHI 可管理每游戏 NVIDIA 驱动配置。这些设置可直接在每个游戏的详情面板上使用：",
            new[]
            {
                "垂直同步：开、关或自适应（快速同步）",
                "低延迟模式：Ultra、开或关",
                "平滑运动：多帧生成（仅限每游戏）",
                "ReBAR：可调整BAR（需要管理员）"
            },
            "垂直同步、低延迟和电源模式的全局默认值在设置中配置。每游戏覆盖可直接在每个游戏的详情面板上配置。"));

        // Vulkan Games
        panel.Children.Add(BuildFaqSpecialSection("V", "AccentPurpleBrush",
            "Vulkan 游戏",
            "Vulkan 游戏（显示 Vulkan 徽章）使用安装到 C:\\ProgramData\\ReShade 的全局 ReShade 层。这需要管理员权限。当您在 Vulkan 游戏上安装 ReShade 时，RHI 会提示提权。",
            "所有 Vulkan 游戏共享同一 ReShade 安装。在一个 Vulkan 游戏上更新 ReShade 会更新所有游戏。\n每游戏 RenoDX 插件和着色器仍单独安装到每个游戏文件夹。"));

        // Adding Games Manually
        panel.Children.Add(BuildFaqSpecialSection("+", "AccentAmberBrush",
            "手动添加游戏",
            "如果游戏未被自动检测到，可将其 .exe 文件直接拖放到 RHI 窗口上。RHI 会将其添加到游戏库并检测其引擎类型。",
            "您还可以从 RenoDX Discord 将 .addon64 文件拖放到任意游戏上，安装尚未在 wiki 上的模组。"));

        // Updating Everything
        panel.Children.Add(BuildFaqSpecialSection("↑", "AccentGreenBrush",
            "全部更新",
            "点击工具栏的\"全部更新\"可一次更新所有游戏中已安装的组件。包括 ReShade、RenoDX 模组、ReLimiter、Display Commander 等。",
            "有可用更新的游戏在侧边栏显示绿点。可在设置中配置哪些组件包含在\"全部更新\"中。"));

        // Troubleshooting - Full Refresh
        panel.Children.Add(BuildFaqSpecialSection("↻", "AccentBlueBrush",
            "故障排除：完整刷新",
            "如果游戏缺失、安装位置已更改或 DLSS/Streamline 文件已添加或移除，可在设置中使用\"完整刷新\"从头重新扫描整个游戏库。",
            "完整刷新会清除缓存的游戏列表并重新检测所有内容。当普通刷新按钮无法获取更改时使用。"));

        // System Tray
        panel.Children.Add(BuildFaqSpecialSection("◰", "AccentPurpleBrush",
            "系统托盘",
            "RHI 可最小化到系统托盘而不是关闭。右键点击托盘图标可快速启动最近的游戏，无需打开主窗口。",
            "在设置中启用\"关闭到系统托盘\"可让 RHI 在后台保持运行。托盘图标可快速访问最近玩过的游戏。RHI 运行时每4小时自动检查更新，保持一切最新。"));

        // Need More Help
        panel.Children.Add(BuildFaqLinksSection());
    }


    private Border BuildFaqSection(string? badge, string title, string titleBrush, string description, string? tip)
    {
        var stack = new StackPanel { Spacing = 10 };

        var header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
        if (badge != null)
        {
            var badgeBorder = new Border
            {
                Background = (Brush)Application.Current.Resources[titleBrush],
                CornerRadius = new CornerRadius(12),
                Width = 24,
                Height = 24
            };
            badgeBorder.Child = new TextBlock
            {
                Text = badge,
                FontSize = 12,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 10, 30, 50)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            header.Children.Add(badgeBorder);
        }
        header.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center
        });
        stack.Children.Add(header);

        stack.Children.Add(new TextBlock
        {
            Text = description,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
            LineHeight = 20
        });

        if (tip != null)
        {
            var tipBorder = new Border
            {
                Background = (Brush)Application.Current.Resources["SurfaceToolbarBrush"],
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 10, 12, 10),
                BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
                BorderThickness = new Thickness(1)
            };
            tipBorder.Child = new TextBlock
            {
                Text = tip,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
                LineHeight = 18
            };
            stack.Children.Add(tipBorder);
        }

        return new Border
        {
            Background = (Brush)Application.Current.Resources["SurfaceRaisedBrush"],
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20, 16, 20, 16),
            BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
            BorderThickness = new Thickness(1),
            Child = stack
        };
    }


    private Border BuildFaqStep(int step, string title, string description, string? tip)
    {
        var stack = new StackPanel { Spacing = 8 };

        var header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var badgeBorder = new Border
        {
            Background = (Brush)Application.Current.Resources["AccentTealBrush"],
            CornerRadius = new CornerRadius(12),
            Width = 24,
            Height = 24
        };
        badgeBorder.Child = new TextBlock
        {
            Text = step.ToString(),
            FontSize = 12,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 10, 30, 50)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        header.Children.Add(badgeBorder);
        header.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center
        });
        stack.Children.Add(header);

        stack.Children.Add(new TextBlock
        {
            Text = description,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
            LineHeight = 20,
            Margin = new Thickness(32, 0, 0, 0)
        });

        if (tip != null)
        {
            var tipBorder = new Border
            {
                Background = (Brush)Application.Current.Resources["SurfaceToolbarBrush"],
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(32, 4, 0, 0),
                BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
                BorderThickness = new Thickness(1)
            };
            tipBorder.Child = new TextBlock
            {
                Text = tip,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
                LineHeight = 18
            };
            stack.Children.Add(tipBorder);
        }

        return new Border
        {
            Background = (Brush)Application.Current.Resources["SurfaceRaisedBrush"],
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20, 16, 20, 16),
            BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
            BorderThickness = new Thickness(1),
            Child = stack
        };
    }


    private Border BuildFaqInfoSection(string title, string description, string[] bullets, string? tip = null)
    {
        var stack = new StackPanel { Spacing = 8 };

        var header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var badgeBorder = new Border
        {
            Background = (Brush)Application.Current.Resources["AccentBlueBrush"],
            CornerRadius = new CornerRadius(12),
            Width = 24,
            Height = 24
        };
        badgeBorder.Child = new TextBlock
        {
            Text = "?",
            FontSize = 12,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 10, 30, 50)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        header.Children.Add(badgeBorder);
        header.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center
        });
        stack.Children.Add(header);

        stack.Children.Add(new TextBlock
        {
            Text = description,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
            LineHeight = 20,
            Margin = new Thickness(32, 0, 0, 0)
        });

        var bulletStack = new StackPanel { Spacing = 6, Margin = new Thickness(32, 4, 0, 0) };
        foreach (var bullet in bullets)
        {
            bulletStack.Children.Add(new TextBlock
            {
                Text = $"• {bullet}",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
                LineHeight = 18
            });
        }
        stack.Children.Add(bulletStack);

        if (tip != null)
        {
            var tipBorder = new Border
            {
                Background = (Brush)Application.Current.Resources["SurfaceToolbarBrush"],
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(32, 4, 0, 0),
                BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
                BorderThickness = new Thickness(1)
            };
            tipBorder.Child = new TextBlock
            {
                Text = tip,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
                LineHeight = 18
            };
            stack.Children.Add(tipBorder);
        }

        return new Border
        {
            Background = (Brush)Application.Current.Resources["SurfaceRaisedBrush"],
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20, 16, 20, 16),
            BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
            BorderThickness = new Thickness(1),
            Child = stack
        };
    }


    private Border BuildFaqSpecialSection(string badge, string badgeBrush, string title, string description, string? tip)
    {
        var stack = new StackPanel { Spacing = 8 };

        var header = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var badgeBorder = new Border
        {
            Background = (Brush)Application.Current.Resources[badgeBrush],
            CornerRadius = new CornerRadius(12),
            Width = 24,
            Height = 24
        };
        var badgeFg = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 10, 30, 50));
        badgeBorder.Child = new TextBlock
        {
            Text = badge,
            FontSize = badge.Length > 1 ? 11 : 14,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Foreground = badgeFg,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        header.Children.Add(badgeBorder);
        header.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"],
            VerticalAlignment = VerticalAlignment.Center
        });
        stack.Children.Add(header);

        stack.Children.Add(new TextBlock
        {
            Text = description,
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
            LineHeight = 20,
            Margin = new Thickness(32, 0, 0, 0)
        });

        if (tip != null)
        {
            var tipBorder = new Border
            {
                Background = (Brush)Application.Current.Resources["SurfaceToolbarBrush"],
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(32, 4, 0, 0),
                BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
                BorderThickness = new Thickness(1)
            };
            tipBorder.Child = new TextBlock
            {
                Text = tip,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
                LineHeight = 18
            };
            stack.Children.Add(tipBorder);
        }

        return new Border
        {
            Background = (Brush)Application.Current.Resources["SurfaceRaisedBrush"],
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20, 16, 20, 16),
            BorderBrush = (Brush)Application.Current.Resources["BorderSubtleBrush"],
            BorderThickness = new Thickness(1),
            Child = stack
        };
    }


    private Border BuildFaqLinksSection()
    {
        var stack = new StackPanel { Spacing = 10 };

        stack.Children.Add(new TextBlock
        {
            Text = "需要更多帮助？",
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Foreground = (Brush)Application.Current.Resources["AccentTealBrush"]
        });

        stack.Children.Add(new TextBlock
        {
            Text = "Discord 上提供支持 — 加入社区获取帮助、模组更新和讨论。",
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Foreground = (Brush)Application.Current.Resources["TextSecondaryBrush"],
            LineHeight = 20
        });

        var linksStack = new StackPanel { Spacing = 6 };

        var discordLink = new HyperlinkButton
        {
            NavigateUri = new Uri("https://discord.gg/ultraplus"),
            Padding = new Thickness(0)
        };
        discordLink.Content = new TextBlock
        {
            Text = "加入 Ultra+ Discord（主社区）",
            Foreground = (Brush)Application.Current.Resources["AccentBlueBrush"],
            FontSize = 12
        };
        linksStack.Children.Add(discordLink);

        var renodxDiscordLink = new HyperlinkButton
        {
            NavigateUri = new Uri("https://discord.gg/renodx"),
            Padding = new Thickness(0)
        };
        renodxDiscordLink.Content = new TextBlock
        {
            Text = "RenoDX Discord（模组开发）",
            Foreground = (Brush)Application.Current.Resources["AccentBlueBrush"],
            FontSize = 12
        };
        linksStack.Children.Add(renodxDiscordLink);

        var wikiLink = new HyperlinkButton
        {
            NavigateUri = new Uri("https://github.com/clshortfuse/renodx/wiki/Mods"),
            Padding = new Thickness(0)
        };
        wikiLink.Content = new TextBlock
        {
            Text = "浏览 RenoDX 模组 Wiki",
            Foreground = (Brush)Application.Current.Resources["AccentBlueBrush"],
            FontSize = 12
        };
        linksStack.Children.Add(wikiLink);

        var githubLink = new HyperlinkButton
        {
            NavigateUri = new Uri("https://github.com/RankFTW/RHI"),
            Padding = new Thickness(0)
        };
        githubLink.Content = new TextBlock
        {
            Text = "RHI GitHub — 报告问题或请求功能",
            Foreground = (Brush)Application.Current.Resources["AccentBlueBrush"],
            FontSize = 12
        };
        linksStack.Children.Add(githubLink);

        stack.Children.Add(linksStack);

        return new Border
        {
            Background = (Brush)Application.Current.Resources["SurfaceRaisedBrush"],
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20, 16, 20, 16),
            BorderBrush = (Brush)Application.Current.Resources["AccentTealBorderBrush"],
            BorderThickness = new Thickness(1),
            Child = stack
        };
    }
}
