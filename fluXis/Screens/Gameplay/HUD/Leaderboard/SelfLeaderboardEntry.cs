using fluXis.Online.API.Models.Users;
using fluXis.Scoring.Processing;

namespace fluXis.Screens.Gameplay.HUD.Leaderboard;

public partial class SelfLeaderboardEntry : LeaderboardEntry
{
    protected override float TotalScore => processor.Score;
    protected override double PerformanceRating => processor.PerformanceRating.Value;
    protected override APIUser Player => processor.Player;

    private ScoreProcessor processor { get; }

    public SelfLeaderboardEntry(GameplayLeaderboard leaderboard, ScoreProcessor processor)
        : base(leaderboard, null)
    {
        this.processor = processor;
    }
}
