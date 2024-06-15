using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories;

public class MainMenuStrings : LocalizationCategory
{
    protected override string File => "mainmenu";

    public TranslatableString PressAnyKey => new(GetKey("press-any-key"), "Press any key.");

    public TranslatableString PlayText => new(GetKey("play-text"), "Play");
    public TranslatableString PlayDescription(int mapcount) => new(GetKey("play-description"), "{0:0} maps loaded", mapcount);

    public TranslatableString MultiplayerText => new(GetKey("multiplayer-text"), "Multiplayer");
    public TranslatableString MultiplayerDescription => new(GetKey("multiplayer-description"), "Play with others");

    public TranslatableString DashboardText => new(GetKey("dashboard-text"), "Dashboard");
    public TranslatableString DashboardDescription => new(GetKey("dashboard-description"), "See updates, news and more");

    public TranslatableString BrowseText => new(GetKey("browse-text"), "Browse");
    public TranslatableString BrowseDescription => new(GetKey("browse-description"), "Download community-made maps");

    public TranslatableString EditText => new(GetKey("edit-text"), "Edit");
    public TranslatableString EditDescription => new(GetKey("edit-description"), "Create your own maps");
}
