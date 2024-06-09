using fluXis.Shared.Scoring.Structs;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures;

public class HitObject : ITimedObject
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("holdtime")]
    public double HoldTime { get; set; }

    [JsonProperty("hitsound")]
    public string HitSound { get; set; }

    /// <summary>
    /// 0 = Normal / Long
    /// 1 = Tick
    /// </summary>
    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonIgnore]
    public bool LongNote => HoldTime > 0;

    [JsonIgnore]
    public double EndTime
    {
        get
        {
            if (HoldTime <= 0)
                return Time;

            return Time + HoldTime;
        }
        set => HoldTime = value - Time;
    }

    [JsonIgnore]
    public HitResult Result { get; set; }

    [JsonIgnore]
    public HitResult HoldEndResult { get; set; }

    public HitObject Copy()
    {
        return new HitObject
        {
            Time = Time,
            Lane = Lane,
            HoldTime = HoldTime,
            HitSound = HitSound,
            Type = Type
        };
    }

    public override string ToString()
    {
        return $"Time: {Time}, Lane: {Lane}, HoldTime: {HoldTime}, HitSound: {HitSound}";
    }
}
