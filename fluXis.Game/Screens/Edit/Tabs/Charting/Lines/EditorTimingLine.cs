using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Lines;

public partial class EditorTimingLine : Box
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    public new float Time { get; set; }

    public bool IsOnScreen
    {
        get
        {
            if (clock == null) return true;

            return clock.CurrentTime <= Time + 1000 && clock.CurrentTime >= Time - 3000 / values.Zoom;
        }
    }

    public EditorTimingLine()
    {
        RelativeSizeAxes = Axes.X;
        Height = 1;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
    }

    protected override void Update()
    {
        Y = -EditorHitObjectContainer.HITPOSITION - .5f * ((Time - (float)clock.CurrentTime) * values.Zoom);
    }
}
