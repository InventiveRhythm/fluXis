using System.Linq;
using fluXis.Game.Mods.Drawables;
using fluXis.Game.Scoring;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Result.Center;

public partial class ResultsCenter : CompositeDrawable
{
    public Vector2 ScreenSpaceRankPosition => rankCenter.ScreenSpaceDrawQuad.Centre;

    private Container rankCenter;

    private ResultsCenterScore score;
    private ModList mods;
    private Circle line;
    private ResultsCenterStats stats;

    [BackgroundDependencyLoader]
    private void load(ScoreInfo scoreInfo)
    {
        Width = 480;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            RowDimensions = new Dimension[]
            {
                new(),
                new(GridSizeMode.AutoSize),
                new(GridSizeMode.AutoSize),
                new(GridSizeMode.AutoSize),
                new(GridSizeMode.AutoSize)
            },
            Content = new[]
            {
                new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = rankCenter = new Container
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    }
                },
                new Drawable[] { score = new ResultsCenterScore() },
                new Drawable[]
                {
                    mods = new ModList
                    {
                        AutoSizeAxes = Axes.X,
                        Height = 40,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        Alpha = scoreInfo.Mods.Count != 0 ? 1 : 0,
                        Mods = scoreInfo.Mods.Select(ModUtils.GetFromAcronym).ToList(),
                        Margin = new MarginPadding { Top = 36 }
                    }
                },
                new Drawable[]
                {
                    line = new Circle
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 4,
                        Alpha = .4f,
                        Margin = new MarginPadding { Vertical = 36 }
                    }
                },
                new Drawable[] { stats = new ResultsCenterStats() }
            }
        };
    }

    public override void Show()
    {
        score.MoveToY(50).MoveToY(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        mods.MoveToY(50).MoveToY(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        line.MoveToY(50).MoveToY(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        stats.MoveToY(50).MoveToY(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
    }

    public override void Hide()
    {
        score.MoveToY(50, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        mods.MoveToY(50, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        line.MoveToY(50, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        stats.MoveToY(50, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
    }
}
