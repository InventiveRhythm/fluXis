using fluXis.Shared.Scoring.Enums;
using Newtonsoft.Json;

namespace fluXis.Shared.Scoring.Structs;

public class HitResult
{
    [JsonProperty("time")]
    public float Time { get; }

    [JsonProperty("difference")]
    public float Difference { get; }

    [JsonProperty("judgement")]
    public Judgement Judgement { get; }

    public HitResult(float time, float diff, Judgement jud)
    {
        Time = time;
        Difference = diff;
        Judgement = jud;
    }
}
