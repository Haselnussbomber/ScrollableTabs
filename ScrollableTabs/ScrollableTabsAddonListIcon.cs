using System;
using System.Runtime.InteropServices;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ScrollableTabs;

[StructLayout(LayoutKind.Explicit, Size = 0x2D8)]
public struct ScrollableTabsAddonListIcon
{
    [FieldOffset(0x00)] public AtkUnitBase AtkUnitBase;
    [FieldOffset(0x238)] public int TotalItemCount;
    [FieldOffset(0x23C)] public int CurrentPage;
    [FieldOffset(0x240)] public int LastPage;
}

public unsafe class ListIconInterop
{
    public static Lazy<ListIconInterop> Instance = new(() => new ListIconInterop());

    public ListIconInterop()
    {
        Services.GameInteropProvider.InitializeFromAttributes(this);
    }

    public delegate void SetPageDelegate(ScrollableTabsAddonListIcon* thisPtr, int page);

    [Signature("E8 ?? ?? ?? ?? 85 DB 74 ?? 3B 9F")]
    public SetPageDelegate? SetPage;
}
