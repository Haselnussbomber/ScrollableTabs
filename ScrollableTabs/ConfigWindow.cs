using System;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiSeStringRenderer;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using Lumina.Text.ReadOnly;
using static ScrollableTabs.Localization;

namespace ScrollableTabs;

public class ConfigWindow : Window, IDisposable
{
    public ConfigWindow() : base("ScrollableTabsConfig")
    {
        AllowClickthrough = false;
        AllowPinning = false;

        Flags |= ImGuiWindowFlags.NoScrollbar;

        Size = new Vector2(500, 500);
        SizeCondition = ImGuiCond.Appearing;

        WindowName = $"{t("ConfigWindow.WindowName")}##ScrollableTabsConfig";

        Services.PluginInterface.LanguageChanged += OnLanguageChanged;
        Services.PluginInterface.UiBuilder.OpenConfigUi += Toggle;
    }

    public void Dispose()
    {
        Services.PluginInterface.LanguageChanged -= OnLanguageChanged;
        Services.PluginInterface.UiBuilder.OpenConfigUi -= Toggle;
    }

    private void OnLanguageChanged(string langCode)
    {
        WindowName = $"{t("ConfigWindow.WindowName")}##ScrollableTabsConfig";
    }

    public override void Draw()
    {
        var config = Services.Config;

        var contentAvail = ImGui.GetContentRegionAvail();
        var style = ImGui.GetStyle();
        var footerHeight = style.ItemSpacing.Y * 3 + ImGui.GetTextLineHeightWithSpacing();

        using (var table = ImRaii.Table("ScrollableTabsConfigTable"u8, 2, ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.ScrollY, contentAvail - new Vector2(0, footerHeight)))
        {
            if (table)
            {
                ImGui.TableSetupColumn("Checkbox", ImGuiTableColumnFlags.WidthFixed, ImGui.GetFrameHeight());
                ImGui.TableSetupColumn("Text", ImGuiTableColumnFlags.WidthStretch);

                DrawBool("Invert", ref config.Invert);
                DrawBool("SuppressQuickPanelSounds", ref config.SuppressQuickPanelSounds);

                DrawBool("HandleAetherCurrent", ref config.HandleAetherCurrent);
                DrawBool("HandleArmouryBoard", ref config.HandleArmouryBoard);
                DrawBool("HandleAOZNotebook", ref config.HandleAOZNotebook);
                DrawBool("HandleCharacter", ref config.HandleCharacter);
                DrawBool("HandleCharacterClass", ref config.HandleCharacterClass);
                DrawBool("HandleCharacterRepute", ref config.HandleCharacterRepute);
                DrawBool("HandleInventoryBuddy", ref config.HandleInventoryBuddy);
                DrawBool("HandleBuddy", ref config.HandleBuddy);
                DrawBool("HandleCurrency", ref config.HandleCurrency);
                DrawBool("HandleGlassSelect", ref config.HandleGlassSelect);
                DrawBool("HandleOrnamentNoteBook", ref config.HandleOrnamentNoteBook);
                DrawBool("HandleFieldRecord", ref config.HandleFieldRecord);
                DrawBool("HandleFishGuide", ref config.HandleFishGuide);
                DrawBool("HandleMiragePrismPrismBox", ref config.HandleMiragePrismPrismBox);
                DrawBool("HandleGoldSaucerCardList", ref config.HandleGoldSaucerCardList);
                DrawBool("HandleGoldSaucerCardDeckEdit", ref config.HandleGoldSaucerCardDeckEdit);
                DrawBool("HandleLovmPaletteEdit", ref config.HandleLovmPaletteEdit);
                DrawBool("HandleInventory", ref config.HandleInventory);
                DrawBool("HandleMJIMinionNoteBook", ref config.HandleMJIMinionNoteBook);
                DrawBool("HandleMinionNoteBook", ref config.HandleMinionNoteBook);
                DrawBool("HandleMountNoteBook", ref config.HandleMountNoteBook);
                DrawBool("HandleRetainer", ref config.HandleRetainer);
                DrawBool("HandleFateProgress", ref config.HandleFateProgress);
                DrawBool("HandleAdventureNoteBook", ref config.HandleAdventureNoteBook);
            }
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        var cursorPos = ImGui.GetCursorPos();

        DrawLink("GitHub", t("ConfigWindow.GitHubLink.Tooltip"), "https://github.com/Haselnussbomber/ScrollableTabs");
        ImGui.SameLine();
        ImGui.Text("•");
        ImGui.SameLine();
        DrawLink("Sponsor", t("ConfigWindow.SponsorLink.Tooltip"), "https://github.com/sponsors/Haselnussbomber");

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

        ImGui.TextWrapped(Translate($"Config.{fieldName}.Label"));

        if (ImGui.IsItemClicked())
        {
            value = !value;
            result = true;
        }

        if (TryGetTranslation($"Config.{fieldName}.Description", out var description))
        {
            ImGuiHelpers.SeStringWrapped(ReadOnlySeString.FromText(description), new SeStringDrawParams() { Color = ColorText700 });
        }

        if (result)
        {
            Services.Config.Save();
            Services.Config.RaiseConfigOptionChanged(fieldName);
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
