using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;

public partial class TimingLine : Box
{
    [Resolved]
    private Playfield playfield { get; set; }

    public float OriginalTime { get; }
    private double scrollVelocityTime;

    public TimingLine(double time)
    {
        OriginalTime = (float)time;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 3;
        Origin = Anchor.BottomLeft;

        scrollVelocityTime = playfield.Manager.ScrollVelocityPositionFromTime(OriginalTime);
    }

    protected override void Update()
    {
        Y = playfield.Manager.PositionAtTime(scrollVelocityTime);
    }
}
