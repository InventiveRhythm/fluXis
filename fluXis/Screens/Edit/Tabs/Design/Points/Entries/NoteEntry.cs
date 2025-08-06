using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Utils;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Design.Points.Entries;

public partial class NoteEntry : PointListEntry
{
    protected override string Text => "Note";
    protected override Colour4 Color => Theme.Note;

    private NoteEvent note => (NoteEvent)Object;

    public NoteEntry(NoteEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => note.JsonCopy();

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
