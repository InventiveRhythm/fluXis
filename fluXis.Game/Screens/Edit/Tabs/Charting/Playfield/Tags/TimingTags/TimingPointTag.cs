using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

public partial class TimingPointTag : EditorTag
{
    public override Colour4 TagColour => FluXisColors.TimingPoint;

    [Resolved]
    private PointsSidebar points { get; set; }

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

    protected override bool OnClick(ClickEvent e)
    {
        points.ShowPoint(TimingPoint);
        return true;
    }
}
