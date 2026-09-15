using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using InteropGenerator.Runtime.Attributes;

namespace ScrollableTabs;

// Client::UI::AddonSatisfactionList
//   Component::GUI::AtkUnitBase
//     Component::GUI::AtkEventListener
[StructLayout(LayoutKind.Explicit, Size = 0x2D8)]
public unsafe struct CustomAddonSatisfactionList
{
    public delegate void SetTabDelegate(CustomAddonSatisfactionList* thisPtr, int tabIndex, byte force = 0);

    [FieldOffset(0x280), FixedSizeArray] private FixedSizeArray9<Pointer<AtkComponentRadioButton>> _tabs;

    [FieldOffset(0x2CC)] public int TabIndex;
    [FieldOffset(0x2D0)] public int TabCount;

    [UnscopedRef] public Span<Pointer<AtkComponentRadioButton>> Tabs => _tabs;
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
[InlineArray(9)]
internal struct FixedSizeArray9<T> where T : unmanaged
{
    private T _element0;
}
