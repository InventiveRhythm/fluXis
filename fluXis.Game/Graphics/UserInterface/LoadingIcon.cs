using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface;

public partial class LoadingIcon : Container
{
    public bool ShowBackground { get; set; } = false;

    private CircularContainer container;

    public LoadingIcon()
    {
        Size = new Vector2(100);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;
        CornerRadius = 10;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black.Opacity(0.5f),
                Alpha = ShowBackground ? 1 : 0
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(.8f, .16f),
                Child = container = new CircularContainer
                {
                    RelativePositionAxes = Axes.Both,
                    RelativeSizeAxes = Axes.Both,
                    Width = 0.2f,
                    Masking = true,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.White
                    },
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Colour = Colour4.Black.Opacity(0.25f),
                        Radius = 5,
                        Offset = new Vector2(0, 1)
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        const float duration = 600;
        const Easing easing = Easing.InOutExpo;

        container.ResizeWidthTo(1f, duration, easing)
                 .Then()
                 .MoveToX(0.8f, duration, easing)
                 .ResizeWidthTo(.2f, duration, easing)
                 .Then()
                 .ResizeWidthTo(1f, duration, easing)
                 .MoveToX(0, duration, easing)
                 .Then()
                 .ResizeWidthTo(.2f, duration, easing)
                 .Loop();
    }
}
