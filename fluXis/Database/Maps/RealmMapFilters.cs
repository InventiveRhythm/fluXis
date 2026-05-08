using System;
using fluXis.Online.API.Models.Maps;
using Realms;

namespace fluXis.Database.Maps;

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
    public int LandmineCount { get; set; }
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
    public MapEffectType Effects
    {
        get
        {
            if (string.IsNullOrEmpty(EffectsString))
                return 0;

            return (MapEffectType)Enum.Parse(typeof(MapEffectType), EffectsString);
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
        LandmineCount = 0;
        NotesPerSecond = 0;
        Effects = 0;
    }
}

