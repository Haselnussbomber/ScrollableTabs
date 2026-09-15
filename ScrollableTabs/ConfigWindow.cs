using System;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiSeStringRenderer;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Text.ReadOnly;

namespace ScrollableTabs;

public class ConfigWindow : Window, IDisposable
{
    private readonly IDalamudPluginInterface _pluginInterface;
    private readonly ICommandManager _commandManager;
    private readonly PluginConfig _config;
    private readonly PluginLocalization _localization;
    private readonly CommandInfo _commandInfo;

    public ConfigWindow(IDalamudPluginInterface pluginInterface, ICommandManager commandManager, PluginConfig config, PluginLocalization localization) : base("ScrollableTabsConfig")
    {
        _pluginInterface = pluginInterface;
        _commandManager = commandManager;
        _config = config;
        _localization = localization;

        AllowClickthrough = false;
        AllowPinning = false;

        Flags |= ImGuiWindowFlags.NoScrollbar;

        Size = new Vector2(500, 500);
        SizeCondition = ImGuiCond.Appearing;

        WindowName = $"{_localization.Translate("ConfigWindow.WindowName")}##ScrollableTabsConfig";

        _pluginInterface.LanguageChanged += OnLanguageChanged;
        _pluginInterface.UiBuilder.OpenConfigUi += Toggle;

        _commandInfo = new CommandInfo((_, _) => Toggle())
        {
            HelpMessage = _localization.Translate("ConfigWindow.CommandHelpMessage")
        };

        commandManager.AddHandler("/scrollabletabs", _commandInfo);
    }

    public void Dispose()
    {
        _pluginInterface.LanguageChanged -= OnLanguageChanged;
        _pluginInterface.UiBuilder.OpenConfigUi -= Toggle;
        _commandManager.RemoveHandler("/scrollabletabs");
    }

    private void OnLanguageChanged(string langCode)
    {
        WindowName = $"{_localization.Translate("ConfigWindow.WindowName")}##ScrollableTabsConfig";
        _commandInfo.HelpMessage = _localization.Translate("ConfigWindow.CommandHelpMessage");
    }

    public override void Draw()
    {
        var contentAvail = ImGui.GetContentRegionAvail();
        var style = ImGui.GetStyle();
        var footerHeight = style.ItemSpacing.Y * 3 + ImGui.GetTextLineHeightWithSpacing();

        using (var table = ImRaii.Table("ScrollableTabsConfigTable"u8, 2, ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.ScrollY, contentAvail - new Vector2(0, footerHeight)))
        {
            if (table)
            {
                ImGui.TableSetupColumn("Checkbox", ImGuiTableColumnFlags.WidthFixed, ImGui.GetFrameHeight());
                ImGui.TableSetupColumn("Text", ImGuiTableColumnFlags.WidthStretch);

                DrawBool("Invert", ref _config.Invert);
                DrawBool("SuppressQuickPanelSounds", ref _config.SuppressQuickPanelSounds);

                DrawBool("HandleAetherCurrent", ref _config.HandleAetherCurrent);
                DrawBool("HandleArmouryBoard", ref _config.HandleArmouryBoard);
                DrawBool("HandleAOZNotebook", ref _config.HandleAOZNotebook);
                DrawBool("HandleCharacter", ref _config.HandleCharacter);
                DrawBool("HandleCharacterClass", ref _config.HandleCharacterClass);
                DrawBool("HandleCharacterRepute", ref _config.HandleCharacterRepute);
                DrawBool("HandleInventoryBuddy", ref _config.HandleInventoryBuddy);
                DrawBool("HandleBuddy", ref _config.HandleBuddy);
                DrawBool("HandleSatisfactionList", ref _config.HandleSatisfactionList);
                DrawBool("HandleCurrency", ref _config.HandleCurrency);
                DrawBool("HandleGlassSelect", ref _config.HandleGlassSelect);
                DrawBool("HandleOrnamentNoteBook", ref _config.HandleOrnamentNoteBook);
                DrawBool("HandleFieldRecord", ref _config.HandleFieldRecord);
                DrawBool("HandleFishGuide", ref _config.HandleFishGuide);
                DrawBool("HandleMiragePrismPrismBox", ref _config.HandleMiragePrismPrismBox);
                DrawBool("HandleGoldSaucerCardList", ref _config.HandleGoldSaucerCardList);
                DrawBool("HandleGoldSaucerCardDeckEdit", ref _config.HandleGoldSaucerCardDeckEdit);
                DrawBool("HandleLovmPaletteEdit", ref _config.HandleLovmPaletteEdit);
                DrawBool("HandleListIcon", ref _config.HandleListIcon);
                DrawBool("HandleInventory", ref _config.HandleInventory);
                DrawBool("HandleMJIMinionNoteBook", ref _config.HandleMJIMinionNoteBook);
                DrawBool("HandleXBMMonsterNotebook", ref _config.HandleXBMMonsterNotebook);
                DrawBool("HandleMinionNoteBook", ref _config.HandleMinionNoteBook);
                DrawBool("HandleMountNoteBook", ref _config.HandleMountNoteBook);
                DrawBool("HandleRetainer", ref _config.HandleRetainer);
                DrawBool("HandleFateProgress", ref _config.HandleFateProgress);
                DrawBool("HandleAdventureNoteBook", ref _config.HandleAdventureNoteBook);
            }
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        var cursorPos = ImGui.GetCursorPos();

        DrawLink("GitHub", _localization.Translate("ConfigWindow.GitHubLink.Tooltip"), "https://github.com/Haselnussbomber/ScrollableTabs");
        ImGui.SameLine();
        ImGui.Text("•");
        ImGui.SameLine();
        DrawLink("Sponsor", _localization.Translate("ConfigWindow.SponsorLink.Tooltip"), "https://github.com/sponsors/Haselnussbomber");

        var version = Assembly.GetExecutingAssembly().GetName().Version;
        if (version != null)
        {
            var versionString = "v" + version.ToString(3);
            ImGui.SetCursorPos(new Vector2(cursorPos.X + contentAvail.X - ImGui.CalcTextSize(versionString).X, cursorPos.Y));
            ImGui.TextDisabled(versionString);
        }
    }

    public bool DrawBool(string fieldName, ref bool value)
    {
        using var id = ImRaii.PushId(fieldName);

        var result = false;

        ImGui.TableNextRow();
        ImGui.TableNextColumn();

        result = ImGui.Checkbox("##Input", ref value);

        ImGui.TableNextColumn();

        ImGui.TextWrapped(_localization.Translate($"Config.{fieldName}.Label"));

        if (ImGui.IsItemClicked())
        {
            value = !value;
            result = true;
        }

        if (_localization.TryGetTranslation($"Config.{fieldName}.Description", out var description))
        {
            ImGuiHelpers.SeStringWrapped(ReadOnlySeString.FromText(description), new SeStringDrawParams() { Color = ColorText700 });
        }

        if (result)
        {
            _config.Save();
            _config.RaiseConfigOptionChanged(fieldName);
        }

        return result;
    }

    public static void DrawLink(string label, string title, string url)
    {
        ImGui.Text(label);

        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);

            using var tooltip = ImRaii.Tooltip();

            if (!string.IsNullOrEmpty(title))
                ImGui.TextColored(Vector4.One, title);

            ImGui.GetWindowDrawList().AddText(
                UiBuilder.IconFont, 12 * ImGuiHelpers.GlobalScale,
                ImGui.GetCursorScreenPos() + new Vector2(2 * ImGuiHelpers.GlobalScale),
                ColorText700,
                FontAwesomeIcon.ExternalLinkAlt.ToIconString());

            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 20 * ImGuiHelpers.GlobalScale);

            ImGui.TextColored(ColorText700, url);
        }

        if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && ImGui.IsItemHovered())
            Task.Run(() => Util.OpenLink(url));
    }

    private static uint ColorText700 => ImGui.ColorConvertFloat4ToU32(ImGui.ColorConvertU32ToFloat4(ImGui.GetColorU32(ImGuiCol.Text)) with { W = 0.7f });
}
