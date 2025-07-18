using fluXis.Graphics.UserInterface.Color;
using JetBrains.Annotations;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using Realms;

namespace fluXis.Database.Maps;

public class RealmMapMetadata : RealmObject
{
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string TitleRomanized { get; set; } = string.Empty;
    public string ArtistRomanized { get; set; } = string.Empty;
    public string Mapper { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public string Audio { get; set; } = string.Empty;
    public int PreviewTime { get; set; }
    public string ColorHex { get; set; }

    [Ignored]
    public RomanisableString LocalizedTitle => new(Title, TitleRomanized);

    [Ignored]
    public RomanisableString LocalizedArtist => new(Artist, ArtistRomanized);

    [Ignored]
    public string SortingTitle => string.IsNullOrWhiteSpace(TitleRomanized) ? Title : TitleRomanized;

    [Ignored]
    public string SortingArtist => string.IsNullOrWhiteSpace(ArtistRomanized) ? Artist : ArtistRomanized;

    [Ignored]
    public Colour4 Color
    {
        get
        {
            if (string.IsNullOrEmpty(ColorHex))
                return Theme.Highlight;

            return Colour4.TryParseHex(ColorHex, out var color) ? color : Theme.Highlight;
        }
        set => ColorHex = value.ToHex();
    }

    [UsedImplicitly]
    public RealmMapMetadata()
    {
    }

    public override string ToString()
    {
        return $"{Title} - {Artist} - {Mapper} - {Source} - {Tags} - {Background} - {Audio} - {PreviewTime}";
    }
}
