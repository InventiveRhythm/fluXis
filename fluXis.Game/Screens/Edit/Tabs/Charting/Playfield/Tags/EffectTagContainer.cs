using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.EffectTags;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EffectTagContainer : EditorTagContainer
{
    protected override bool RightSide => true;

    protected override void LoadComplete()
    {
        Map.ShakeEventAdded += addShake;
        Map.ShakeEventRemoved += RemoveTag;
        Map.MapEvents.ShakeEvents.ForEach(addShake);

        Map.NoteEventAdded += addNote;
        Map.NoteEventRemoved -= RemoveTag;
        Map.MapEvents.NoteEvents.ForEach(addNote);
    }

    private void addShake(ShakeEvent shake) => AddTag(new ShakeEventTag(this, shake));
    private void addNote(NoteEvent note) => AddTag(new NoteEventTag(this, note));
}
