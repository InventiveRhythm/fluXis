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

    // gimmick stuffs
    public bool HasScrollVelocity { get; set; }
    public bool HasLaneSwitch { get; set; }
    public bool HasFlash { get; set; }
}
