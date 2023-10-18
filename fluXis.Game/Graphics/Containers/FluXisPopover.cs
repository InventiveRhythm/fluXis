using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Graphics.Containers;

public partial class FluXisPopover : Popover
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Padding = new MarginPadding(10);
        Background.Colour = FluXisColors.Background2;

        Body.Masking = true;
        Body.CornerRadius = 20;
        Body.Margin = new MarginPadding(10);
        Body.EdgeEffect = FluXisStyles.ShadowSmall;
    }

    protected override Drawable CreateArrow() => Empty();

    protected override void PopIn() => this.ScaleTo(1, 800, Easing.OutElastic).FadeIn(200);
    protected override void PopOut() => this.ScaleTo(0.7f, 300, Easing.OutQuint).FadeOut(200);
}
