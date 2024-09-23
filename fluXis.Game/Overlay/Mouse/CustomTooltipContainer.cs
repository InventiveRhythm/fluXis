using fluXis.Game.Graphics;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Overlay.Mouse;

public abstract partial class CustomTooltipContainer<T> : CustomTooltipContainer, ITooltip<T>
{
    public sealed override void SetContent(object content) => SetContent((T)content);
    public abstract void SetContent(T content);
}

public abstract partial class CustomTooltipContainer : VisibilityContainer, ITooltip
{
    protected override Container<Drawable> Content { get; } = new Container { AutoSizeAxes = Axes.Both };
    private bool instant = true;

    private const float starting_scale = .9f;

    protected CustomTooltipContainer()
    {
        AutoSizeAxes = Axes.Both;
        CornerRadius = 8;
        Masking = true;
        AutoSizeEasing = Easing.OutQuint;
        EdgeEffect = FluXisStyles.ShadowSmall;
        Scale = new Vector2(starting_scale);

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            Content
        };
    }

    protected override void PopIn()
    {
        instant |= !IsPresent;
        this.FadeIn(400, Easing.OutQuint)
            .ScaleTo(1, 800, Easing.OutElasticHalf);
    }

    protected override void PopOut()
    {
        this.Delay(200).FadeOut(400, Easing.OutQuint)
            .ScaleTo(starting_scale, 600, Easing.OutQuint);
    }

    public abstract void SetContent(object content);

    public void Move(Vector2 pos)
    {
        if (instant)
        {
            Position = pos;
            instant = false;
            return;
        }

        this.MoveTo(pos, 200, Easing.OutQuint);
    }
}
