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

    protected override void PopIn() => this.ScaleTo(1, 800, Easing.OutElastic).FadeIn(200);
    protected override void PopOut() => this.ScaleTo(0.7f, 300, Easing.OutQuint).FadeOut(200);

    protected override bool OnHover(HoverEvent e) => HandleHover && base.OnHover(e);
}
