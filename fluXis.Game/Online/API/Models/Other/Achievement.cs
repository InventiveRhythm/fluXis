using fluXis.Game.Graphics.UserInterface.Color;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Online.API.Models.Other;

public class Achievement
{
    [JsonProperty("id")]
    public string ID { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("color")]
    public string ColorHex { get; set; }

    [JsonIgnore]
    public Colour4 Color
    {
        get
        {
            if (string.IsNullOrEmpty(ColorHex))
                return FluXisColors.Highlight;

            return Colour4.TryParseHex(ColorHex, out var c) ? c : FluXisColors.Highlight;
        }
    }
}
