using fluXis.Modes.Keys.HitObjects;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Modes.Keys.TimingLines;

public partial class TimingLine : Box
{
    [Resolved]
    private Playfield playfield { get; set; }

    [CanBeNull]
    private HitObjectColumn column => (playfield as KeysPlayfield)?.HitManager[0];

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

        scrollVelocityTime = column?.DefaultScrollGroup.PositionFromTime(OriginalTime) ?? OriginalTime;
        easing = (playfield as KeysPlayfield)?.HitManager.EasingAtTime(OriginalTime) ?? Easing.None;
    }

    protected override void Update()
    {
        Y = column?.PositionAtTime(scrollVelocityTime, null, easing) ?? 0;
    }
}
