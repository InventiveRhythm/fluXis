using fluXis.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Screens.Menu.UI.Snow;

public partial class MenuSnow : Container
{
    [Resolved]
    private FluXisGameBase game { get; set; }

    private const int snow_count = 200;
    private float xStrength;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;

        for (int i = 0; i < snow_count; i++)
        {
            Add(new SnowParticle
            {
                Position = new Vector2(RNG.NextSingle(), RNG.NextSingle()),
                Scale = new Vector2(RNG.NextSingle(0.5f, 1.5f)),
            });
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        // to avoid making it move around
        // with the toolbar
        Size = game.ContentSize;

        // we can't do this in load because the DrawSize is not set.
        foreach (var drawable in InternalChildren)
            drawable.Position *= DrawSize;
    }

    protected override void Update()
    {
        base.Update();

        Size = game.ContentSize;

        foreach (var drawable in InternalChildren)
        {
            drawable.X += xStrength * DrawWidth * (float)Time.Elapsed / 1000 * 100;
            drawable.Y += drawable.Scale.Y * (float)Time.Elapsed / 1000 * 100;

            if (drawable.X > DrawWidth + 20) drawable.X = -20;
            if (drawable.X < -20) drawable.X = DrawWidth + 20;

            if (drawable.Y > DrawHeight + 20)
            {
                drawable.X = RNG.NextSingle() * DrawWidth;
                drawable.Y = -20;
            }
        }
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        var xFromCenter = (e.MousePosition.X - DrawWidth / 2) / (DrawWidth / 2);
        xStrength = xFromCenter / DrawWidth;

        return base.OnMouseMove(e);
    }

    private partial class SnowParticle : Circle
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Origin = Anchor.Centre;
            Size = new Vector2(10);
            EdgeEffect = FluXisStyles.SnowShadow;
        }
    }
}
