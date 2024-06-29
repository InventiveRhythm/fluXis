using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using SixLabors.ImageSharp;

namespace fluXis.Game.Skinning.Json;

[JsonObject(MemberSerialization.OptIn)]
public class SkinSnapColors : IDeepCloneable<SkinSnapColors>
{
    [JsonProperty("1/3")]
    public string Third { get; set; } = "#FF5555";

    [JsonProperty("1/4")]
    public string Fourth { get; set; } = "#558EFF";

    [JsonProperty("1/6")]
    public string Sixth { get; set; } = "#8EFF55";

    [JsonProperty("1/8")]
    public string Eighth { get; set; } = "#FFE355";

    [JsonProperty("1/12")]
    public string Twelfth { get; set; } = "#C655FF";

    [JsonProperty("1/16")]
    public string Sixteenth { get; set; } = "#55FFAA";

    [JsonProperty("1/24")]
    public string TwentyFourth { get; set; } = "#FF55AA";

    [JsonProperty("1/48")]
    public string FortyEighth { get; set; } = "#BFBFBF";

    public string[] All => new[]
    {
        Third,
        Fourth,
        Sixth,
        Eighth,
        Twelfth,
        Sixteenth,
        TwentyFourth,
        FortyEighth
    };

    private Dictionary<string, Colour4> cache { get; } = new();

    public Colour4 GetColor(int index)
    {
        if (index < 0 || index >= All.Length)
            return Colour4.White;

        var hex = All[index];

        if (cache.TryGetValue(hex, out var color))
            return color;

        if (!Colour4.TryParseHex(hex, out color))
            return Colour4.White;

        cache.Add(hex, color);
        return color;
    }

    public SkinSnapColors DeepClone() => new()
    {
        Third = Third,
        Fourth = Fourth,
        Sixth = Sixth,
        Eighth = Eighth,
        Twelfth = Twelfth,
        Sixteenth = Sixteenth,
        TwentyFourth = TwentyFourth,
        FortyEighth = FortyEighth
    };
}
