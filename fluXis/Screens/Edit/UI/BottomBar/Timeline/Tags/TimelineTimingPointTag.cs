using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.UI.BottomBar.Timeline.Tags;

public partial class TimelineTimingPointTag : TimelineTag
{
    public override Colour4 TagColour => Theme.TimingPoint;

    public TimingPoint TimingPoint => (TimingPoint)TimedObject;

    public TimelineTimingPointTag(EditorClock clock, TimingPoint tp)
        : base(clock, tp)
    {
    }

    protected override void Update()
    {
        base.Update();
        Text.Text = $"{(int)TimingPoint.BPM} BPM";
    }
}