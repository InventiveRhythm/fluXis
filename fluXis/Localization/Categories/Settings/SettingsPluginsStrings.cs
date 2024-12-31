using osu.Framework.Localisation;

namespace fluXis.Localization.Categories.Settings;

public class SettingsPluginsStrings : LocalizationCategory
{
    protected override string File => "settings-plugins";

    public TranslatableString Title => Get("title", "Plugins");

    #region Import Plugins

    public TranslatableString ImportPlugins => Get("import-plugins-title", "Import Plugins");

    #endregion
}
