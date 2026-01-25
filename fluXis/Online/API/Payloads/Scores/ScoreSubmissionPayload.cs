using System.Collections.Generic;
using fluXis.Replays;
using fluXis.Scoring.Enums;
using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Scores;

public class ScoreSubmissionPayload
{
    [JsonProperty("hash")]
    public string MapHash { get; set; }

    [JsonProperty("effect-hash")]
    public string EffectHash { get; set; }

    [JsonProperty("sb-hash")]
    public string StoryboardHash { get; set; }

    [JsonProperty("mods")]
    public List<string> Mods { get; set; }

    [JsonProperty("players")]
    public List<Player> Scores { get; set; }

    [JsonProperty("replay")]
    public Replay Replay { get; set; }

    public class Player
    {
        [JsonProperty("user")]
        public long UserID { get; set; }

        [JsonProperty("scroll")]
        public float ScrollSpeed { get; set; }

        [JsonProperty("results")]
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        [JsonProperty("diff")]
        public double Difference { get; set; }

        [JsonProperty("type")]
        public ResultType Type { get; set; }
    }
}
