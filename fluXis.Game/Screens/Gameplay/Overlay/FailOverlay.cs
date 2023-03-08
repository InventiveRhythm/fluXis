using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Overlay;

public partial class FailOverlay : Container
{
    private readonly StripePattern pattern;
    private readonly SpriteText text;
    private readonly Box wedgeLeft;
    private readonly Box wedgeRight;

    public FailOverlay()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        Children = new Drawable[]
        {
            wedgeLeft = new Box
            {
                RelativePositionAxes = Axes.Both,
                RelativeSizeAxes = Axes.Both,
                Width = 1.1f,
                X = -1.1f,
                Shear = new Vector2(.1f, 0),
                Colour = Colour4.FromHex("#090912")
            },
            wedgeRight = new Box
            {
                RelativePositionAxes = Axes.Both,
                RelativeSizeAxes = Axes.Both,
                Width = 1.1f,
                X = 1.1f,
                Shear = new Vector2(.1f, 0),
                Colour = Colour4.FromHex("#090912")
            },
            pattern = new StripePattern
            {
                Speed = new Vector2(-300)
            },
            text = new SpriteText
            {
                Text = "FAILED",
                Font = new FontUsage("Quicksand", 100, "Bold"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    public override void Show()
    {
        this.FadeIn(200);
        pattern.SpeedTo(new Vector2(-50), 2000, Easing.OutQuint);
        text.ScaleTo(1.1f, 6000, Easing.OutQuint);

        wedgeLeft.MoveToX(0, 2000, Easing.OutQuint);
        wedgeRight.MoveToX(0, 2000, Easing.OutQuint);
    }
}
