using fluXis.Game.Localization.Categories;

namespace fluXis.Game.Localization;

public static class LocalizationStrings
{
    public static SettingsStrings Settings { get; } = new();

    public static ModStrings Mods { get; } = new();
    public static ModSelectStrings ModSelect { get; } = new();

    public static MainMenuStrings MainMenu { get; } = new();
}
