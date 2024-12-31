using osu.Framework.Localisation;

namespace fluXis.Localization.Categories.Settings;

public class SettingsAppearanceStrings : LocalizationCategory
{
    protected override string File => "settings-appearance";

    public TranslatableString Title => Get("title", "Appearance");

    #region Skin

    public TranslatableString Skin => Get("skin-title", "Skin");

    public TranslatableString SkinCurrent => Get("skin-current", "Current Skin");
    public TranslatableString SkinCurrentDescription => Get("skin-current-description", "The skin you want to use.");

    public TranslatableString SkinRefresh => Get("skin-refresh", "Refresh Skin list");
    public TranslatableString SkinRefreshDescription => Get("skin-refresh-description", "Refresh the list of available skins.");

    public TranslatableString SkinOpenEditor => Get("skin-open-editor", "Open Skin editor");
    public TranslatableString SkinOpenEditorDescription => Get("skin-open-editor-description", "Open the skin in the in-game editor.");

    public TranslatableString SkinOpenFolder => Get("skin-open-folder", "Open Skin folder");
    public TranslatableString SkinOpenFolderDescription => Get("skin-open-folder-description", "Open the folder containing the skin in your file explorer.");

    public TranslatableString SkinExport => Get("skin-export", "Export Skin");
    public TranslatableString SkinExportDescription => Get("skin-export-description", "Export the current skin as a .fsk file.");

    public TranslatableString SkinDelete => Get("skin-delete", "Delete Skin");
    public TranslatableString SkinDeleteDescription => Get("skin-delete-description", "Delete the current skin.");

    #endregion

    #region Layout

    public TranslatableString Layout => Get("layout-title", "HUD Layout");

    public TranslatableString LayoutCurrent => Get("layout-current", "Current Layout");
    public TranslatableString LayoutCurrentDescription => Get("layout-current-description", "The layout you want to use.");

    public TranslatableString LayoutReload => Get("layout-reload", "Reload Layout");
    public TranslatableString LayoutReloadDescription => Get("layout-reload-description", "Reload the list of available layouts.");

    public TranslatableString LayoutCreate => Get("layout-create", "Create New Layout");
    public TranslatableString LayoutCreateDescription => Get("layout-create-description", "Create a new layout.");

    public TranslatableString LayoutShowInExplorer => Get("layout-show-in-explorer", "Show layout file in explorer");
    public TranslatableString LayoutShowInExplorerDescription => Get("layout-show-in-explorer-description", "Opens the folder containing the layout file in your file explorer. (This is temporary until the layout editor is finished)");

    public TranslatableString LayoutOpenEditor => Get("layout-open-editor", "Open Layout editor");
    public TranslatableString LayoutOpenEditorDescription => Get("layout-open-editor-description", "Open the layout in the in-game editor.");

    #endregion
}
