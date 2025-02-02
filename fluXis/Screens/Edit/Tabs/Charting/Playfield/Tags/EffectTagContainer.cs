using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags.EffectTags;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EffectTagContainer : EditorTagContainer
{
    protected override bool RightSide => true;

    protected override void LoadComplete()
    {
        Map.NoteEventAdded += addNote;
        Map.NoteEventRemoved -= RemoveTag;
        Map.MapEvents.NoteEvents.ForEach(addNote);
    }

    private void addNote(NoteEvent note) => AddTag(new NoteEventTag(this, note));
}
