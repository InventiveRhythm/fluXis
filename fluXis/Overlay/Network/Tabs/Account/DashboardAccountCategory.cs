using System.Collections.Generic;
using fluXis.Graphics.Sprites;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Network.Tabs.Account;

public partial class DashboardAccountCategory : FillFlowContainer
{
    public new IReadOnlyList<Drawable> Children { set => AddRange(value); }

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
                }
            }
        };
    }
}
