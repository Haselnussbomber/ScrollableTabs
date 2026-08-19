using System;
using Dalamud.Memory;

namespace ScrollableTabs;

public class QuickPanelPlaySoundEffectPatch : IDisposable
{
    private static nint Address;

    private byte[]? _originalBytes;

    public QuickPanelPlaySoundEffectPatch()
    {
        if (Address == 0)
            Services.SigScanner.TryScanText("41 B8 0D 00 00 00 48 8D 54 24 ?? 48 8B 48 ?? ?? ?? ?? FF 50 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 0F B6 47", out Address);

        if (Services.Config.SuppressQuickPanelSounds)
            Enable();

        Services.Config.ConfigOptionChanged += OnConfigChange;
    }

    private void OnConfigChange(string fieldName)
    {
        if (fieldName == nameof(PluginConfig.SuppressQuickPanelSounds))
        {
            if (Services.Config.SuppressQuickPanelSounds)
                Enable();
            else
                Disable();
        }
    }

    public void Enable()
    {
        if (Address != 0 && _originalBytes == null)
            _originalBytes = ReplaceRaw(Address, [0xEB, 0x13]);
    }

    public void Disable()
    {
        if (Address != 0 && _originalBytes != null)
        {
            ReplaceRaw(Address, _originalBytes);
            _originalBytes = null;
        }
    }

    public void Dispose()
    {
        Services.Config.ConfigOptionChanged -= OnConfigChange;
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
