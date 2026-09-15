using System;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace ScrollableTabs;

public class PluginWindowSystem : WindowSystem, IDisposable
{
    private readonly IDalamudPluginInterface _pluginInterface;

    public PluginWindowSystem(IDalamudPluginInterface pluginInterface) : base("ScrollableTabs")
    {
        _pluginInterface = pluginInterface;
        _pluginInterface.UiBuilder.Draw += Draw;
    }

    public void Dispose()
    {
        _pluginInterface.UiBuilder.Draw -= Draw;
        RemoveAllWindows();
    }
}
