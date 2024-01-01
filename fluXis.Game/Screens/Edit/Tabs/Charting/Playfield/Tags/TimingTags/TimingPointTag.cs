using fluXis.Game.Map.Structures;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

public partial class TimingPointTag : EditorTag
{
    public override Colour4 TagColour => Colour4.FromHex("#00FF80");

    public TimingPoint TimingPoint => (TimingPoint)TimedObject;

    public TimingPointTag(EditorTagContainer parent, TimingPoint tp)
        : base(parent, tp)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{(int)TimingPoint.BPM}BPM";
    }
}
