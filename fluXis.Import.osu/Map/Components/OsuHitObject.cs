using System;
using System.Collections.Generic;
using fluXis.Map.Structures;
using osu.Framework.Graphics.Primitives;

namespace fluXis.Import.osu.Map.Components;

public class OsuHitObject
{
    public Vector2I Position { get; init; }
    public float StartTime { get; init; }
    public OsuHitObjectType Type { get; init; }
    public OsuHitSound HitSound { get; init; }
    public OsuSampleSet SampleSet { get; init; }
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
            if (HitSound.HasFlag(OsuHitSound.Clap))
                CustomHitSound = getSound(map.MapFiles, SampleSet, OsuHitSound.Clap, ":clap");
            else if (HitSound.HasFlag(OsuHitSound.Finish))
                CustomHitSound = getSound(map.MapFiles, SampleSet, OsuHitSound.Finish, ":finish");
            else if (HitSound.HasFlag(OsuHitSound.Whistle))
                CustomHitSound = getSound(map.MapFiles, SampleSet, OsuHitSound.Whistle, ":whistle");
            else
                CustomHitSound = getSound(map.MapFiles, SampleSet, OsuHitSound.Normal, ":normal");
        }

        return new HitObject
        {
            Time = StartTime,
            Lane = (int)Math.Floor(Position.X * map.CircleSize / 512) + 1,
            HoldTime = holdTime,
            HitSound = CustomHitSound
        };
    }

    private string getSound(List<string> files, OsuSampleSet set, OsuHitSound sound, string fallback)
    {
        var prefix = set switch
        {
            OsuSampleSet.Normal => "normal",
            OsuSampleSet.Soft => "soft",
            OsuSampleSet.Drum => "drum",
            _ => "normal"
        };

        var name = $"{prefix}-";

        switch (sound)
        {
            case OsuHitSound.Normal:
                name += "hitnormal";
                break;

            case OsuHitSound.Whistle:
                name += "hitwhistle";
                break;

            case OsuHitSound.Finish:
                name += "hitfinish";
                break;

            case OsuHitSound.Clap:
                name += "hitclap";
                break;

            default:
                return fallback;
        }

        name += ".wav";

        if (files.Contains(name))
            return name;

        return fallback;
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

public enum OsuSampleSet
{
    None = 0,
    Normal = 1,
    Soft = 2,
    Drum = 3
}
