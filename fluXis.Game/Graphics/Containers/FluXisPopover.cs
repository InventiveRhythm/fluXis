using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics.Containers;

public partial class FluXisPopover : Popover
{
    public bool HandleHover { get; init; } = true;
    public int ContentPadding { get; init; } = 10;
    public float BodyRadius { get; init; } = 20;

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Padding = new MarginPadding(ContentPadding);
        Background.Colour = FluXisColors.Background2;

        Body.Masking = true;
        Body.CornerRadius = BodyRadius;
        Body.Margin = new MarginPadding(10);
        Body.EdgeEffect = FluXisStyles.ShadowSmall;
    }

    protected override Drawable CreateArrow() => Empty();

    protected override void PopIn() => this.ScaleTo(.75f).ScaleTo(1, 800, Easing.OutElasticHalf).FadeInFromZero(400, Easing.OutQuint);
    protected override void PopOut() => this.ScaleTo(.9f, 400, Easing.OutQuint).FadeOut(400, Easing.OutQuint);

    protected override bool OnHover(HoverEvent e) => HandleHover && base.OnHover(e);
}
