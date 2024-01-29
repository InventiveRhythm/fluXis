using fluXis.Game.Graphics.UserInterface.Color;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;

namespace fluXis.Game.Online.API.Models.Other;

public class Achievement
{
    [JsonProperty("id")]
    public string ID { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonIgnore]
    public ColourInfo Color
    {
        get
        {
            if (Level == 1)
                return Colour4.FromHex("#bf8970");
            if (Level == 2)
                return Colour4.FromHex("#d4af37");
            if (Level == 3)
                return getHighestColor();

            return FluXisColors.Highlight;
        }
    }

    private static ColourInfo getHighestColor() => new()
    {
        TopLeft = Colour4.FromHex("#c2752c"),
        TopRight = Colour4.FromHex("#4cb5d6"),
        BottomLeft = Colour4.FromHex("#c37182"),
        BottomRight = Colour4.FromHex("#b570e8")
    };
}
