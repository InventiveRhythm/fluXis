using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Structures;
using osu.Framework.Utils;

namespace fluXis.Game.Screens.Edit.Actions.Notes.Shortcuts;

public class NoteShuffleAction : EditorAction
{
    public override string Description => $"Shuffle {notes.Count} note(s)";

    private List<HitObject> notes { get; }
    private int keyCount { get; }

    private Dictionary<HitObject, int> oldLanes { get; }
    private Dictionary<HitObject, int> newLanes { get; } = new();

    public NoteShuffleAction(List<HitObject> notes, int keyCount)
    {
        this.notes = notes;
        this.keyCount = keyCount;

        oldLanes = notes.ToDictionary(n => n, n => n.Lane);
        shuffle();
    }

    private void shuffle()
    {
        foreach (var note in notes)
        {
            var onSameTime = newLanes.Where(n => n.Key.Time == note.Time).ToList();

            if (onSameTime.Count == 0)
            {
                newLanes[note] = RNG.Next(1, keyCount + 1);
                continue;
            }

            var lanes = onSameTime.Select(n => n.Value).ToList();
            var lane = RNG.Next(1, keyCount + 1);

            while (lanes.Contains(lane))
                lane = RNG.Next(1, keyCount + 1);

            newLanes[note] = lane;
        }
    }

    public override void Run(EditorMap map)
    {
        foreach (var note in notes)
        {
            note.Lane = newLanes[note];
            map.Update(note);
        }
    }

    public override void Undo(EditorMap map)
    {
        foreach (var note in notes)
        {
            note.Lane = oldLanes[note];
            map.Update(note);
        }
    }
}
