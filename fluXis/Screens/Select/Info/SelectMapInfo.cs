using System;
using fluXis.Screens.Select.Info.Header;
using fluXis.Screens.Select.Info.Tabs.Scores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Select.Info;

public partial class SelectMapInfo : GridContainer
{
    public ScoreListTab ScoreList => tabs.Scores;
    public Action HoverAction { get; init; }

    private SelectInfoTabs tabs;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreRight;
        Origin = Anchor.CentreRight;
        RelativeSizeAxes = Axes.Both;
        Width = .5f;
        Padding = new MarginPadding { Top = 20, Bottom = 10 };
        Margin = new MarginPadding { Right = -20 };
        RowDimensions = new Dimension[]
        {
            new(GridSizeMode.AutoSize),
            new()
        };

        Content = new[]
        {
            new Drawable[] { new SelectMapInfoHeader() },
            new Drawable[] { tabs = new SelectInfoTabs() }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        HoverAction?.Invoke();
        return true;
    }
}
