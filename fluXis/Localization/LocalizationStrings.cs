using fluXis.Localization.Categories;

namespace fluXis.Localization;

public static class LocalizationStrings
{
    public static GeneralStrings General { get; } = new();

    public static DashboardStrings Dashboard { get; } = new();
    public static ToolbarStrings Toolbar { get; } = new();
    public static SettingsStrings Settings { get; } = new();

    public static SongSelectStrings SongSelect { get; } = new();
    public static ModStrings Mods { get; } = new();
    public static ModSelectStrings ModSelect { get; } = new();

    public static MainMenuStrings MainMenu { get; } = new();
}
