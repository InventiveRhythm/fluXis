using System.Collections.Generic;
using fluXis.Replays;
using fluXis.Scoring.Enums;
using Newtonsoft.Json;
using rhym;

namespace fluXis.Online.API.Payloads.Scores;

public class ScoreSubmissionPayload
{
    [JsonProperty("ticket")]
    public string Ticket { get; set; }

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
        [JsonProperty("judgement")]
        public Judgement Judgement { get; set; }

        [JsonProperty("type")]
        public ResourceLocation Type { get; set; }
    }
}
