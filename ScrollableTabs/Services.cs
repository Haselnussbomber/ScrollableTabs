using System;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace ScrollableTabs;

public static class Services
{
    public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    public static IPluginLog PluginLog { get; private set; } = null!;
    public static IFramework Framework { get; private set; } = null!;
    public static IGameConfig GameConfig { get; private set; } = null!;
    public static ISigScanner SigScanner { get; private set; } = null!;
    public static PluginConfig Config { get; private set; } = null!;

    public static void Initialize(IDalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;
        PluginLog = pluginInterface.GetService<IPluginLog>();
        Framework = pluginInterface.GetService<IFramework>();
        GameConfig  = pluginInterface.GetService<IGameConfig>();
        SigScanner = pluginInterface.GetService<ISigScanner>();
        Config = PluginConfig.Load();
    }

    private static T GetService<T>(this IServiceProvider serviceProvider)
    {
        return (T)serviceProvider.GetService(typeof(T))!;
    }
}
