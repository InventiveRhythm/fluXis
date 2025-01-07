using System;
using fluXis.Map.Structures;
using osu.Framework.Graphics.Primitives;

namespace fluXis.Import.osu.Map.Components;

public class OsuHitObject
{
    public Vector2I Position { get; init; }
    public float StartTime { get; init; }
    public OsuHitObjectType Type { get; init; }
    public OsuHitSound HitSound { get; init; }
    public string CustomHitSound { get; set; }
    public float EndTime { get; set; }

    public HitObject ToHitObjectInfo(OsuMap map)
    {
        var holdTime = 0f;

        if (EndTime > 0)
            holdTime = EndTime - StartTime;

        if (holdTime < 0)
            holdTime = 0;

        if (string.IsNullOrEmpty(CustomHitSound))
        {
            // whistle and finish doesn't exist in fluXis but no harm in keeping it
            if (HitSound.HasFlag(OsuHitSound.Normal))
                CustomHitSound = ":normal";
            else if (HitSound.HasFlag(OsuHitSound.Whistle))
                CustomHitSound = ":whistle";
            else if (HitSound.HasFlag(OsuHitSound.Finish))
                CustomHitSound = ":finish";
            else if (HitSound.HasFlag(OsuHitSound.Clap))
                CustomHitSound = ":clap";
        }

        return new HitObject
        {
            Time = StartTime,
            Lane = (int)Math.Floor(Position.X * map.CircleSize / 512) + 1,
            HoldTime = holdTime,
            HitSound = CustomHitSound
        };
    }
}

[Flags]
public enum OsuHitObjectType
{
    Circle = 1,
    Slider = 1 << 1,
    NewCombo = 1 << 2,
    Spinner = 1 << 3,
    ComboOffset = (1 << 4) | (1 << 5) | (1 << 6),
    Hold = 1 << 7
}

[Flags]
public enum OsuHitSound
{
    None = 0,
    Normal = 1 << 0,
    Whistle = 1 << 1,
    Finish = 1 << 2,
    Clap = 1 << 3
}
