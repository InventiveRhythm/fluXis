using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories;

public class GeneralStrings : LocalizationCategory
{
    protected override string File => "general";

    public TranslatableString General => Get("general", "General");
    public TranslatableString Back => Get("back", "Back");
    public TranslatableString Play => Get("play", "Play");
    public TranslatableString Edit => Get("edit", "Edit");
    public TranslatableString Select => Get("select", "Select");
    public TranslatableString Delete => Get("delete", "Delete");
    public TranslatableString Export => Get("export", "Export");

    public TranslatableString PanelGenericConfirm => Get("panel-generic-confirm", "Yes, do it.");
    public TranslatableString PanelGenericCancel => Get("panel-generic-cancel", "Wait, no nevermind.");
    public TranslatableString PanelConfirmExit => Get("panel-confirm-exit", "Are you sure you want to exit?");

    public TranslatableString CanNotBeUndone => Get("can-not-be-undone", "This action cannot be undone.");

    public TranslatableString LoginToUse => Get("login-to-use", "Log in to use this feature.");
}
