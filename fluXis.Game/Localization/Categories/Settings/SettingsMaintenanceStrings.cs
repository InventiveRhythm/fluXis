using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories.Settings;

public class SettingsMaintenanceStrings : LocalizationCategory
{
    protected override string File => "settings-maintenance";

    public TranslatableString Title => Get("title", "Maintenance");

    #region Files

    public TranslatableString Files => Get("files-title", "Files");

    public TranslatableString CleanUpFiles => Get("files-clean-up", "Clean up files");
    public TranslatableString CleanUpFilesDescription => Get("files-clean-up-description", "Deletes all files that are not used by any maps.");

    #endregion

    #region Maps

    public TranslatableString Maps => Get("maps-title", "Maps");

    public TranslatableString RecalculateFilters => Get("maps-recalculate-filters", "Recalculate Filters");
    public TranslatableString RecalculateFiltersDescription => Get("maps-recalculate-filters-description", "Recalculates all filters for all maps.");

    #endregion
}
