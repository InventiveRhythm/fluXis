using System.Collections.Generic;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Network.Tabs.Account;

public partial class DashboardAccountCategory : FillFlowContainer
{
    public new IReadOnlyList<Drawable> Children { set => AddRange(value); }

    public Drawable LoadingIcon { get; init; }
    public Drawable CompletedIcon { get; init; }

    public DashboardAccountCategory(string title)
    {
        Width = 500;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(10);

        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = title,
                    FontSize = 24
                },
                new Container
                {
                    Size = new Vector2(16),
                    Margin = new MarginPadding(4),
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Children = new[]
                    {
                        LoadingIcon = new SpriteIcon
                        {
                            RelativeSizeAxes = Axes.Both,
                            Icon = FontAwesome6.Solid.Rotate,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Alpha = 0
                        },
                        CompletedIcon = new SpriteIcon
                        {
                            RelativeSizeAxes = Axes.Both,
                            Icon = FontAwesome6.Solid.Check,
                            Alpha = 0
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        LoadingIcon.Spin(1000, RotationDirection.Clockwise);
    }
}
