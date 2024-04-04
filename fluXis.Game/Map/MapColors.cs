using fluXis.Game.Graphics.UserInterface.Color;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Map;

[JsonObject(MemberSerialization.OptIn)]
public class MapColors
{
    [JsonProperty("accent")]
    public string AccentHex { get; set; } = "";

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
}
