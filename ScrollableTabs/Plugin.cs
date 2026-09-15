using System;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;

namespace ScrollableTabs;

public unsafe class Plugin(
    IDalamudPluginInterface pluginInterface,
    IPluginLog pluginLog,
    IFramework framework,
    IGameConfig gameConfig,
    ISigScanner sigScanner,
    ICommandManager commandManager,
    IGameInteropProvider gameInteropProvider) : IAsyncDalamudPlugin
{
    public const int NumArmouryBoardTabs = 12;
    public const int NumInventoryTabs = 5;
    public const int NumInventoryLargeTabs = 4;
    public const int NumInventoryExpansionTabs = 2;
    public const int NumInventoryRetainerTabs = 6;
    public const int NumInventoryRetainerLargeTabs = 3;
    public const int NumBuddyTabs = 3;

    private readonly PluginConfig _config = PluginConfig.Load(pluginInterface, pluginLog);
    private readonly PluginWindowSystem _windowSystem = new(pluginInterface.UiBuilder);
    private readonly PluginLocalization _localization = new(pluginInterface);

    private ConfigWindow? _configWindow;
    private QuickPanelPlaySoundEffectPatch? _patch;

    [Signature("40 56 48 83 EC ?? 48 8B F1 45 84 C0 75 ?? 3B 91 ?? ?? ?? ?? 0F 84 ?? ?? ?? ?? 8B 81")]
    public CustomAddonSatisfactionList.SetTabDelegate? _addonSatisfactionListSetTab { get; set; }

    public Task LoadAsync(CancellationToken cancellationToken)
    {
        gameInteropProvider.InitializeFromAttributes(this);

        _configWindow = new(pluginInterface, commandManager, _config, _localization);
        _windowSystem.AddWindow(_configWindow);
        _patch = new(sigScanner, _config);

        framework.Update += OnFrameworkUpdate;

        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        framework.Update -= OnFrameworkUpdate;

        _windowSystem.Dispose();
        _configWindow?.Dispose();
        _patch?.Dispose();

        return ValueTask.CompletedTask;
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        var atkModule = RaptureAtkModule.Instance();
        if (atkModule == null || atkModule->UIScene != GameUIScene.GameMain)
            return;

        var unitBase = (Pointer<AtkUnitBase>)atkModule->AtkCollisionManager.IntersectingAddon;
        if (unitBase.IsNull)
            return;

        var inputData = UIInputData.Instance();
        if (inputData == null || inputData->CurrentMouseDragButtons != 0)
            return;

        var wheelState = inputData->CursorInputs.MouseWheel;
        if (wheelState == 0)
            return;

        wheelState = Math.Clamp(wheelState, -1, 1);

        if (!_config.Invert)
            wheelState = -wheelState;

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
                if (gameConfig.UiConfig.TryGet("ItemInventryWindowSizeType", out uint size) && size == 2)
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

            case "ListIcon":
                UpdateListIcon(unitBase.Cast<AddonListIcon>(), wheelState);
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
            case "SatisfactionList":
                UpdateSatisfactionList(unitBase.Cast<CustomAddonSatisfactionList>(), wheelState);
                break;

            case "AdventureNoteBook":
                UpdateTabController(unitBase, &unitBase.Cast<AddonAdventureNoteBook>()->TabController, _config.HandleAdventureNoteBook, wheelState);
                break;
            case "FishGuide2":
                UpdateTabController(unitBase, &unitBase.Cast<AddonFishGuide2>()->TabController, _config.HandleFishGuide && !AgentFishGuide.Instance()->IsSearchTab, wheelState);
                break;
            case "GSInfoCardList":
                UpdateTabController(unitBase, &unitBase.Cast<AddonGSInfoCardList>()->TabController, _config.HandleGoldSaucerCardList, wheelState);
                break;
            case "GSInfoEditDeck":
                UpdateTabController(unitBase, &unitBase.Cast<AddonGSInfoEditDeck>()->TabController, _config.HandleGoldSaucerCardDeckEdit, wheelState);
                break;
            case "LovmPaletteEdit":
                UpdateTabController(unitBase, &unitBase.Cast<AddonLovmPaletteEdit>()->TabController, _config.HandleLovmPaletteEdit, wheelState);
                break;
            case "OrnamentNoteBook":
                UpdateTabController(unitBase, &unitBase.Cast<AddonOrnamentNoteBook>()->TabController, _config.HandleOrnamentNoteBook, wheelState);
                break;
            case "XBMMonsterNotebook":
                UpdateTabController(unitBase, &unitBase.Cast<CustomAddonXBMMonsterNotebook>()->TabController, _config.HandleXBMMonsterNotebook, wheelState);
                break;
        }
    }

    public void UpdateArmouryBoard(AddonArmouryBoard* addon, int wheelState)
    {
        if (!_config.HandleArmouryBoard)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumArmouryBoardTabs, wheelState);

        if (addon->TabIndex < tabIndex)
            addon->NextTab(0);
        else if (addon->TabIndex > tabIndex)
            addon->PreviousTab(0);
    }

    public void UpdateInventory(int wheelState)
    {
        if (!_config.HandleInventory)
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

    public void UpdateInventoryEvent(int wheelState)
    {
        if (!_config.HandleInventory)
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

    public void UpdateInventoryLarge(int wheelState)
    {
        if (!_config.HandleInventory)
            return;

        if (!TryGetAddon<AddonInventoryLarge>("InventoryLarge"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryLargeTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public void UpdateInventoryExpansion(int wheelState)
    {
        if (!_config.HandleInventory)
            return;

        if (!TryGetAddon<AddonInventoryExpansion>("InventoryExpansion"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryExpansionTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex, false);
    }

    public void UpdateInventoryRetainer(int wheelState)
    {
        if (!_config.HandleRetainer)
            return;

        if (!TryGetAddon<AddonInventoryRetainer>("InventoryRetainer"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryRetainerTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public void UpdateInventoryRetainerLarge(int wheelState)
    {
        if (!_config.HandleRetainer)
            return;

        if (!TryGetAddon<AddonInventoryRetainerLarge>("InventoryRetainerLarge"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, NumInventoryRetainerLargeTabs, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public void UpdateAOZNotebook(AddonAOZNotebook* addon, int wheelState)
    {
        if (!_config.HandleAOZNotebook)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex, true);
    }

    public void UpdateAetherCurrent(AddonAetherCurrent* addon, int wheelState)
    {
        if (!_config.HandleAetherCurrent)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);

        for (var i = 0; i < addon->Tabs.Length; i++)
            addon->Tabs[i].Value->IsSelected = i == tabIndex;
    }

    public void UpdateFateProgress(AddonFateProgress* addon, int wheelState)
    {
        if (!_config.HandleFateProgress)
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (!addon->IsLoaded || addon->TabIndex == tabIndex)
            return;

        // fake event, so it can call SetEventIsHandled
        var atkEvent = new AtkEvent();
        addon->SetTab(tabIndex, &atkEvent);
    }

    public void UpdateFieldNotes(AddonMYCWarResultNotebook* addon, int wheelState)
    {
        if (!_config.HandleFieldRecord)
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

    public void UpdateListIcon(AddonListIcon* addon, int wheelState)
    {
        if (!_config.HandleListIcon)
            return;

        if (addon->TotalItemCount == 0)
            return;

        var page = GetTabIndex(addon->CurrentPage, addon->LastPage + 1, wheelState);
        if (addon->CurrentPage == page)
            return;

        addon->SetPage(page);
    }

    public void UpdateMountMinion(AddonMinionMountBase* addon, int wheelState)
    {
        var isEnabled = addon->NameString switch
        {
            "MinionNoteBook" => _config.HandleMinionNoteBook,
            "MountNoteBook" => _config.HandleMountNoteBook,
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

    public void UpdateMJIMinionNoteBook(AddonMJIMinionNoteBook* addon, int wheelState)
    {
        if (!_config.HandleMJIMinionNoteBook)
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

    public void UpdateCurrency(AddonCurrency* addon, int wheelState)
    {
        if (!_config.HandleCurrency)
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

    public void UpdateInventoryBuddy(int wheelState)
    {
        if (!_config.HandleInventoryBuddy)
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

    public void UpdateBuddy(int wheelState)
    {
        if (!_config.HandleBuddy)
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

    public void UpdateMiragePrismPrismBox(AddonMiragePrismPrismBox* addon, int wheelState)
    {
        if (!_config.HandleMiragePrismPrismBox)
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

        var prevButton = _config.Invert ? addon->PrevButton : addon->NextButton;
        var nextButton = _config.Invert ? addon->NextButton : addon->PrevButton;

        var isPrev = wheelState == (_config.Invert ? -1 : 1);
        if (prevButton == null || (isPrev && !prevButton->IsEnabled))
            return;

        var isNext = wheelState == (_config.Invert ? 1 : -1);
        if (nextButton == null || (isNext && !nextButton->IsEnabled))
            return;

        if (TryGetAddon<AtkUnitBase>("MiragePrismPrismBoxFilter"u8, out var filterAddon) && filterAddon->IsVisible)
            return;

        var agent = AgentMiragePrismPrismBox.Instance();
        agent->PageIndex += (byte)wheelState;
        agent->UpdateItems(false, false);
    }

    private void UpdateSatisfactionList(CustomAddonSatisfactionList* customAddonSatisfactionList, int wheelState)
    {
        if (!_config.HandleSatisfactionList)
            return;

        if (!TryGetAddon<CustomAddonSatisfactionList>("SatisfactionList"u8, out var addon))
            return;

        var tabIndex = GetTabIndex(addon->TabIndex, addon->TabCount, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        _addonSatisfactionListSetTab?.Invoke(addon, tabIndex);

        for (var i = 0; i < addon->TabCount; i++)
        {
            var button = addon->Tabs.GetPointer(i);
            if (button->Value != null)
                button->Value->IsSelected = i == addon->TabIndex;
        }
    }

    public void UpdateGlassSelect(AddonGlassSelect* addon, int wheelState)
    {
        if (!_config.HandleGlassSelect)
            return;

        UpdateTabController((AtkUnitBase*)addon, &addon->TabController, true, wheelState);

        for (var i = 0; i < addon->TabController.TabCount; i++)
        {
            var button = addon->Tabs.GetPointer(i);
            if (button->Value != null)
                button->Value->IsSelected = i == addon->TabController.TabIndex;
        }
    }

    public void UpdateCharacter(int wheelState)
    {
        if (!_config.HandleCharacter)
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

    public void UpdateCharacterClass(AddonCharacterClass* addon, int wheelState)
    {
        // prev or next embedded addon
        if (!_config.HandleCharacterClass || addon->TabIndex + wheelState < 0 || addon->TabIndex + wheelState > 1)
        {
            UpdateCharacter(wheelState);
            return;
        }

        var tabIndex = GetTabIndex(addon->TabIndex, 2, wheelState);

        if (addon->TabIndex == tabIndex)
            return;

        addon->SetTab(tabIndex);
    }

    public void UpdateCharacterRepute(AddonCharacterRepute* addon, int wheelState)
    {
        if (addon->ExpansionsDropDownList == null || addon->ExpansionsDropDownList->List == null)
            return;

        if (addon->ExpansionsDropDownList->IsOpen)
            return;

        var currentIndex = addon->ExpansionsDropDownList->GetSelectedItemIndex();

        // prev embedded addon
        if (!_config.HandleCharacterRepute || currentIndex + wheelState < 0)
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
