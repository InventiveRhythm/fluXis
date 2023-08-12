using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Screens.Edit.Tabs.Charting.Tools;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Toolbox;

public partial class ToolboxCategory : Container
{
    public string Title { get; init; }
    public IconUsage Icon { get; init; }
    public IReadOnlyList<ChartingTool> Tools { get; init; }

    private FillFlowContainer title;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 10;
        Masking = true;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(5),
                Spacing = new Vector2(0, 5),
                AutoSizeDuration = 200,
                AutoSizeEasing = Easing.OutQuint,
                Children = new Drawable[]
                {
                    title = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 30,
                        Direction = FillDirection.Horizontal,
                        Padding = new MarginPadding(5),
                        Spacing = new Vector2(10),
                        Children = new Drawable[]
                        {
                            new SpriteIcon
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Icon = Icon,
                                Size = new Vector2(20)
                            },
                            new FluXisSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Text = Title,
                                FontSize = 24
                            }
                        }
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5),
                        Children = Tools.Select(t => new ToolboxButton { Tool = t }).ToList()
                    }
                }
            }
        };
    }

    public void OnSizeChanged(bool open)
    {
        title.FadeTo(open ? 1 : 0, 500, Easing.OutQuint);
    }
}
