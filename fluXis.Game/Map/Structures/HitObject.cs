using fluXis.Game.Scoring.Structs;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures;

public class HitObject : TimedObject
{
    public int Lane { get; set; }
    public float HoldTime { get; set; }
    public string HitSound { get; set; }

    [JsonIgnore]
    public bool LongNote => HoldTime > 0;

    [JsonIgnore]
    public float EndTime
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
            HitSound = HitSound
        };
    }

    public override string ToString()
    {
        return $"Time: {Time}, Lane: {Lane}, HoldTime: {HoldTime}, HitSound: {HitSound}";
    }
}
