using osu.Framework.Localisation;

namespace fluXis.Localization.Categories.Settings;

public class SettingsGeneralStrings : LocalizationCategory
{
    protected override string File => "settings-general";

    public TranslatableString Title => Get("title", "General");

    #region Language

    public TranslatableString Language => Get("language-title", "Language");

    public TranslatableString LanguageCurrent => Get("language-current", "Current Language");

    public TranslatableString OriginalMeta => Get("original-meta", "Prefer original metadata");
    public TranslatableString OriginalMetaDescription => Get("original-meta-description", "Displays song metadata in its original language.");

    #endregion

    #region Folders

    public TranslatableString Folders => Get("folders-title", "Folders");

    public TranslatableString FoldersOpen => Get("open-folder", "Open fluXis folder");

    public TranslatableString ExportLogs => Get("export-logs", "Export logs");
    public TranslatableString ExportLogsDescription => Get("export-logs-description", "Compresses all log files into a single zip archive for better sharing.");

    public TranslatableString FoldersChange => Get("change-folder", "Change folder location");

    #endregion
}
