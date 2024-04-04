using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Skinning.Default;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Map;

[JsonObject(MemberSerialization.OptIn)]
public class MapColors : ICustomColorProvider
{
    [JsonProperty("accent")]
    public string AccentHex { get; set; } = "";

    [JsonProperty("primary")]
    public string PrimaryHex { get; set; } = "";

    [JsonProperty("secondary")]
    public string SecondaryHex { get; set; } = "";

    [JsonProperty("middle")]
    public string MiddleHex { get; set; } = "";

    public override string ToString()
        => $"Accent: {AccentHex}, Primary: {PrimaryHex}, Secondary: {SecondaryHex}, Middle: {MiddleHex}";

    public Colour4 Accent
    {
        get
        {
            if (string.IsNullOrEmpty(AccentHex))
                return FluXisColors.Accent2;

            return Colour4.TryParseHex(AccentHex, out var color) ? color : FluXisColors.Accent2;
        }
        set => AccentHex = value.ToHex();
    }

    public Colour4 Primary
    {
        get
        {
            if (string.IsNullOrEmpty(PrimaryHex))
                return Colour4.Transparent;

            return Colour4.TryParseHex(PrimaryHex, out var color) ? color : Colour4.Transparent;
        }
    }

    public Colour4 Secondary
    {
        get
        {
            if (string.IsNullOrEmpty(SecondaryHex))
                return Colour4.Transparent;

            return Colour4.TryParseHex(SecondaryHex, out var color) ? color : Colour4.Transparent;
        }
    }

    public Colour4 Middle
    {
        get
        {
            if (string.IsNullOrEmpty(MiddleHex))
                return Colour4.Transparent;

            return Colour4.TryParseHex(MiddleHex, out var color) ? color : Colour4.Transparent;
        }
    }

    public bool HasColorFor(int lane, int keyCount, out Colour4 colour)
    {
        var index = FluXisColors.GetLaneColorIndex(lane, keyCount);
        colour = GetColor(index);
        return colour != Colour4.Transparent;
    }

    public Colour4 GetColor(int index)
    {
        var colors = new[]
        {
            Colour4.Transparent,
            Primary,
            Secondary,
            Middle
        };

        return colors[index];
    }
}
