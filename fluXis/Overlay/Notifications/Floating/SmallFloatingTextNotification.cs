using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Overlay.Notifications.Floating;

public partial class SmallFloatingTextNotification : FloatingNotification
{
    public string Text { get; init; }
    public IconUsage Icon { get; init; } = FontAwesome6.Solid.Info;

    private FillFlowContainer content;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;

        Child = content = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(8),
            Children = new Drawable[]
            {
                new FluXisSpriteIcon
                {
                    Icon = Icon,
                    Size = new Vector2(12),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Shadow = true
                },
                new FluXisSpriteText
                {
                    Text = Text,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Shadow = true
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        content.MoveToY(-70).MoveToY(0, 600, Easing.OutQuint);
        this.ResizeHeightTo(20, 600, Easing.OutQuint).Delay(5000).FadeOut(300).Expire();
    }
}
