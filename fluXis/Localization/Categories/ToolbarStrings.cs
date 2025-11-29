using osu.Framework.Localisation;

namespace fluXis.Localization.Categories;

public class ToolbarStrings : LocalizationCategory
{
    protected override string File => "toolbar";

    public TranslatableString Settings => Get("settings", "Settings");
    public TranslatableString SettingsDescription => Get("settings-description", "Change your settings.");

    public TranslatableString Home => Get("home", "Home");
    public TranslatableString HomeDescription => Get("home-description", "Return to the main menu.");

    public TranslatableString Maps => Get("maps", "Maps");
    public TranslatableString MapsDescription => Get("maps-description", "Browse your maps.");

    public TranslatableString Multiplayer => Get("multiplayer", "Multiplayer");
    public TranslatableString MultiplayerDescription => Get("multiplayer-description", "Play with others.");

    public TranslatableString Ranking => Get("ranking", "Ranking");
    public TranslatableString RankingDescription => Get("ranking-description", "See the top players.");

    public TranslatableString Dashboard => Get("dashboard", "Dashboard");
    public TranslatableString DashboardDescription => Get("dashboard-description", "Updates, Chats, news and more.");

    public TranslatableString Browse => Get("browse", "Browse");
    public TranslatableString BrowseDescription => Get("browse-description", "Download community-made maps.");

    public TranslatableString Wiki => Get("wiki", "Wiki");
    public TranslatableString WikiDescription => Get("wiki-description", "Learn about the game.");

    public TranslatableString Music => Get("music", "Music Player");
    public TranslatableString MusicDescription => Get("music-description", "Listen to your music.");
}
