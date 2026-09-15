using System;
using Dalamud.Memory;
using Dalamud.Plugin.Services;

namespace ScrollableTabs;

public class QuickPanelPlaySoundEffectPatch : IDisposable
{
    private readonly PluginConfig _config;

    private readonly nint _address;

    private byte[]? _originalBytes;

    public QuickPanelPlaySoundEffectPatch(ISigScanner sigScanner, PluginConfig config)
    {
        _config = config;

        if (_address == 0)
            sigScanner.TryScanText("41 B8 0D 00 00 00 48 8D 54 24 ?? 48 8B 48 ?? ?? ?? ?? FF 50 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 0F B6 47", out _address);

        if (_config.SuppressQuickPanelSounds)
            Enable();

        _config.ConfigOptionChanged += OnConfigChange;
    }

    private void OnConfigChange(string fieldName)
    {
        if (fieldName == nameof(PluginConfig.SuppressQuickPanelSounds))
        {
            if (_config.SuppressQuickPanelSounds)
                Enable();
            else
                Disable();
        }
    }

    public void Enable()
    {
        if (_address != 0 && _originalBytes == null)
            _originalBytes = ReplaceRaw(_address, [0xEB, 0x13]);
    }

    public void Disable()
    {
        if (_address != 0 && _originalBytes != null)
        {
            ReplaceRaw(_address, _originalBytes);
            _originalBytes = null;
        }
    }

    public void Dispose()
    {
        _config.ConfigOptionChanged -= OnConfigChange;
        Disable();
    }

    public static byte[] ReplaceRaw(nint address, byte[] data)
    {
        var originalBytes = MemoryHelper.ReadRaw(address, data.Length);

        MemoryHelper.ChangePermission(address, data.Length, MemoryProtection.ExecuteReadWrite, out var oldPermissions);
        MemoryHelper.WriteRaw(address, data);
        MemoryHelper.ChangePermission(address, data.Length, oldPermissions);

        return originalBytes;
    }
}
