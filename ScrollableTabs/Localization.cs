using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ScrollableTabs;

public static class Localization
{
    private static readonly FrozenDictionary<string, Dictionary<string, string>> Localizations = new Dictionary<string, Dictionary<string, string>>()
    {
        ["ConfigWindow.WindowName"] = new() {
            { "en", "Scrollable Tabs Configuration" },
            { "de", "Scrollable Tabs Konfiguration" }
        },
        ["ConfigWindow.GitHubLink.Tooltip"] = new() {
            { "en", "Visit the Scrollable Tabs GitHub Repository" },
            { "de", "Zum Scrollable Tabs GitHub Repository" }
        },
        ["ConfigWindow.SponsorLink.Tooltip"] = new() {
            { "en", "Support me on GitHub Sponsors" },
            { "de", "Unterstütze mich auf GitHub Sponsors" }
        },
        ["Config.Invert.Label"] = new() {
            { "en", "Invert scroll behaviour" },
            { "de", "Invertiertes Scrollverhalten" },
            { "zh", "反转滚轮行为" },
            { "ja", "スクロール方向を反転" }
        },
        ["Config.SuppressQuickPanelSounds.Label"] = new() {
            { "en", "Suppress sound effect when scrolling in Command Panel" },
            { "de", "Soundeffekt beim scrollen in Kommandotafel unterdrücken" },
            { "ja", "コマンドパネルのスクロール時の効果音を抑制" }
        },
        ["Config.SuppressQuickPanelSounds.Description"] = new() {
            { "en", "Note: The game already integrated tab scrolling into the Command Panel." },
            { "de", "Hinweis: Das Spiel hat bereits Tab-Scrolling in die Kommandotafel integriert." },
            { "ja", "注意：コマンドパネルへのタブスクロール機能はゲーム本体に既に統合されています。" }
        },
        ["Config.HandleAetherCurrent.Label"] = new() {
            { "en", "Enable in Aether Currents" },
            { "de", "Aktiviere für Windätherquellen" },
            { "zh", "在风脉泉窗口启用" },
            { "ja", "エーテル風脈で有効化" }
        },
        ["Config.HandleArmouryBoard.Label"] = new() {
            { "en", "Enable in Armoury Chest" },
            { "de", "Aktiviere für Arsenal" },
            { "zh", "在兵装库窗口启用" },
            { "ja", "兵装庫で有効化" }
        },
        ["Config.HandleAOZNotebook.Label"] = new() {
            { "en", "Enable in Blue Magic Spellbook" },
            { "de", "Aktiviere für Zauberbuch der Blaumagie" },
            { "zh", "在青魔法书窗口启用" },
            { "ja", "青魔法手帳で有効化" }
        },
        ["Config.HandleCharacter.Label"] = new() {
            { "en", "Enable in Character" },
            { "de", "Aktiviere für Charakter" },
            { "zh", "在角色窗口启用" },
            { "ja", "キャラクターで有効化" }
        },
        ["Config.HandleCharacterClass.Label"] = new() {
            { "en", "Enable in Character -> Classes/Jobs" },
            { "de", "Aktiviere für Charakter -> Klassen/Jobs" },
            { "zh", "在角色->职业&特职窗口启用" },
            { "ja", "キャラクター → クラス/ジョブで有効化" }
        },
        ["Config.HandleCharacterRepute.Label"] = new() {
            { "en", "Enable in Character -> Reputation" },
            { "de", "Aktiviere für Charakter -> Ansehen" },
            { "zh", "在 角色->评价窗口启用" },
            { "ja", "キャラクター → 名声で有効化" }
        },
        ["Config.HandleInventoryBuddy.Label"] = new() {
            { "en", "Enable in Chocobo Saddlebag" },
            { "de", "Aktiviere für Chocobo-Satteltasche" },
            { "zh", "在陆行鸟鞍囊窗口启用" },
            { "ja", "チョコボかばんで有効化" }
        },
        ["Config.HandleInventoryBuddy.Description"] = new() {
            { "en", "The second tab requires a subscription to the Companion Premium Service" },
            { "de", "Der zweite Tab benötigt ein Abonnement des Premium-Nutzungsplans in der Companion-App." },
            { "zh", "第二页标签页需要开通陆行鸟鞍囊2服务" },
            { "ja", "2つ目のタブはコンパニオンアプリのプレミアムサービスへの加入が必要です。" }
        },
        ["Config.HandleBuddy.Label"] = new() {
            { "en", "Enable in Companion" },
            { "de", "Aktiviere für Mitstreiter" },
            { "zh", "在搭档窗口启用" },
            { "ja", "バディで有効化" }
        },
        ["Config.HandleCurrency.Label"] = new() {
            { "en", "Enable in Currency" },
            { "de", "Aktiviere für Vermögen" },
            { "zh", "在货币一览窗口启用" },
            { "ja", "所持金・通貨で有効化" }
        },
        ["Config.HandleOrnamentNoteBook.Label"] = new() {
            { "en", "Enable in Fashion Accessories" },
            { "de", "Aktiviere für Modeaccessoires" },
            { "zh", "在时尚配饰窗口启用" },
            { "ja", "ファッションアクセサリーで有効化" }
        },
        ["Config.HandleFieldRecord.Label"] = new() {
            { "en", "Enable in Field Records" },
            { "de", "Aktiviere für Frontbericht" },
            { "zh", "在战果记录窗口启用" }
        },
        ["Config.HandleFishGuide.Label"] = new() {
            { "en", "Enable in Fish Guide" },
            { "de", "Aktiviere für Fischverzeichnis" },
            { "zh", "在鱼类图鉴窗口启用" },
            { "ja", "魚類図鑑で有効化" }
        },
        ["Config.HandleMiragePrismPrismBox.Label"] = new() {
            { "en", "Enable in Glamour Dresser" },
            { "de", "Aktiviere für Projektionskommode" },
            { "zh", "在投影台窗口启用" }
        },
        ["Config.HandleMiragePrismPrismBox.Description"] = new() {
            { "en", "Scrolls pages, not tabs." },
            { "de", "Blättert durch Seiten, nicht durch Tabs." },
            { "zh", "滚动页面，而非标签页。" },
            { "ja", "タブではなくページをスクロールします。" }
        },
        ["Config.HandleGlassSelect.Label"] = new() {
            { "en", "Enable in Facewear" },
            { "de", "Aktiviere für Gesichtsaccessoires" }
        },
        ["Config.HandleGoldSaucerCardList.Label"] = new() {
            { "en", "Enable in Gold Saucer -> Card List" },
            { "de", "Aktiviere für Gold Saucer -> Karten" },
            { "zh", "在金碟游乐场->幻卡列表窗口启用" },
            { "ja", "ゴールドソーサー → カード一覧で有効化" }
        },
        ["Config.HandleGoldSaucerCardDeckEdit.Label"] = new() {
            { "en", "Enable in Gold Saucer -> Decks -> Edit Deck" },
            { "de", "Aktiviere für Gold Saucer -> Decks -> Deck ändern" },
            { "zh", "在金碟游乐场->卡组->编辑卡组窗口启用" },
            { "ja", "ゴールドソーサー → デッキ → デッキ編集で有効化" }
        },
        ["Config.HandleLovmPaletteEdit.Label"] = new() {
            { "en", "Enable in Gold Saucer -> Lord of Verminion -> Minion Hotbar" },
            { "de", "Aktiviere für Gold Saucer -> Trabanten -> Kommandomenü bearbeiten" },
            { "zh", "在金碟游乐场->萌宠之王->宠物热键栏窗口启用" },
            { "ja", "ゴールドソーサー → ミニオンレース → ミニオンホットバーで有効化" }
        },
        ["Config.HandleInventory.Label"] = new() {
            { "en", "Enable in Inventory" },
            { "de", "Aktiviere für Inventar" },
            { "zh", "在物品栏窗口启用" }
        },
        ["Config.HandleMJIMinionNoteBook.Label"] = new() {
            { "en", "Enable in Island Minion Guide" },
            { "de", "Aktiviere für Insel-Begleiterliste" },
            { "zh", "在岛内宠物列表窗口启用" },
            { "ja", "島のミニオン図鑑で有効化" }
        },
        ["Config.HandleMinionNoteBook.Label"] = new() {
            { "en", "Enable in Minions" },
            { "de", "Aktiviere für Begleiter-Verzeichnis" },
            { "zh", "在宠物窗口启用" },
            { "ja", "ミニオン図鑑で有効化" }
        },
        ["Config.HandleMountNoteBook.Label"] = new() {
            { "en", "Enable in Mounts" },
            { "de", "Aktiviere für Reittier-Verzeichnis" },
            { "zh", "在坐骑窗口启用" },
            { "ja", "マウント図鑑で有効化" }
        },
        ["Config.HandleRetainer.Label"] = new() {
            { "en", "Enable in Retainer Inventory" },
            { "de", "Aktiviere für Gehilfeninventar" },
            { "zh", "在雇员物品栏窗口启用" },
            { "ja", "リテイナーインベントリで有効化" }
        },
        ["Config.HandleFateProgress.Label"] = new() {
            { "en", "Enable in Shared FATE" },
            { "de", "Aktiviere für FATE-Fortschritt" },
            { "zh", "在危命任务完成度窗口启用" }
        },
        ["Config.HandleAdventureNoteBook.Label"] = new() {
            { "en", "Enable in Sightseeing Log" },
            { "de", "Aktiviere für Eorzea Incognita" },
            { "zh", "在探索笔记窗口启用" }
        }
    }.ToFrozenDictionary();

    public static string t(string key)
        => TryGetTranslation(key, out var text) ? text : key;

    public static string Translate(string key)
        => TryGetTranslation(key, out var text) ? text : key;

    public static bool TryGetTranslation(string key, [MaybeNullWhen(returnValue: false)] out string text)
    {
        text = string.Empty;
        return Localizations.TryGetValue(key, out var languages)
            && (languages.TryGetValue(Services.PluginInterface.UiLanguage, out text)
            || languages.TryGetValue("en", out text));
    }
}
