using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.UI.BottomBar.Timeline.Tags;

public partial class TimelineNoteTag : TimelineTag
{
    public override Colour4 TagColour => Theme.NoteTag;

    public NoteEvent Note => (NoteEvent)TimedObject;

    public TimelineNoteTag(EditorClock clock, NoteEvent note)
        : base(clock, note)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = Note.Content;
    }
}