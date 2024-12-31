using JetBrains.Annotations;
using osu.Framework.Localisation;

namespace fluXis.Localization.Categories;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class SongSelectStrings : LocalizationCategory
{
    protected override string File => "song-select";

    private static SongSelectStrings instance;

    public SongSelectStrings()
    {
        instance = this;
    }

    public TranslatableString SearchPlaceholder => Get("search-placeholder", "Click to Search...");

    // for the LocalisableString attribute to
    // work the fields have to be static...
    public TranslatableString SortBy => Get("sort-by", "sort by");
    public static LocalisableString SortByTitle => instance.Get("sort-by-title", "Title");
    public static LocalisableString SortByArtist => instance.Get("sort-by-artist", "Artist");
    public static LocalisableString SortByLength => instance.Get("sort-by-length", "Length");
    public static LocalisableString SortByDateAdded => instance.Get("sort-by-date-added", "Date Added");
    public static LocalisableString SortByDifficulty => instance.Get("sort-by-difficulty", "Difficulty");

    public TranslatableString GroupBy => Get("group-by", "group by");
    public static LocalisableString GroupByDefault => instance.Get("group-by-default", "Default");
    public static LocalisableString GroupByNothing => instance.Get("group-by-nothing", "Nothing");

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
