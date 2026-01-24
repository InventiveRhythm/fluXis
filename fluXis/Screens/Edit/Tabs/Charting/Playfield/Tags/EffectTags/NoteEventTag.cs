using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags.EffectTags;

public partial class NoteEventTag : EditorTag
{
    public override Colour4 TagColour => Theme.NoteTag;

    private NoteEvent note => (NoteEvent)TimedObject;

    public NoteEventTag(EditorTagContainer parent, NoteEvent timedObject)
        : base(parent, timedObject)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{note.Content}";
    }

    protected override bool OnClick(ClickEvent e)
    {
        Editor.ChangeToTab<DesignTab>(x => x.Container.Sidebar.ShowPoint(note));
        return true;
    }
}
