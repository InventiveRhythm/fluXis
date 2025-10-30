using fluXis.Localization.Categories.Settings;

namespace fluXis.Localization.Categories;

public class SettingsStrings : LocalizationCategory
{
    protected override string File => "settings";

    public SettingsGeneralStrings General => new();
    public SettingsAppearanceStrings Appearance => new();
    public SettingsInputStrings Input => new();
    public SettingsUIStrings UI => new();
    public SettingsGameplayStrings Gameplay => new();
    public SettingsAudioStrings Audio => new();
    public SettingsGraphicsStrings Graphics => new();
    public SettingsPluginsStrings Plugins => new();
    public SettingsAdvancedStrings Advanced => new();
    public SettingsDebugStrings Debug => new();
}
