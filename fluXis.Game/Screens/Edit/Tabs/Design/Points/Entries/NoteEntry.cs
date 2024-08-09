using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class NoteEntry : PointListEntry
{
    protected override string Text => "Note";
    protected override Colour4 Color => FluXisColors.Note;

    private NoteEvent note => (NoteEvent)Object;

    public NoteEntry(NoteEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => new NoteEvent
    {
        Time = note.Time,
        Content = note.Content
    };

    protected override Drawable[] CreateValueContent() => new Drawable[] { new FluXisSpriteText { Text = note.Content } };

    protected override IEnumerable<Drawable> CreateSettings() => base.CreateSettings().Concat(new Drawable[]
    {
        new PointSettingsTextBox
        {
            Text = "Content",
            TooltipText = "The content of the note.",
            DefaultText = note.Content,
            OnTextChanged = t =>
            {
                note.Content = t.Text;
                Map.Update(note);
            }
        }
    });
}
