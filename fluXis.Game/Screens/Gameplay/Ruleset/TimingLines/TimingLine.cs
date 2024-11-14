using fluXis.Game.Screens.Gameplay.Ruleset.HitObjects;
using fluXis.Game.Screens.Gameplay.Ruleset.Playfields;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;

public partial class TimingLine : Box
{
    [Resolved]
    private Playfield playfield { get; set; }

    private HitObjectColumn column => playfield.Manager[0];

    public double OriginalTime { get; }
    private double scrollVelocityTime;
    private Easing easing = Easing.None;

    public TimingLine(double time)
    {
        OriginalTime = time;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 3;
        Origin = Anchor.BottomLeft;

        scrollVelocityTime = column.ScrollVelocityPositionFromTime(OriginalTime);
        easing = playfield.Manager.EasingAtTime(OriginalTime);
    }

    protected override void Update()
    {
        Y = column.PositionAtTime(scrollVelocityTime, easing);
    }
}
