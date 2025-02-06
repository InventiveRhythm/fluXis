using fluXis.Online.API.Models.Users;
using Newtonsoft.Json;
using osu.Framework.Localisation;

namespace fluXis.Online.API.Models.Maps;

#nullable enable

public class APIMap
{
    [JsonProperty("id")]
    public long ID { get; init; }

    [JsonProperty("mapset")]
    public long MapSetID { get; init; }

    [JsonProperty("mapper")]
    public APIUser Mapper { get; init; } = null!;

    [JsonProperty("hash")]
    public string SHA256Hash { get; init; } = null!;

    [JsonProperty("difficulty")]
    public string Difficulty { get; init; } = null!;

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("title-rm")]
    public string TitleRomanized { get; set; } = string.Empty;

    [JsonProperty("artist")]
    public string Artist { get; set; } = string.Empty;

    [JsonProperty("artist-rm")]
    public string ArtistRomanized { get; set; } = string.Empty;

    [JsonProperty("source")]
    public string Source { get; init; } = null!;

    [JsonProperty("tags")]
    public string Tags { get; init; } = null!;

    [JsonProperty("mode")]
    public int Mode { get; init; }

    [JsonProperty("status")]
    public int Status { get; init; }

    [JsonProperty("bpm")]
    public double BPM { get; init; }

    [JsonProperty("length")]
    public int Length { get; init; }

    [JsonProperty("notes")]
    public int NoteCount { get; init; }

    [JsonProperty("long-notes")]
    public int LongNoteCount { get; init; }

    [JsonProperty("maxcombo")]
    public int MaxCombo { get; init; }

    [JsonProperty("nps")]
    public double NotesPerSecond { get; init; }

    [JsonProperty("rating")]
    public double Rating { get; init; }

    [JsonProperty("effects")]
    public MapEffectType Effects { get; init; }

    [JsonProperty("ups")]
    public long UpVotes { get; init; }

    [JsonProperty("downs")]
    public long DownVotes { get; init; }

    #region Optional

    [JsonProperty("file")]
    public string? FileName { get; set; }

    #endregion

    #region Localization

    [JsonIgnore]
    public RomanisableString LocalizedTitle => new(Title, TitleRomanized);

    [JsonIgnore]
    public RomanisableString LocalizedArtist => new(Artist, ArtistRomanized);

    #endregion

    public static APIMap CreateUnknown(long id, long mapper = 0) => new()
    {
        ID = id,
        Title = $"Unknown Map {id}",
        Artist = "Unknown Artist",
        Source = "Unknown Source",
        Tags = "",
        Difficulty = "Unknown",
        Mapper = APIUser.CreateUnknown(mapper)
    };
}
