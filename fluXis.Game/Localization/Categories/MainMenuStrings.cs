using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories;

public class MainMenuStrings : LocalizationCategory
{
    protected override string File => "mainmenu";

    public TranslatableString PressAnyKey => new TranslatableString(GetKey("press-any-key"), "Press any key.");

    public TranslatableString PlayText => new TranslatableString(GetKey("play-text"), "Play");
    public TranslatableString PlayDescription(int mapcount) => new TranslatableString(GetKey("play-description"), "{0:0} maps loaded", mapcount);

    public TranslatableString MultiplayerText => new TranslatableString(GetKey("multiplayer-text"), "Multiplayer");
    public TranslatableString MultiplayerDescription => new TranslatableString(GetKey("multiplayer-description"), "Play against other players");

    public TranslatableString RankingText => new TranslatableString(GetKey("ranking-text"), "Ranking");
    public TranslatableString RankingDescription => new TranslatableString(GetKey("ranking-description"), "Check online leaderboards");

    public TranslatableString BrowseText => new TranslatableString(GetKey("browse-text"), "Browse");
    public TranslatableString BrowseDescription => new TranslatableString(GetKey("browse-description"), "Download community-made maps");

    public TranslatableString EditText => new TranslatableString(GetKey("edit-text"), "Edit");
    public TranslatableString EditDescription => new TranslatableString(GetKey("edit-description"), "Create your own maps");
}
