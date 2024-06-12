using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Online;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Multiplayer.Gameplay.Results;

public partial class MultiResultsScores : FluXisScrollContainer
{
    private List<ScoreInfo> scores { get; }

    public MultiResultsScores(List<ScoreInfo> scores)
    {
        this.scores = scores.OrderByDescending(score => score.Score).ToList();
    }

    [BackgroundDependencyLoader]
    private void load(UserCache users)
    {
        RelativeSizeAxes = Axes.Both;
        ScrollbarVisible = false;

        Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Padding = new MarginPadding(40),
            Spacing = new Vector2(0, 10),
            ChildrenEnumerable = scores.Select(score => new MultiResultsScore(score, users.Get(score.PlayerID), scores.IndexOf(score) + 1))
        };
    }
}
