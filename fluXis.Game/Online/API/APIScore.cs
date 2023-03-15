using System.Collections.Generic;
using fluXis.Game.Scoring;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API;

public class APIScore
{
    [JsonProperty("score")]
    public int Score { get; set; }

    [JsonProperty("accuracy")]
    public float Accuracy { get; set; }

    [JsonProperty("maxCombo")]
    public int MaxCombo { get; set; }

    [JsonProperty("judgements")]
    public Dictionary<string, int> Judgements { get; set; }

    [JsonProperty("hitStats")]
    public List<HitStat> HitStats { get; set; }

    [JsonProperty("mapid")]
    public int MapID { get; set; }

    [JsonProperty("maphash")]
    public string MapHash { get; set; }

    [JsonProperty("playerid")]
    public int PlayerID { get; set; }
}
