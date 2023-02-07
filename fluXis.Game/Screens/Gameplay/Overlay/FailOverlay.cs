using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Overlay;

public partial class FailOverlay : Container
{
    private readonly StripePattern pattern;
    private readonly SpriteText text;

    public FailOverlay()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        Children = new Drawable[]
        {
            pattern = new StripePattern
            {
                Speed = new Vector2(-300)
            },
            text = new SpriteText
            {
                Text = "FAILED",
                Font = new FontUsage("Quicksand", 100, "Bold"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            }
        };
    }

    public override void Show()
    {
        this.FadeIn(200);
        pattern.SpeedTo(new Vector2(-50), 500, Easing.OutQuint);
        text.ScaleTo(1.1f, 3000, Easing.OutQuint);
    }
}
