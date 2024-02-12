using System;
using fluXis.Game.Screens.Select.Info.Header;
using fluXis.Game.Screens.Select.Info.Scores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Select.Info;

public partial class SelectMapInfo : GridContainer
{
    public ScoreList ScoreList { get; set; }

    public Action HoverAction { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreRight;
        Origin = Anchor.CentreRight;
        RelativeSizeAxes = Axes.Both;
        Width = .5f;
        Padding = new MarginPadding { Vertical = 10 };
        Margin = new MarginPadding { Right = -20 };
        RowDimensions = new Dimension[]
        {
            new(GridSizeMode.AutoSize),
            new(GridSizeMode.Absolute, 10),
            new()
        };

        Content = new[]
        {
            new Drawable[] { new SelectMapInfoHeader() },
            Array.Empty<Drawable>(), // Spacer
            new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Bottom = 20 },
                    Child = ScoreList = new ScoreList()
                }
            }
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        HoverAction?.Invoke();
        return true;
    }
}
