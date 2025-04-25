using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Network.Tabs.Shared;

public partial class DashboardItemList<T> : FillFlowContainer
{
    private string title { get; }
    private IList<T> items { get; }
    private Func<T, Drawable> create { get; }

    public DashboardItemList(string title, IList<T> items, Func<T, Drawable> create)
    {
        this.create = create;
        this.title = title;
        this.items = items;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(items.Any() ? 12 : 4);

        InternalChildren = new Drawable[]
        {
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                Height = 20,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(12),
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = title,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 20
                    },
                    new FluXisSpriteText
                    {
                        Text = $"{items.Count}",
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 14,
                        Alpha = .6f
                    }
                }
            },
            new FluXisSpriteText
            {
                Text = "Nobody here...",
                WebFontSize = 14,
                Alpha = items.Any() ? 0f : .8f
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Full,
                Spacing = new Vector2(8),
                Alpha = items.Any() ? 1f : 0f,
                ChildrenEnumerable = items.Select(create)
            }
        };
    }
}
