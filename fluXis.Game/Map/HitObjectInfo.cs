using Newtonsoft.Json;

namespace fluXis.Game.Map;

public class HitObjectInfo : TimedObject
{
    public int Lane;
    public float HoldTime;

    [JsonIgnore]
    public float HoldEndTime
    {
        get
        {
            if (HoldTime <= 0)
                return Time;

            return Time + HoldTime;
        }
        set => HoldTime = value - Time;
    }

    public bool IsLongNote()
    {
        return HoldTime > 0;
    }

    public HitObjectInfo Copy()
    {
        return new HitObjectInfo
        {
            Time = Time,
            Lane = Lane,
            HoldTime = HoldTime
        };
    }

    public override string ToString()
    {
        return $"Time: {Time}, Lane: {Lane}, HoldTime: {HoldTime}";
    }
}
