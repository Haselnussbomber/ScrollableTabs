using System;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;

namespace ScrollableTabs;

public static unsafe class ScrollHandlers
{
    public const int NumArmouryBoardTabs = 12;
    public const int NumInventoryTabs = 5;
    public const int NumInventoryLargeTabs = 4;
    public const int NumInventoryExpansionTabs = 2;
    public const int NumInventoryRetainerTabs = 6;
    public const int NumInventoryRetainerLargeTabs = 3;
    public const int NumBuddyTabs = 3;

    public static void Handle(Pointer<AtkUnitBase> unitBase, int wheelState)
    {
        switch (unitBase.Value->NameString)
        {
            case "Buddy":
            case "BuddyAction":
            case "BuddySkill":
            case "BuddyAppearance":
                UpdateBuddy(wheelState);
                break;

            case "Character":
            case "CharacterStatus":
            case "CharacterProfile":
                UpdateCharacter(wheelState);
                break;

            case "InventoryCrystalGrid":
                if (Services.GameConfig.UiConfig.TryGet("ItemInventryWindowSizeType", out uint size) && size == 2)
                    UpdateInventoryExpansion(wheelState);
                else
                    UpdateInventoryLarge(wheelState);
                break;

            case "Inventory":
            case "InventoryGrid":
            case "InventoryGridCrystal":
                UpdateInventory(wheelState);
                break;

            case "InventoryLarge":
            case "InventoryEventGrid0":
            case "InventoryEventGrid1":
            case "InventoryEventGrid2":
            case "InventoryGrid0":
            case "InventoryGrid1":
                UpdateInventoryLarge(wheelState);
                break;

            case "InventoryExpansion":
            case "InventoryEventGrid0E":
            case "InventoryEventGrid1E":
            case "InventoryEventGrid2E":
            case "InventoryGrid0E":
            case "InventoryGrid1E":
            case "InventoryGrid2E":
            case "InventoryGrid3E":
                UpdateInventoryExpansion(wheelState);
                break;

            case "InventoryEvent":
            case "InventoryEventGrid":
                UpdateInventoryEvent(wheelState);
                break;

            case "InventoryBuddy":
            case "InventoryBuddy2":
                UpdateInventoryBuddy(wheelState);
                break;

            case "InventoryRetainer":
            case "RetainerGridCrystal":
            case "RetainerGrid":
                UpdateInventoryRetainer(wheelState);
                break;

            case "InventoryRetainerLarge":
            case "RetainerCrystalGrid":
            case "RetainerGrid0":
            case "RetainerGrid1":
            case "RetainerGrid2":
            case "RetainerGrid3":
            case "RetainerGrid4":
                UpdateInventoryRetainerLarge(wheelState);
                break;

            case "MinionNoteBook":
            case "MountNoteBook":
                UpdateMountMinion(unitBase.Cast<AddonMinionMountBase>(), wheelState);
                break;

            case "CharacterClass":
                UpdateCharacterClass(unitBase.Cast<AddonCharacterClass>(), wheelState);
                break;
            case "CharacterRepute":
                UpdateCharacterRepute(unitBase.Cast<AddonCharacterRepute>(), wheelState);
                break;
            case "AOZNotebook":
                UpdateAOZNotebook(unitBase.Cast<AddonAOZNotebook>(), wheelState);
                break;
            case "AetherCurrent":
                UpdateAetherCurrent(unitBase.Cast<AddonAetherCurrent>(), wheelState);
                break;
            case "ArmouryBoard":
                UpdateArmouryBoard(unitBase.Cast<AddonArmouryBoard>(), wheelState);
                break;
            case "Currency":
                UpdateCurrency(unitBase.Cast<AddonCurrency>(), wheelState);
                break;
            case "FateProgress":
                UpdateFateProgress(unitBase.Cast<AddonFateProgress>(), wheelState);
                break;
            case "GlassSelect":
                UpdateGlassSelect(unitBase.Cast<AddonGlassSelect>(), wheelState);
                break;
            case "MJIMinionNoteBook":
                UpdateMJIMinionNoteBook(unitBase.Cast<AddonMJIMinionNoteBook>(), wheelState);
                break;
            case "MYCWarResultNotebook":
                UpdateFieldNotes(unitBase.Cast<AddonMYCWarResultNotebook>(), wheelState);
                break;
            case "MiragePrismPrismBox":
                UpdateMiragePrismPrismBox(unitBase.Cast<AddonMiragePrismPrismBox>(), wheelState);
                break;

            case "AdventureNoteBook":
                UpdateTabController(unitBase, &unitBase.Cast<AddonAdventureNoteBook>()->TabController, Services.Config.HandleAdventureNoteBook, wheelState);
                break;
            case "FishGuide2":
                UpdateTabController(unitBase, &unitBase.Cast<AddonFishGuide2>()->TabController, Services.Config.HandleFishGuide && !AgentFishGuide.Instance()->IsSearchTab, wheelState);
                break;
            case "GSInfoCardList":
                UpdateTabController(unitBase, &unitBase.Cast<AddonGSInfoCardList>()->TabController, Services.Config.HandleGoldSaucerCardList, wheelState);
                break;
            case "GSInfoEditDeck":
                UpdateTabController(unitBase, &unitBase.Cast<AddonGSInfoEditDeck>()->TabController, Services.Config.HandleGoldSaucerCardDeckEdit, wheelState);
                break;
            case "LovmPaletteEdit":
                UpdateTabController(unitBase, &unitBase.Cast<AddonLovmPaletteEdit>()->TabController, Services.Config.HandleLovmPaletteEdit, wheelState);
                break;
            case "OrnamentNoteBook":
                UpdateTabController(unitBase, &unitBase.Cast<AddonOrnamentNoteBook>()->TabController, Services.Config.HandleOrnamentNoteBook, wheelState);
                break;
        }
    }

    public static void UpdateArmouryBoard(AddonArmouryBoard* addon, int wheelState)
    {
        if (!Services.Config.HandleArmouryBoard)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumArmouryBoardTabs, wheelState);

        if (addon->TabIndex < tabIndex)
            addon->NextTab(0);
        else if (addon->TabIndex > tabIndex)
            addon->PreviousTab(0);
    }

    public static void UpdateInventory(int wheelState)
    {
        if (!Services.Config.HandleInventory)
            return;

        if (!TryGetAddon<AddonInventory>("Inventory"u8, out var addon))
            return;

        if (addon->TabIndex == NumInventoryTabs - 1 && wheelState > 0)
        {
            // Client::UI::AddonInventory.SwitchToKeyItems call in HandleBackButtonInput
            Span<AtkValue> values = stackalloc AtkValue[3];
            values.Clear();

            values[0].SetInt(22);
            values[1].SetInt(addon->OpenerAddonId);
            values[2].SetUInt(0);

            addon->FireCallback(3, values.GetPointer(0));
        }
        else
        {
            var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryTabs, wheelState);

            if (addon->TabIndex == tabIndex)
                return;

            addon->SetTab(tabIndex);
        }
    }

    public static void UpdateInventoryEvent(int wheelState)
    {
        if (!Services.Config.HandleInventory)
            return;

        if (!TryGetAddon<AddonInventoryEvent>("InventoryEvent"u8, out var addon))
            return;

        if (addon->TabIndex == 0 && wheelState < 0)
        {
            // Client::UI::AddonInventoryEvent.SwitchToInventory call in HandleBackButtonInput
            Span<AtkValue> values = stackalloc AtkValue[3];
            values.Clear();

            values[0].SetInt(22);
            values[1].SetInt(addon->OpenerAddonId);
            values[2].SetUInt(2);

            addon->FireCallback(3, values.GetPointer(0));
        }
        else
        {
            var numEnabledButtons = 0;
            foreach (ref var button in addon->Buttons)
            {
                if ((button.Value->AtkComponentButton.Flags & 0x40000) != 0)
                    numEnabledButtons++;
            }

            var tabIndex = GetTabIndex(addon->TabIndex, numEnabledButtons, wheelState);

            if (addon->TabIndex == tabIndex)
                return;

            addon->SetTab(tabIndex);
        }
    }

    public static void UpdateInventoryLarge(int wheelState)
    {
        if (!Services.Config.HandleInventory)
            return;

        if (!TryGetAddon<AddonInventoryLarge>("InventoryLarge"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryLargeTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public static void UpdateInventoryExpansion(int wheelState)
    {
        if (!Services.Config.HandleInventory)
            return;

        if (!TryGetAddon<AddonInventoryExpansion>("InventoryExpansion"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryExpansionTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex, false);
    }

    public static void UpdateInventoryRetainer(int wheelState)
    {
        if (!Services.Config.HandleRetainer)
            return;

        if (!TryGetAddon<AddonInventoryRetainer>("InventoryRetainer"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryRetainerTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public static void UpdateInventoryRetainerLarge(int wheelState)
    {
        if (!Services.Config.HandleRetainer)
            return;

        if (!TryGetAddon<AddonInventoryRetainerLarge>("InventoryRetainerLarge"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryRetainerLargeTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public static void UpdateAOZNotebook(AddonAOZNotebook* addon, int wheelState)
    {
        if (!Services.Config.HandleAOZNotebook)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex, true);
    }

    public static void UpdateAetherCurrent(AddonAetherCurrent* addon, int wheelState)
    {
        if (!Services.Config.HandleAetherCurrent)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);

        for (var i = 0; i < addon->Tabs.Length; i++)
            addon->Tabs[i].Value->IsSelected = i == tabIndex;
    }

    public static void UpdateFateProgress(AddonFateProgress* addon, int wheelState)
    {
        if (!Services.Config.HandleFateProgress)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (!addon->IsLoaded || addon->TabIndex == tabIndex)
            return;

        // fake event, so it can call SetEventIsHandled
        var atkEvent = new AtkEvent();
        addon->SetTab(tabIndex, &atkEvent);
    }

    public static void UpdateFieldNotes(AddonMYCWarResultNotebook* addon, int wheelState)
    {
        if (!Services.Config.HandleFieldRecord)
            return;

        if (RaptureAtkModule.Instance()->AtkCollisionManager.IntersectingCollisionNode == addon->DescriptionCollisionNode)
            return;

        var atkEvent = new AtkEvent();
        var eventParam = Math.Clamp(addon->CurrentNoteIndex % 10 + wheelState, -1, addon->MaxNoteIndex - 1);

        if (eventParam == -1)
        {
            if (addon->CurrentPageIndex > 0)
            {
                var page = addon->CurrentPageIndex - 1;
                addon->ReceiveEvent(AtkEventType.ButtonClick, page + 10, &atkEvent);
                addon->ReceiveEvent(AtkEventType.ButtonClick, 9, &atkEvent);
            }
        }
        else if (eventParam == 10)
        {
            if (addon->CurrentPageIndex < 4)
            {
                var page = addon->CurrentPageIndex + 1;
                addon->ReceiveEvent(AtkEventType.ButtonClick, page + 10, &atkEvent);
            }
        }
        else
        {
            addon->ReceiveEvent(AtkEventType.ButtonClick, eventParam, &atkEvent);
        }
    }

    public static void UpdateMountMinion(AddonMinionMountBase* addon, int wheelState)
    {
        var isEnabled = addon->NameString switch
        {
            "MinionNoteBook" => Services.Config.HandleMinionNoteBook,
            "MountNoteBook" => Services.Config.HandleMountNoteBook,
            _ => false,
        };

        if (!isEnabled)
            return;

        if (addon->CurrentView == AddonMinionMountBase.ViewType.Normal)
        {
            if (addon->TabController.TabIndex == 0 && wheelState < 0)
            {
                addon->SwitchToFavorites();
            }
            else
            {
                UpdateTabController((AtkUnitBase*)addon, &addon->TabController, true, wheelState);
            }
        }
        else if (addon->CurrentView == AddonMinionMountBase.ViewType.Favorites && wheelState > 0)
        {
            addon->TabController.CallbackFunction(0, (AtkUnitBase*)addon);
        }
    }

    public static void UpdateMJIMinionNoteBook(AddonMJIMinionNoteBook* addon, int wheelState)
    {
        if (!Services.Config.HandleMJIMinionNoteBook)
            return;

        var agent = AgentMJIMinionNoteBook.Instance();

        if (agent->CurrentView == AgentMJIMinionNoteBook.ViewType.Normal)
        {
            if (addon->TabController.TabIndex == 0 && wheelState < 0)
            {
                agent->CurrentView = AgentMJIMinionNoteBook.ViewType.Favorites;
                agent->SelectedFavoriteMinion.TabIndex = 0;
                agent->SelectedFavoriteMinion.SlotIndex = agent->SelectedNormalMinion.SlotIndex;
                agent->SelectedFavoriteMinion.MinionId = agent->GetSelectedMinionId();
                agent->SelectedMinion = &agent->SelectedFavoriteMinion;
                agent->HandleCommand(0x407);
            }
            else
            {
                UpdateTabController((AtkUnitBase*)addon, &addon->TabController, true, wheelState);
                agent->HandleCommand(0x40B);
            }
        }
        else if (agent->CurrentView == AgentMJIMinionNoteBook.ViewType.Favorites && wheelState > 0)
        {
            agent->CurrentView = AgentMJIMinionNoteBook.ViewType.Normal;
            agent->SelectedNormalMinion.TabIndex = 0;
            agent->SelectedNormalMinion.SlotIndex = agent->SelectedFavoriteMinion.SlotIndex;
            agent->SelectedNormalMinion.MinionId = agent->GetSelectedMinionId();
            agent->SelectedMinion = &agent->SelectedNormalMinion;

            addon->TabController.TabIndex = 0;
            addon->TabController.CallbackFunction(0, (AtkUnitBase*)addon);
            agent->HandleCommand(0x40B);
        }
    }

    public static void UpdateCurrency(AddonCurrency* addon, int wheelState)
    {
        if (!Services.Config.HandleCurrency)
            return;

        var atkStage = AtkStage.Instance();
        var numberArray = atkStage->GetNumberArrayData(NumberArrayType.Currency);
        var currentTab = numberArray->IntArray[0];
        var newTab = currentTab;

        var enableStates = new bool[addon->Tabs.Length];
        for (var i = 0; i < addon->Tabs.Length; i++)
            enableStates[i] = addon->Tabs[i].Value != null && addon->Tabs[i].Value->IsEnabled;

        if (wheelState > 0 && currentTab < enableStates.Length)
        {
            for (var i = currentTab + 1; i < enableStates.Length; i++)
            {
                if (enableStates[i])
                {
                    newTab = i;
                    break;
                }
            }
        }
        else if (currentTab > 0)
        {
            for (var i = currentTab - 1; i >= 0; i--)
            {
                if (enableStates[i])
                {
                    newTab = i;
                    break;
                }
            }
        }

        if (currentTab == newTab)
            return;

        numberArray->SetValue(0, newTab);
        addon->OnRequestedUpdate(atkStage->GetNumberArrayData(), atkStage->GetStringArrayData());
    }

    public static void UpdateInventoryBuddy(int wheelState)
    {
        if (!Services.Config.HandleInventoryBuddy)
            return;

        if (!PlayerState.Instance()->HasPremiumSaddlebag)
            return;

        if (!TryGetAddon<AddonInventoryBuddy>("InventoryBuddy"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, 2, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab((byte)tabIndex);
    }

    public static void UpdateBuddy(int wheelState)
    {
        if (!Services.Config.HandleBuddy)
            return;

        if (!TryGetAddon<AddonBuddy>("Buddy"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumBuddyTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);

        for (var i = 0; i < NumBuddyTabs; i++)
        {
            var button = addon->RadioButtons.GetPointer(i);
            if (button->Value != null)
                button->Value->IsSelected = i == addon->TabIndex;
        }
    }

    public static void UpdateMiragePrismPrismBox(AddonMiragePrismPrismBox* addon, int wheelState)
    {
        if (!Services.Config.HandleMiragePrismPrismBox)
            return;

        if (addon->JobDropdown == null ||
            addon->JobDropdown->List == null ||
            addon->JobDropdown->List->OwnerNode == null ||
            addon->JobDropdown->List->OwnerNode->IsVisible())
        {
            return;
        }

        if (addon->OrderDropdown == null ||
            addon->OrderDropdown->List == null ||
            addon->OrderDropdown->List->OwnerNode == null ||
            addon->OrderDropdown->List->OwnerNode->IsVisible())
        {
            return;
        }

        var prevButton = Services.Config.Invert ? addon->PrevButton : addon->NextButton;
        var nextButton = Services.Config.Invert ? addon->NextButton : addon->PrevButton;

        var isPrev = wheelState == (Services.Config.Invert ? -1 : 1);
        if (prevButton == null || (isPrev && !prevButton->IsEnabled))
            return;

        var isNext = wheelState == (Services.Config.Invert ? 1 : -1);
        if (nextButton == null || (isNext && !nextButton->IsEnabled))
            return;

        if (TryGetAddon<AtkUnitBase>("MiragePrismPrismBoxFilter"u8, out var filterAddon) && filterAddon->IsVisible)
            return;

        var agent = AgentMiragePrismPrismBox.Instance();
        agent->PageIndex += (byte)wheelState;
        agent->UpdateItems(false, false);
    }

    public static void UpdateGlassSelect(AddonGlassSelect* addon, int wheelState)
    {
        if (!Services.Config.HandleGlassSelect)
            return;

        UpdateTabController((AtkUnitBase*)addon, &addon->TabController, true, wheelState);

        for (var i = 0; i < addon->TabController.TabCount; i++)
        {
            var button = addon->Tabs.GetPointer(i);
            if (button->Value != null)
                button->Value->IsSelected = i == addon->TabController.TabIndex;
        }
    }

    public static void UpdateCharacter(int wheelState)
    {
        if (!Services.Config.HandleCharacter)
            return;

        if (!TryGetAddon<AddonCharacter>("Character"u8, out var addon))
            return;

        if (!addon->AddonControl.IsChildSetupComplete)
            return;

        if (RaptureAtkModule.Instance()->AtkCollisionManager.IntersectingCollisionNode == addon->PreviewController.CollisionNode)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);

        for (var i = 0; i < addon->TabCount; i++)
        {
            var button = addon->Tabs.GetPointer(i);
            if (button->Value != null)
                button->Value->IsSelected = i == addon->TabIndex;
        }
    }

    public static void UpdateCharacterClass(AddonCharacterClass* addon, int wheelState)
    {
        // prev or next embedded addon
        if (!Services.Config.HandleCharacterClass || addon->TabIndex + wheelState < 0 || addon->TabIndex + wheelState > 1)
        {
            UpdateCharacter(wheelState);
            return;
        }

        var tabIndex = GetTabIndex(addon->TabIndex, 2, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public static void UpdateCharacterRepute(AddonCharacterRepute* addon, int wheelState)
    {
        if (addon->ExpansionsDropDownList == null || addon->ExpansionsDropDownList->List == null)
            return;

        if (addon->ExpansionsDropDownList->IsOpen)
            return;

        var currentIndex = addon->ExpansionsDropDownList->GetSelectedItemIndex();

        // prev embedded addon
        if (!Services.Config.HandleCharacterRepute || currentIndex + wheelState < 0)
        {
            UpdateCharacter(wheelState);
            return;
        }

        var itemCount = addon->ExpansionsDropDownList->List->GetItemCount();
        var tabIndex = GetTabIndex(currentIndex, itemCount, wheelState);
        if (currentIndex == tabIndex)
            return;

        var atkEvent = new AtkEvent();
        var data = new AtkEventData();
        data.ListItemData.SelectedIndex = tabIndex;
        addon->AtkUnitBase.ReceiveEvent(AtkEventType.ListItemHighlight, 0, &atkEvent, &data);

        addon->ExpansionsDropDownList->SelectItem(tabIndex);
    }

    private static void UpdateTabController(AtkUnitBase* addon, TabController* tabController, bool isEnabled, int wheelState)
    {
        if (!isEnabled)
            return;

        var tabIndex = GetTabIndex(tabController->TabIndex, tabController->TabCount, wheelState);

        if (tabController->TabIndex == tabIndex)
            return;

        tabController->TabIndex = tabIndex;
        tabController->CallbackFunction(tabIndex, addon);
    }

    private static int GetTabIndex(int currentTabIndex, int numTabs, int wheelState)
    {
        return Math.Clamp(currentTabIndex + wheelState, 0, numTabs - 1);
    }

    private static bool TryGetAddon<T>(ReadOnlySpan<byte> name, out T* addon) where T : unmanaged
    {
        var unitbase = RaptureAtkUnitManager.Instance()->GetAddonByName(name);
        addon = (T*)unitbase;
        return unitbase != null && unitbase->IsReady;
    }
}
