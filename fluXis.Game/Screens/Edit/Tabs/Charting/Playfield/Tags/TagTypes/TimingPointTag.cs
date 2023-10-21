using fluXis.Game.Map;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.TagTypes;

public partial class TimingPointTag : EditorTag
{
    public override Colour4 TagColour => Colour4.FromHex("#00FF80");

    public TimingPointInfo TimingPoint => (TimingPointInfo)TimedObject;

    public TimingPointTag(EditorTagContainer parent, TimingPointInfo tp)
        : base(parent, tp)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{(int)TimingPoint.BPM}BPM";
    }
}
