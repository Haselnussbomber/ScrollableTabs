using System;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;

namespace ScrollableTabs;

public class PluginWindowSystem : WindowSystem, IDisposable
{
    private readonly IUiBuilder _uiBuilder;

    public PluginWindowSystem(IUiBuilder uiBuilder) : base("ScrollableTabs")
    {
        _uiBuilder = uiBuilder;
        _uiBuilder.Draw += Draw;
    }

    public void Dispose()
    {
        _uiBuilder.Draw -= Draw;
        RemoveAllWindows();
    }
}
