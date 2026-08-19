using System;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace ScrollableTabs;

public unsafe class Plugin(IDalamudPluginInterface pluginInterface) : IAsyncDalamudPlugin
{
    private PluginWindowSystem? _windowSystem;
    private ConfigWindow? _configWindow;
    private QuickPanelPlaySoundEffectPatch? _patch;

    public Task LoadAsync(CancellationToken cancellationToken)
    {
        Services.Initialize(pluginInterface);

        _windowSystem = new PluginWindowSystem();
        _configWindow = new ConfigWindow();
        _windowSystem.AddWindow(_configWindow);
        _patch = new();

        Services.Framework.Update += OnFrameworkUpdate;

        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Services.Framework.Update -= OnFrameworkUpdate;

        if (_configWindow != null)
        {
            _windowSystem?.RemoveWindow(_configWindow);
            _configWindow.Dispose();
            _configWindow = null;
        }

        _windowSystem?.Dispose();
        _windowSystem = null!;

        _patch?.Dispose();
        _patch = null;

        return ValueTask.CompletedTask;
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        var atkModule = RaptureAtkModule.Instance();
        if (atkModule == null || atkModule->UIScene != GameUIScene.GameMain)
            return;

        var hoveredUnitBase = atkModule->AtkCollisionManager.IntersectingAddon;
        if (hoveredUnitBase == null)
            return;

        var inputData = UIInputData.Instance();
        if (inputData == null || inputData->CurrentMouseDragButtons != 0)
            return;

        var wheelState = inputData->CursorInputs.MouseWheel;
        if (wheelState == 0)
            return;

        wheelState = Math.Clamp(wheelState, -1, 1);

        if (!Services.Config.Invert)
            wheelState = -wheelState;

        ScrollHandlers.Handle(hoveredUnitBase, wheelState);
    }

    public class PluginWindowSystem : WindowSystem, IDisposable
    {
        public PluginWindowSystem() : base("ScrollableTabs")
        {
            Services.PluginInterface.UiBuilder.Draw += Draw;
        }

        public void Dispose()
        {
            Services.PluginInterface.UiBuilder.Draw -= Draw;
            RemoveAllWindows();
        }
    }
}
