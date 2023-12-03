using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Intro;

public partial class IntroCorner : Container
{
    private Corner corner { get; }

    private Circle vertical;
    private Circle horizontal;

    public IntroCorner(Corner corner)
    {
        this.corner = corner;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(300);
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Alpha = 0;

        Rotation = corner switch
        {
            Corner.TopLeft => 0,
            Corner.TopRight => 90,
            Corner.BottomRight => 180,
            Corner.BottomLeft => 270,
            _ => Rotation
        };

        InternalChildren = new Drawable[]
        {
            vertical = new Circle { Size = new Vector2(15) },
            horizontal = new Circle { Size = new Vector2(15) }
        };
    }

    public void Show(int delay, int duration)
    {
        this.Delay(delay).FadeIn();
        vertical.Delay(delay).ResizeHeightTo(60, duration * 2, Easing.OutElastic);
        horizontal.Delay(delay).ResizeWidthTo(60, duration * 2, Easing.OutElastic);
    }

    public void Hide(int duration)
    {
        this.ResizeTo(new Vector2(800), duration * 2, Easing.OutQuint).FadeOut(duration);
    }
}
