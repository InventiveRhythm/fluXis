using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures;

namespace fluXis.Screens.Edit.Actions.Notes.Shortcuts;

public class NoteApplyGroupAction : EditorAction
{
    public override string Description => $"Apply group '{group}' to {notes.Length} note(s).";

    private readonly HitObject[] notes;
    private readonly string[] oldGroups;
    private readonly string group;

    public NoteApplyGroupAction(IEnumerable<HitObject> notes, string group)
    {
        this.notes = notes.ToArray();
        oldGroups = this.notes.Select(x => x.Group).ToArray();
        this.group = group;
    }

    public override void Run(EditorMap map)
    {
        foreach (var note in notes)
        {
            note.Group = group;
            map.Update(note);
        }
    }

    public override void Undo(EditorMap map)
    {
        for (var i = 0; i < notes.Length; i++)
        {
            notes[i].Group = oldGroups[i];
            map.Update(notes[i]);
        }
    }
}
