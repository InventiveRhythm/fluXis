using fluXis.Online.API.Models.Multi;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring.Processing;
using fluXis.Screens.Gameplay.HUD.Leaderboard;

namespace fluXis.Screens.Multiplayer.Gameplay.HUD.Leaderboard;

public partial class MultiSelfLeaderboardEntry : MultiLeaderboardEntry
{
    protected override float TotalScore => processor.Score;
    protected override double PerformanceRating => processor.PerformanceRating.Value;
    protected override APIUser Player => processor.Player;

    private ScoreProcessor processor { get; }

    public MultiSelfLeaderboardEntry(GameplayLeaderboard leaderboard, ScoreProcessor processor, MultiplayerRoom room)
        : base(leaderboard, null, room)
    {
        this.processor = processor;
    }
}
