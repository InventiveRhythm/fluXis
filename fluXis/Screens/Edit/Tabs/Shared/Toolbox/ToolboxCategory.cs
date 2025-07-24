using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Shared.Toolbox;

public partial class ToolboxCategory : Container
{
    public string Title { get; init; }
    public string ExtraTitle { get; init; }
    public IconUsage Icon { get; init; }
    public IReadOnlyList<ChartingTool> Tools { get; init; }

    private FillFlowContainer title;
    private FluXisSpriteIcon icon;
    private FluXisSpriteText titleText;
    private FluXisSpriteText keybindText;

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
                Colour = Theme.Background3
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
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 30,
                        Child = title = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 30,
                            Direction = FillDirection.Horizontal,
                            Padding = new MarginPadding(6),
                            Spacing = new Vector2(6),
                            Children = new Drawable[]
                            {
                                icon = new FluXisSpriteIcon
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Icon = Icon,
                                    Size = new Vector2(16)
                                },
                                titleText = new FluXisSpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Text = Title,
                                    WebFontSize = 14
                                },
                                keybindText = new FluXisSpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Text = ExtraTitle,
                                    Alpha = .6f,
                                    WebFontSize = 12
                                }
                            }
                        }
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(0, 5),
                        Children = GetItems()
                    }
                }
            }
        };
    }

    protected virtual List<ToolboxButton> GetItems() => Tools.Select(t => new ToolboxButton { Tool = t }).ToList();

    public void OnSizeChanged(bool open)
    {
        title.MoveToX(open ? 0 : 10, 400, Easing.OutQuint);
        icon.FadeTo(open ? 1 : .8f, 200);
        titleText.FadeTo(open ? 1 : 0, 200);
        keybindText.FadeTo(open ? .6f : 0, 200);
    }
}
