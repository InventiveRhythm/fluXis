using System;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.UI.BottomBar.Timeline.Tags;

public partial class TimelineTimingPointTag : TimelineTag
{
    public override Colour4 TagColour => Theme.TimingPoint;
    protected override Action UpdateAction => () => Text.Text = $"{(int)TimingPoint.BPM} BPM";

    public TimingPoint TimingPoint => (TimingPoint)TimedObject;

    public TimelineTimingPointTag(EditorClock clock, TimingPoint tp)
        : base(clock, tp)
    {
    }
}