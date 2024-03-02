using fluXis.Game.Map.Structures;

namespace fluXis.Import.osu.Map.Components;

public class OsuHitObject
{
    public int X { get; init; }
    public int StartTime { get; init; }
    public int EndTime { get; init; }
    public string HitSound { get; init; }

    public HitObject ToHitObjectInfo(int lane)
    {
        var holdTime = 0f;

        if (EndTime > 0)
            holdTime = EndTime - StartTime;

        if (holdTime < 0)
            holdTime = 0;

        return new HitObject
        {
            Lane = lane + 1,
            Time = StartTime,
            HoldTime = holdTime,
            HitSound = HitSound
        };
    }
}
