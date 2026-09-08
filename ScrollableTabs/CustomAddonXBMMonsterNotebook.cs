using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace ScrollableTabs;

[StructLayout(LayoutKind.Explicit, Size = 0x880)]
public struct CustomAddonXBMMonsterNotebook
{
    [FieldOffset(0x6E8)] public TabController TabController;
}
