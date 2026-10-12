using Microsoft.UI.Xaml.Controls;
using RenoDXCommander.Models;
using RenoDXCommander.Services;
using RenoDXCommander.ViewModels;

namespace RenoDXCommander.ViewModels;

/// <summary>
/// DXVK install, uninstall, update, and configuration command handlers.
/// Requirements: 10.1, 10.2, 10.3, 10.4, 9.3, 9.4, 22.1
/// </summary>
public partial class MainViewModel
{
    // ── DXVK Install ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Installs DXVK for the given game card.
    /// Shows a first-time warning dialog if not yet acknowledged this session.
    /// Sets <c>DxvkIsInstalling</c> during the operation to disable controls.
    /// </summary>
    public async Task InstallDxvkAsync(GameCardViewModel card, Microsoft.UI.Xaml.XamlRoot? xamlRoot = null)
    {
        if (string.IsNullOrEmpty(card.InstallPath)) return;

        // Check for manifest-driven install warning
        if (!await CheckInstallWarningAsync(card.GameName, "dxvk")) return;

        // ── DXVK warning (shown unless user has opted out via checkbox) ─────────
        if (xamlRoot != null && !_settingsViewModel.DxvkWarningDismissed)
        {
            var dontShowAgain = new CheckBox
            {
                Content = "不再显示此警告",
                Foreground = UIFactory.Brush(ResourceKeys.TextSecondaryBrush),
                Margin = new Microsoft.UI.Xaml.Thickness(0, 8, 0, 0),
            };

            var contentPanel = new StackPanel();
            contentPanel.Children.Add(new TextBlock
            {
                Text = "⚠ 高级功能 — 风险自负\n\n"
                    + "DXVK 是非官方的 DirectX 到 Vulkan 转换层。\n"
                    + "如果游戏不兼容，将不提供任何支持。\n\n"
                    + "适用人群：\n"
                    + "• 主要使旧版 DX8/DX9 游戏受益（如 FFXIV、Morrowind）\n"
                    + "• 在原生不支持的游戏上启用 ReShade 计算着色器\n"
                    + "• 可减少旧游戏的 CPU 瓶颈卡顿\n\n"
                    + "重要警告：\n"
                    + "• 反作弊游戏可能会封禁使用 DXVK 的玩家\n"
                    + "• 游戏覆盖层（Steam、NVIDIA、RTSS）可能冲突或停止工作\n"
                    + "• 独占全屏被阻止 — 请使用无边框窗口模式\n"
                    + "• 首次启动会因着色器编译而变慢（后续运行会改善）\n"
                    + "• 某些游戏可能崩溃或出现图形故障\n\n"
                    + "是否继续？",
                TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
                Foreground = UIFactory.Brush(ResourceKeys.TextPrimaryBrush),
                FontSize = 13,
            });
            contentPanel.Children.Add(dontShowAgain);

            var warningDialog = new ContentDialog
            {
                Title = "⚠ DXVK 警告",
                Content = contentPanel,
                PrimaryButtonText = "继续",
                CloseButtonText = "取消",
                XamlRoot = xamlRoot,
                RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Dark,
            };

            var result = await DialogService.ShowSafeAsync(warningDialog);
            if (result != ContentDialogResult.Primary) return;

            if (dontShowAgain.IsChecked == true)
            {
                _settingsViewModel.DxvkWarningDismissed = true;
                SaveSettingsPublic();
            }
        }

        // ── Install ──────────────────────────────────────────────────────
        // Resolve the per-game DXVK variant and set it on the service before install
        var resolvedVariant = ResolveDxvkVariant(card.GameName, card.Source ?? "");
        var savedVariant = _dxvkService.SelectedVariant;
        _dxvkService.SelectedVariant = resolvedVariant;
        _dxvkService.LiliumPresetIndex = GetLiliumPreset(card.GameName, card.Source ?? "");

        card.DxvkIsInstalling = true;
        card.DxvkActionMessage = "正在安装 DXVK...";
        card.DxvkProgress = 0;
        try
        {
            // Ensure the resolved variant's staging is ready
            if (!_dxvkService.IsStagingReady)
                await _dxvkService.EnsureStagingAsync();

            await _dxvkService.InstallAsync(card,
                new Progress<(string message, double percent)>(p =>
                {
                    card.DxvkActionMessage = p.message;
                    card.DxvkProgress = p.percent;
                }),
                screenshotSavePath: BuildScreenshotSavePath(card.GameName),
                overlayHotkey: _settingsViewModel.OverlayHotkey,
                screenshotHotkey: _settingsViewModel.ScreenshotHotkey);

            card.DxvkActionMessage = "✅ DXVK installed!";
            card.DxvkEnabled = true; // must be set so RequiresVulkanInstall returns true for DX11
            card.NotifyAll();
            card.FadeMessage(m => card.DxvkActionMessage = m, card.DxvkActionMessage);

            // Persist the installed variant as a per-game override so future update checks
            // and Update All always use the correct variant regardless of the global setting.
            // Same pattern as OptiScaler's variantHint persistence.
            SetDxvkVariantOverride(card.GameName, resolvedVariant.ToString(), card.Source ?? "");

            SaveLibrary();

            // Persist Vulkan rendering path if direct DX9 mode switched the game to Vulkan
            // Persist Vulkan rendering path for any DXVK install that switched to Vulkan —
            // not just DX9 (d3d9.dll) but also DX10/DX11 (dxgi.dll / d3d11.dll).
            if (card.DxvkStatus == GameStatus.Installed && card.VulkanRenderingPath == "Vulkan")
                SetVulkanRenderingPath(card.GameName, "Vulkan", card.Source ?? "");

            // Deploy shaders — DXVK sets up a Vulkan ReShade environment so the
            // reshade-shaders folder is needed even when ReShade wasn't installed before.
            if (!string.IsNullOrEmpty(card.InstallPath))
                DeployShadersForCard(card.GameName);

            // Rebuild the detail panel — DXVK install changes GraphicsApi, RsStatus, and badge.
            // DetailPanelBuilder is imperative so NotifyAll alone won't update it.
            RequestDetailPanelRebuild?.Invoke(card);
        }
        catch (Exception ex)
        {
            card.DxvkActionMessage = $"❌ Install failed: {ex.Message}";
            _crashReporter.WriteCrashReport("InstallDxvk", ex, note: $"Game: {card.GameName}");
        }
        finally
        {
            card.DxvkIsInstalling = false;
            _dxvkService.SelectedVariant = savedVariant;
        }
    }

    // ── DXVK Uninstall ────────────────────────────────────────────────────────────

    /// <summary>
    /// Uninstalls DXVK from the given game card.
    /// Removes deployed DLLs, restores backups, and cleans up the tracking record.
    /// </summary>
    public async Task UninstallDxvkAsync(GameCardViewModel card)
    {
        if (string.IsNullOrEmpty(card.InstallPath)) return;

        card.DxvkIsInstalling = true;
        card.DxvkActionMessage = "正在移除 DXVK...";
        try
        {
            await _dxvkService.UninstallAsync(card);
            card.DxvkEnabled = false; // must be cleared so RequiresVulkanInstall returns false after uninstall

            // Clear persisted Vulkan rendering path
            SetVulkanRenderingPath(card.GameName, "DirectX", card.Source ?? "");

            // Re-resolve the API now that DXVK DLLs are gone — skip the Vulkan override
            // so we get the original native API (DX9, DX11, etc.) back.
            if (!string.IsNullOrEmpty(card.InstallPath))
            {
                card.DetectedApis = _DetectAllApisForCard(card.InstallPath, card.GameName, card.Source);
                card.IsDualApiGame = GraphicsApiDetector.IsDualApi(card.DetectedApis);
                var nativeApi = DetectGraphicsApi(card.InstallPath, EngineType.Unknown, card.GameName, card.Source);
                card.GraphicsApi = nativeApi;
                var nativeSet = new System.Collections.Generic.HashSet<GraphicsApiType>(card.DetectedApis);
                nativeSet.Remove(GraphicsApiType.Vulkan);
                if (nativeSet.Count == 0) nativeSet.Add(nativeApi);
                CacheGameApi(card.InstallPath, nativeApi, nativeSet);
                SaveGameApiCache();
            }
            
            card.DxvkActionMessage = "✖ DXVK removed.";
            card.NotifyAll();
            card.FadeMessage(m => card.DxvkActionMessage = m, card.DxvkActionMessage);
            SaveLibrary();

            // Deploy shaders now that ReShade is back as DX proxy
            if (card.RsStatus == GameStatus.Installed && !string.IsNullOrEmpty(card.InstallPath))
                DeployShadersForCard(card.GameName);

            // Rebuild the detail panel — uninstall reverts GraphicsApi and RS state.
            RequestDetailPanelRebuild?.Invoke(card);
        }
        catch (Exception ex)
        {
            card.DxvkActionMessage = $"❌ Uninstall failed: {ex.Message}";
            _crashReporter.WriteCrashReport("UninstallDxvk", ex, note: $"Game: {card.GameName}");
        }
        finally
        {
            card.DxvkIsInstalling = false;
        }
    }

    // Keep sync wrapper for callers that pass it as a delegate (bitness change handler etc.)
    public void UninstallDxvk(GameCardViewModel card) => _ = UninstallDxvkAsync(card);

    // ── DXVK Update ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Updates DXVK for the given game card.
    /// Re-stages the latest release if needed and re-deploys DLLs.
    /// </summary>
    public async Task UpdateDxvkAsync(GameCardViewModel card)
    {
        if (string.IsNullOrEmpty(card.InstallPath)) return;

        card.DxvkIsInstalling = true;
        card.DxvkActionMessage = "正在更新 DXVK...";
        card.DxvkProgress = 0;
        try
        {
            // Resolve per-game variant and switch before update
            var resolvedVariant = ResolveDxvkVariant(card.GameName, card.Source ?? "");
            var savedVariant = _dxvkService.SelectedVariant;
            _dxvkService.SelectedVariant = resolvedVariant;

            await _dxvkService.EnsureStagingAsync(new Progress<(string message, double percent)>(p =>
            {
                card.DxvkActionMessage = p.message;
                card.DxvkProgress = p.percent;
            }));

            await _dxvkService.UpdateAsync(card,
                new Progress<(string message, double percent)>(p =>
                {
                    card.DxvkActionMessage = p.message;
                    card.DxvkProgress = p.percent;
                }));

            _dxvkService.SelectedVariant = savedVariant;

            card.DxvkActionMessage = "✅ DXVK updated!";
            card.DxvkStatus = GameStatus.Installed;
            card.NotifyAll();
            card.FadeMessage(m => card.DxvkActionMessage = m, card.DxvkActionMessage);

            // Re-persist the variant — ensures per-game override is locked in even when
            // it previously fell through to the global default.
            SetDxvkVariantOverride(card.GameName, resolvedVariant.ToString(), card.Source ?? "");

            SaveLibrary();
        }
        catch (Exception ex)
        {
            card.DxvkActionMessage = $"❌ Update failed: {ex.Message}";
            _crashReporter.WriteCrashReport("UpdateDxvk", ex, note: $"Game: {card.GameName}");
        }
        finally
        {
            card.DxvkIsInstalling = false;
        }
    }

    // ── DXVK Copy dxvk.conf ───────────────────────────────────────────────────────

    /// <summary>
    /// Copies the dxvk.conf template to the game directory.
    /// </summary>
    public void CopyDxvkConf(GameCardViewModel card)
    {
        if (string.IsNullOrEmpty(card.InstallPath)) return;
        try
        {
            // Set the correct preset index before calling the service so CopyConfToGame
            // uses the right Lilium HDR preset content for this specific game.
            _dxvkService.LiliumPresetIndex = GetLiliumPreset(card.GameName, card.Source ?? "");
            _dxvkService.CopyConfToGame(card);
            card.DxvkActionMessage = "✅ dxvk.conf copied to game folder.";
            card.FadeMessage(m => card.DxvkActionMessage = m, card.DxvkActionMessage);
        }
        catch (Exception ex)
        {
            card.DxvkActionMessage = $"❌ {ex.Message}";
        }
    }

    // ── DXVK Toggle Handler ───────────────────────────────────────────────────────

    /// <summary>
    /// Handles the DxvkEnabled toggle change.
    /// When toggled ON, triggers the DXVK install flow.
    /// When toggled OFF, triggers the DXVK uninstall flow.
    /// </summary>
    public async Task HandleDxvkToggleAsync(GameCardViewModel card, bool enabled, Microsoft.UI.Xaml.XamlRoot? xamlRoot = null)
    {
        card.DxvkEnabled = enabled;

        if (enabled)
        {
            await InstallDxvkAsync(card, xamlRoot);

            // If install failed (status didn't change to Installed), revert the toggle
            if (card.DxvkStatus != GameStatus.Installed && card.DxvkStatus != GameStatus.UpdateAvailable)
            {
                card.DxvkEnabled = false;
            }
        }
        else
        {
            UninstallDxvk(card);

            // Re-resolve Graphics API from manifest/user overrides after uninstall.
            // The DxvkService sets a hardcoded DX9 default for Lilium HDR uninstall,
            // but the proper value may differ based on manifest overrides.
            if (!string.IsNullOrEmpty(card.InstallPath))
            {
                card.DetectedApis = _DetectAllApisForCard(card.InstallPath, card.GameName, card.Source);
                card.IsDualApiGame = GraphicsApiDetector.IsDualApi(card.DetectedApis);
                card.GraphicsApi = DetectGraphicsApi(card.InstallPath, EngineType.Unknown, card.GameName, card.Source);
            }

            // Deploy shaders after ReShade is restored as DX proxy.
            // The DxvkService reinstalls ReShade but can't resolve shader packs,
            // so we deploy them here where we have access to the shader resolver.
            if (card.RsStatus == GameStatus.Installed && !string.IsNullOrEmpty(card.InstallPath))
            {
                DeployShadersForCard(card.GameName);
            }
        }

        SaveLibrary();

        // Rebuild the detail/overrides panel so the Update Inclusion section
        // picks up the new DxvkEnabled state immediately.
        card.NotifyAll();
    }
}
