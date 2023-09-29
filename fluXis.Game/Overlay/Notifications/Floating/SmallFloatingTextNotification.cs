using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Notifications.Floating;

public partial class SmallFloatingTextNotification : FloatingNotification
{
    public string Text { get; set; }
    public IconUsage Icon { get; set; } = FontAwesome.Solid.Info;

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
                new SpriteIcon
                {
                    Icon = Icon,
                    Size = new Vector2(12),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                },
                new FluXisSpriteText
                {
                    Text = Text,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        content.MoveToY(-70).MoveToY(0, 400, Easing.OutQuint);
        this.ResizeHeightTo(20, 400, Easing.OutQuint).Delay(5000).FadeOut(400).Expire();
    }
}
