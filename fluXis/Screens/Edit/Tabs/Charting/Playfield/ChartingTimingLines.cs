using fluXis.Screens.Edit.Tabs.Shared.Lines;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield;

public partial class ChartingTimingLines : EditorTimingLines<ChartingTimingLines.ChartingLine>
{
    public ChartingTimingLines()
    {
        RelativeSizeAxes = Axes.Both;
    }

    protected override ChartingLine CreateLine(double time, bool big, Colour4 color) => new(this, time)
    {
        Height = big ? 5 : 3,
        Colour = color
    };

    protected override Vector2 GetPosition(double time)
    {
        var y = (float)(-EditorHitObjectContainer.HITPOSITION - .5f * ((time - EditorClock.CurrentTime) * Settings.Zoom));
        return new Vector2(0, y);
    }

    public partial class ChartingLine : Line
    {
        public override bool BelowScreen => Parent.EditorClock.CurrentTime >= Time + 1000;
        public override bool AboveScreen => Parent.EditorClock.CurrentTime <= Time - 3000 / Parent.Settings.Zoom;

        public ChartingLine(ChartingTimingLines parent, double time)
            : base(parent, time)
        {
            RelativeSizeAxes = Axes.X;
            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;
        }
    }
}
