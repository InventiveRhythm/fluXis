using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories;

public class MainMenuStrings : LocalizationCategory
{
    protected override string File => "mainmenu";

    public LocalisableString PlayText => new TranslatableString(GetKey("play-text"), "Play");
    public LocalisableString PlayDescription(int mapcount) => new TranslatableString(GetKey("play-description"), "{0:0} maps loaded", mapcount);

    public LocalisableString MultiplayerText => new TranslatableString(GetKey("multiplayer-text"), "Multiplayer");
    public LocalisableString MultiplayerDescription => new TranslatableString(GetKey("multiplayer-description"), "Play against other players");

    public LocalisableString RankingText => new TranslatableString(GetKey("ranking-text"), "Ranking");
    public LocalisableString RankingDescription => new TranslatableString(GetKey("ranking-description"), "Check online leaderboards");

    public LocalisableString BrowseText => new TranslatableString(GetKey("browse-text"), "Browse");
    public LocalisableString BrowseDescription => new TranslatableString(GetKey("browse-description"), "Download community-made maps");

    public LocalisableString EditText => new TranslatableString(GetKey("edit-text"), "Edit");
    public LocalisableString EditDescription => new TranslatableString(GetKey("edit-description"), "Create your own maps");
}
