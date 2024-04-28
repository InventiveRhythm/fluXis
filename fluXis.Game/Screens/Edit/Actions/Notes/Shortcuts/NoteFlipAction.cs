using System.Collections.Generic;
using fluXis.Game.Map.Structures;

namespace fluXis.Game.Screens.Edit.Actions.Notes.Shortcuts;

public class NoteFlipAction : EditorAction
{
    public override string Description => "Flip Selection";

    private IEnumerable<HitObject> notes { get; }
    private int keyCount { get; }

    public NoteFlipAction(IEnumerable<HitObject> notes, int keyCount)
    {
        this.notes = notes;
        this.keyCount = keyCount;
    }

    public override void Run(EditorMap map)
    {
        foreach (var note in notes)
        {
            note.Lane = keyCount - note.Lane + 1;
            map.Update(note);
        }
    }

    public override void Undo(EditorMap map)
    {
        foreach (var note in notes)
        {
            note.Lane = keyCount - note.Lane + 1;
            map.Update(note);
        }
    }
}
