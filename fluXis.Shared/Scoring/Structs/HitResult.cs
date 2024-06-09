using fluXis.Shared.Scoring.Enums;
using Newtonsoft.Json;

namespace fluXis.Shared.Scoring.Structs;

public class HitResult
{
    [JsonProperty("time")]
    public double Time { get; }

    [JsonProperty("difference")]
    public double Difference { get; }

    [JsonProperty("judgement")]
    public Judgement Judgement { get; }

    public HitResult(double time, double diff, Judgement jud)
    {
        Time = time;
        Difference = diff;
        Judgement = jud;
    }
}
