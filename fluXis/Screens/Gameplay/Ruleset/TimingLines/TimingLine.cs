using fluXis.Screens.Gameplay.Ruleset.HitObjects;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Gameplay.Ruleset.TimingLines;

public partial class TimingLine : Box
{
    [Resolved]
    private Playfield playfield { get; set; }

    private HitObjectColumn column => playfield.HitManager[0];

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

        scrollVelocityTime = column.DefaultScrollGroup.PositionFromTime(OriginalTime);
        easing = playfield.HitManager.EasingAtTime(OriginalTime);
    }

    protected override void Update()
    {
        Y = column.PositionAtTime(scrollVelocityTime, null, easing);
    }
}
