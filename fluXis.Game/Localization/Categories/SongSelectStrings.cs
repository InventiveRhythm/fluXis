using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Categories;

public class SongSelectStrings : LocalizationCategory
{
    protected override string File => "song-select";

    public TranslatableString SearchPlaceholder => Get("search-placeholder", "Click to Search...");

    public TranslatableString FooterMods => Get("footer-mods", "Mods");
    public TranslatableString FooterRandom => Get("footer-random", "Random");
    public TranslatableString FooterRewind => Get("footer-rewind", "Rewind");
    public TranslatableString FooterOptions => Get("footer-options", "Options");

    public TranslatableString LeaderboardTitle => Get("leaderboard-title", "Scores");
    public TranslatableString LeaderboardLocal => Get("leaderboard-local", "Local");
    public TranslatableString LeaderboardGlobal => Get("leaderboard-global", "Global");
    public TranslatableString LeaderboardCountry => Get("leaderboard-country", "Country");
    public TranslatableString LeaderboardFriends => Get("leaderboard-friends", "Friends");
    public TranslatableString LeaderboardClub => Get("leaderboard-club", "Club");

    public TranslatableString LeaderboardOutOfDate => Get("leaderboard-out-of-date", "Your local version of this map is out of date!");
    public TranslatableString LeaderboardNoScores => Get("leaderboard-no-scores", "No scores yet!");
    public TranslatableString LeaderboardScoresUnavailable => Get("leaderboard-scores-unavailable", "Scores are not available for this map!");
    public TranslatableString LeaderboardNotUploaded => Get("leaderboard-not-uploaded", "This map is not submitted online!");

    public TranslatableString OptionsSettings => Get("options-settings", "Game Settings");
    public TranslatableString OptionsForAll => Get("options-for-all", "For all difficulties");
    public TranslatableString OptionsForCurrent => Get("options-for-current", "For this difficulty");
    public TranslatableString OptionsDeleteSet => Get("options-delete", "Delete mapset");
    public TranslatableString OptionsWipeScores => Get("options-wipe-scores", "Wipe local scores");

    public TranslatableString WipeScoresConfirmation => Get("wipe-scores-title", "Are you sure you want to wipe all local scores for this difficulty?");

    public TranslatableString NoMapsFound => Get("no-maps-title", "No maps found!");
    public TranslatableString NoMapsFoundDescription => Get("no-maps-description", "Try changing your search filters.");
}
