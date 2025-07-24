using System;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;

namespace fluXis.Graphics.Containers;

public partial class FluXisPopover : Popover
{
    public Action OnClose { get; set; }
    private bool initial = true;

    public bool HandleHover { get; init; } = true;
    public int ContentPadding { get; init; } = 12;
    public float BodyRadius { get; init; } = 16;

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Padding = new MarginPadding(ContentPadding);
        Background.Colour = Theme.Background2;

        Body.Masking = true;
        Body.CornerRadius = BodyRadius;
        Body.Margin = new MarginPadding(10);
        Body.EdgeEffect = Styling.ShadowSmall;
    }

    protected override Drawable CreateArrow() => Empty();

    protected override void PopIn()
    {
        this.ScaleTo(.75f).ScaleTo(1, 800, Easing.OutElasticHalf)
            .FadeInFromZero(400, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.ScaleTo(.9f, 400, Easing.OutQuint).FadeOut(400, Easing.OutQuint);

        if (!initial) OnClose?.Invoke();
        initial = false;
    }

    protected override bool OnHover(HoverEvent e) => HandleHover && base.OnHover(e);
}
