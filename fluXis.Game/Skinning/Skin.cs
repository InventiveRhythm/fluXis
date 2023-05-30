using Newtonsoft.Json;

namespace fluXis.Game.Skinning;

public class Skin
{
    [JsonProperty("column_width")]
    public int ColumnWidth { get; init; } = 114;

    [JsonProperty("hit_position")]
    public int HitPosition { get; init; } = 130;
}
