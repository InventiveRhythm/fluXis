using System;
using System.Linq;
using fluXis.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Setup;

public partial class SetupSection : FillFlowContainer
{
    private string title { get; }

    public Drawable[] Entries { get; init; } = Array.Empty<Drawable>();

    public SetupSection(string title)
    {
        this.title = title;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Spacing = new Vector2(15);
        Direction = FillDirection.Vertical;

        InternalChildrenEnumerable = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = title,
                WebFontSize = 24,
                Margin = new MarginPadding { Horizontal = 10, Bottom = -8 }
            }
        }.Concat(Entries);
    }
}
