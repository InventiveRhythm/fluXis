using System;
using Realms;

namespace fluXis.Game.Database.Maps;

/**
 * Used to filter maps in the song selection screen.
 */
public class RealmMapFilters : RealmObject
{
    public float Length { get; set; }
    public float BPMMin { get; set; }
    public float BPMMax { get; set; }

    public int NoteCount { get; set; }
    public int LongNoteCount { get; set; }
    public float NotesPerSecond { get; set; }

    [Ignored]
    public float LongNotePercentage
    {
        get
        {
            if (NoteCount + LongNoteCount == 0)
                return 0;

            if (LongNoteCount == 0)
                return 0;

            return (float)LongNoteCount / (NoteCount + LongNoteCount);
        }
    }

    [MapTo(nameof(Effects))]
    public string EffectsString { get; set; } = string.Empty;

    [Ignored]
    public EffectType Effects
    {
        get
        {
            if (string.IsNullOrEmpty(EffectsString))
                return 0;

            return (EffectType)Enum.Parse(typeof(EffectType), EffectsString);
        }
        set => EffectsString = ((ulong)value).ToString();
    }

    public void Reset()
    {
        Length = 0;
        BPMMin = 0;
        BPMMax = 0;
        NoteCount = 0;
        LongNoteCount = 0;
        NotesPerSecond = 0;
        Effects = 0;
    }
}

[Flags]
public enum EffectType : ulong
{
    ScrollVelocity = 1 << 0, // 1
    LaneSwitch = 1 << 1, // 2
    Flash = 1 << 2, // 4
    Pulse = 1 << 3, // 8
    PlayfieldMove = 1 << 4, // 16
    PlayfieldScale = 1 << 5, // 32
    PlayfieldRotate = 1 << 6, // 64
    PlayfieldFade = 1 << 7, // 128
    Shake = 1 << 8, // 256
    Shader = 1 << 9, // 512
    BeatPulse = 1 << 10, // 1024
}
