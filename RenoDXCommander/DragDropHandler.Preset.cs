// DragDropHandler.Preset.cs — Preset drop processing: validate, store, game selection, deploy, shader confirmation.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RenoDXCommander.Services;
using RenoDXCommander.ViewModels;

namespace RenoDXCommander;

public partial class DragDropHandler
{
    /// <summary>
    /// Processes a dropped .ini file: validate → store → game selection → deploy → shader confirmation.
    /// </summary>
    public async Task ProcessDroppedPreset(string iniPath)
    {
        var fileName = Path.GetFileName(iniPath);
        _crashReporter.Log($"[DragDropHandler.ProcessDroppedPreset] Received '{fileName}'");

        // ── Step 1: Read and validate ─────────────────────────────────────────
        string content;
        try
        {
            content = File.ReadAllText(iniPath);
        }
        catch (Exception ex)
        {
            _crashReporter.Log($"[DragDropHandler.ProcessDroppedPreset] Failed to read '{iniPath}' — {ex.Message}");
            var errDialog = new ContentDialog
            {
                Title = "❌ 读取错误",
                Content = $"读取文件失败：{ex.Message}",
                CloseButtonText = "确定",
                XamlRoot = _window.Content.XamlRoot,
                RequestedTheme = ElementTheme.Dark,
            };
            await DialogService.ShowSafeAsync(errDialog);
            return;
        }

        if (!PresetValidator.IsReShadePreset(content))
        {
            _crashReporter.Log($"[DragDropHandler.ProcessDroppedPreset] '{fileName}' is not a recognised ReShade preset");
            var errDialog = new ContentDialog
            {
                Title = "❌ 不是 ReShade 预设",
                Content = "此文件不是有效的 ReShade 预设。有效的预设必须包含 Techniques= 行且至少有一个 @.fx 条目。",
                CloseButtonText = "确定",
                XamlRoot = _window.Content.XamlRoot,
                RequestedTheme = ElementTheme.Dark,
            };
            await DialogService.ShowSafeAsync(errDialog);
            return;
        }

        // ── Step 2: Copy to presets folder ────────────────────────────────────
        try
        {
            Directory.CreateDirectory(PresetPopupHelper.PresetsDir);
            var destPreset = Path.Combine(PresetPopupHelper.PresetsDir, fileName);
            File.Copy(iniPath, destPreset, overwrite: true);
            _crashReporter.Log($"[DragDropHandler.ProcessDroppedPreset] Stored '{fileName}' in presets folder");
        }
        catch (Exception ex)
        {
            _crashReporter.Log($"[DragDropHandler.ProcessDroppedPreset] Failed to copy to presets folder — {ex.Message}");
            var errDialog = new ContentDialog
            {
                Title = "❌ 存储错误",
                Content = $"保存预设到预设文件夹失败：{ex.Message}",
                CloseButtonText = "确定",
                XamlRoot = _window.Content.XamlRoot,
                RequestedTheme = ElementTheme.Dark,
            };
            await DialogService.ShowSafeAsync(errDialog);
            return;
        }

        // ── Step 3: Game selection dialog ─────────────────────────────────────
        var cards = ViewModel.AllCards?.ToList() ?? new();
        if (cards.Count == 0)
        {
            var noGamesDialog = new ContentDialog
            {
                Title = "无可用游戏",
                Content = "当前未检测到游戏。请先添加游戏。",
                CloseButtonText = "确定",
                XamlRoot = _window.Content.XamlRoot,
                RequestedTheme = ElementTheme.Dark,
            };
            await DialogService.ShowSafeAsync(noGamesDialog);
            return;
        }

        var combo = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            PlaceholderText = "选择游戏...",
        };

        var sortedCards = cards.OrderBy(c => c.GameName, StringComparer.OrdinalIgnoreCase).ToList();
        foreach (var card in sortedCards)
            combo.Items.Add(new ComboBoxItem { Content = card.GameName, Tag = card });

        // Auto-select the currently selected game in the sidebar
        if (ViewModel.SelectedGame != null)
        {
            for (int i = 0; i < sortedCards.Count; i++)
            {
                if (string.Equals(sortedCards[i].GameName, ViewModel.SelectedGame.GameName, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedIndex = i;
                    break;
                }
            }
        }

        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(new TextBlock
        {
            Text = $"将 {fileName} 安装到游戏文件夹。",
            TextWrapping = TextWrapping.Wrap,
            FontSize = 13,
            Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
        });
        panel.Children.Add(combo);

        var pickDialog = new ContentDialog
        {
            Title = "🎨 安装 ReShade 预设",
            Content = panel,
            PrimaryButtonText = "下一步",
            CloseButtonText = "取消",
            XamlRoot = _window.Content.XamlRoot,
            RequestedTheme = ElementTheme.Dark,
        };

        var pickResult = await DialogService.ShowSafeAsync(pickDialog);
        if (pickResult != ContentDialogResult.Primary) return;

        if (combo.SelectedItem is not ComboBoxItem selected || selected.Tag is not GameCardViewModel targetCard)
        {
            var noSelection = new ContentDialog
            {
                Title = "未选择游戏",
                Content = "请选择要安装预设的游戏。",
                CloseButtonText = "确定",
                XamlRoot = _window.Content.XamlRoot,
                RequestedTheme = ElementTheme.Dark,
            };
            await DialogService.ShowSafeAsync(noSelection);
            return;
        }

        var gameName = targetCard.GameName;
        var installPath = targetCard.InstallPath;

        // ── Step 4: Copy preset to game folder ───────────────────────────────
        try
        {
            var destGame = Path.Combine(installPath, fileName);
            File.Copy(iniPath, destGame, overwrite: true);
            _crashReporter.Log($"[DragDropHandler.ProcessDroppedPreset] Deployed '{fileName}' to '{installPath}'");
        }
        catch (Exception ex)
        {
            _crashReporter.Log($"[DragDropHandler.ProcessDroppedPreset] Failed to deploy preset — {ex.Message}");
            var errDialog = new ContentDialog
            {
                Title = "❌ 部署失败",
                Content = $"复制预设到游戏文件夹失败：{ex.Message}",
                CloseButtonText = "确定",
                XamlRoot = _window.Content.XamlRoot,
                RequestedTheme = ElementTheme.Dark,
            };
            await DialogService.ShowSafeAsync(errDialog);
            return;
        }

        // ── Step 5: Shader confirmation dialog ───────────────────────────────
        var shaderDialog = new ContentDialog
        {
            Title = "🔧 安装着色器？",
            Content = "同时安装所需的着色器和纹理？",
            PrimaryButtonText = "是",
            CloseButtonText = "否",
            XamlRoot = _window.Content.XamlRoot,
            RequestedTheme = ElementTheme.Dark,
        };

        var shaderResult = await DialogService.ShowSafeAsync(shaderDialog);
        if (shaderResult == ContentDialogResult.Primary)
        {
            _crashReporter.Log($"[DragDropHandler.ProcessDroppedPreset] User chose to install shaders for '{gameName}'");
            await ViewModel.ApplyPresetShadersAsync(gameName, new[] { iniPath }, targetCard.Source ?? "");

            // Rebuild overrides panel so the shader toggle reflects the new "Select" mode
            if (ViewModel.SelectedGame is { } selectedCard
                && selectedCard.GameName.Equals(gameName, StringComparison.OrdinalIgnoreCase))
            {
                _window.BuildOverridesPanel(selectedCard);
            }
        }
        else
        {
            _crashReporter.Log($"[DragDropHandler.ProcessDroppedPreset] User declined shader install for '{gameName}'");
        }
    }
}
