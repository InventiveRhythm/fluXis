using fluXis.Graphics.UserInterface.Color;
using JetBrains.Annotations;
using osu.Framework.Graphics;
using Realms;

namespace fluXis.Database.Maps;

public class RealmMapMetadata : RealmObject
{
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Mapper { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public string Audio { get; set; } = string.Empty;
    public int PreviewTime { get; set; }
    public string ColorHex { get; set; }

    [Ignored]
    public Colour4 Color
    {
        get
        {
            if (string.IsNullOrEmpty(ColorHex))
                return FluXisColors.Highlight;

            return Colour4.TryParseHex(ColorHex, out var color) ? color : FluXisColors.Highlight;
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
