using fluXis.Game.Map.Structures;

namespace fluXis.Import.osu.Map.Components;

public class OsuHitObject
{
    public int X { get; init; }
    public int StartTime { get; init; }
    public int EndTime { get; init; }
    public string HitSound { get; init; }

    public HitObject ToHitObjectInfo(int keyCount)
    {
        int key = (int)(X * (keyCount / 512f));

        float holdTime = 0;
        if (EndTime > 0) holdTime = EndTime - StartTime;

        if (holdTime < 0) holdTime = 0;

        return new HitObject
        {
            Lane = key + 1,
            Time = StartTime,
            HoldTime = holdTime,
            HitSound = HitSound
        };
    }
}
