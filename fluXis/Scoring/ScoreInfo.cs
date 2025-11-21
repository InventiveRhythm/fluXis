using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace fluXis.Scoring;

#nullable enable

public class ScoreInfo
{
    [JsonProperty("mapid")]
    public long MapID { get; set; }

    [JsonProperty("mods")]
    public List<string> Mods { get; set; } = new();

    [JsonProperty("time")]
    public long Timestamp { get; set; }

    [JsonProperty("players")]
    public List<PlayerScore> Players { get; set; } = new();

    [JsonIgnore]
    public float Rate
    {
        get
        {
            var rate = Mods.FirstOrDefault(x => x.EndsWith('x')) ?? "1.0";
            rate = rate.TrimEnd('x');

            return float.TryParse(rate, out var result) ? result : 1;
        }
    }
}
