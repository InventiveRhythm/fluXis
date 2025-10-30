using osu.Framework.Localisation;

namespace fluXis.Localization.Categories.Settings;

public class SettingsAdvancedStrings : LocalizationCategory
{
    protected override string File => "settings-maintenance";

    public TranslatableString Title => Get("title", "Advanced");

    #region Files

    public TranslatableString Files => Get("files-title", "Files");

    public TranslatableString CleanUpFiles => Get("files-clean-up", "Clean up files");
    public TranslatableString CleanUpFilesDescription => Get("files-clean-up-description", "Deletes all files that are not used by any maps.");

    public TranslatableString StreamFileBrowser => Get("stream-file-browser", "Stream Files in File Browser.");

    public TranslatableString StreamFileBrowserDescription => Get("stream-file-browser-description",
        "Streams files and folders for large directories. Turning this off might result in a game freeze when opening a large directory.");

    #endregion

    #region Maps

    public TranslatableString Maps => Get("maps-title", "Maps");

    public TranslatableString RecalculateFilters => Get("maps-recalculate-filters", "Recalculate Filters");
    public TranslatableString RecalculateFiltersDescription => Get("maps-recalculate-filters-description", "Recalculates all filters for all maps.");

    public TranslatableString CleanUpScores => Get("maps-scores-clean-up", "Clean up Scores");
    public TranslatableString CleanUpScoresDescription => Get("maps-scores-clean-up-description", "Removes scores from maps that you do not have anymore.");

    #endregion
}
