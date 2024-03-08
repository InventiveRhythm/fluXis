using fluXis.Shared.Replays;
using fluXis.Shared.Scoring.Enums;
using fluXis.Shared.Scoring.Structs;
using Newtonsoft.Json;

namespace fluXis.Shared.Scoring;

public class ScoreInfo
{
    [JsonProperty("accuracy")]
    public float Accuracy { get; set; }

    [JsonProperty("grade")]
    public ScoreRank Rank { get; set; }

    [JsonProperty("score")]
    public int Score { get; set; }

    [JsonProperty("combo")]
    public int Combo { get; set; }

    [JsonProperty("maxCombo")]
    public int MaxCombo { get; set; }

    [JsonProperty("flawless")]
    public int Flawless { get; set; }

    [JsonProperty("perfect")]
    public int Perfect { get; set; }

    [JsonProperty("great")]
    public int Great { get; set; }

    [JsonProperty("alright")]
    public int Alright { get; set; }

    [JsonProperty("okay")]
    public int Okay { get; set; }

    [JsonProperty("miss")]
    public int Miss { get; set; }

    [JsonProperty("results")]
    public List<HitResult> HitResults { get; set; } = new();

    [JsonProperty("mapid")]
    public long MapID { get; set; }

    [JsonProperty("player")]
    public long PlayerID { get; set; }

    [JsonProperty("hash")]
    public string MapHash { get; set; } = "";

    [JsonProperty("mods")]
    public List<string> Mods { get; set; } = new();

    [JsonProperty("scrollspeed")]
    public float ScrollSpeed { get; set; }

    [JsonProperty("time")]
    public long Timestamp { get; set; }

    [JsonProperty("replay")]
    public Replay? Replay { get; set; }

    [JsonIgnore]
    public bool FullFlawless => Flawless == MaxCombo;

    [JsonIgnore]
    public bool FullCombo => Miss == 0;
}
