using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.Ruleset.TimingLines;

public partial class TimingLine : Box
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    private readonly TimingLineManager manager;
    public double ScrollVelocityTime { get; }

    public TimingLine(TimingLineManager manager, double time)
    {
        this.manager = manager;
        ScrollVelocityTime = time;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 3;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.Centre;
    }

    protected override void Update()
    {
        double delta = ScrollVelocityTime - manager.HitObjectManager.CurrentTime;
        var receptor = manager.HitObjectManager.Playfield.Receptors[0];
        float hitY = receptor.Y - skinManager.SkinJson.GetKeymode(manager.HitObjectManager.Map.KeyCount).HitPosition;
        Y = (float)(hitY - 0.5f * (delta * manager.HitObjectManager.ScrollSpeed));

        base.Update();
    }
}
