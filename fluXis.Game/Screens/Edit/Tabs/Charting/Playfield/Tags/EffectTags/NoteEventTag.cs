using fluXis.Game.Map.Structures.Events;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.EffectTags;

public partial class NoteEventTag : EditorTag
{
    public override int TagWidth => 240;
    public override Colour4 TagColour => Colour4.FromHex("#235284");

    private NoteEvent note => (NoteEvent)TimedObject;

    public NoteEventTag(EditorTagContainer parent, NoteEvent timedObject)
        : base(parent, timedObject)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Container.AutoSizeAxes = Axes.X;
        Text.Margin = new MarginPadding { Horizontal = 12 };
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{note.Content}";
    }
}
