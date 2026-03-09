using System.Collections.Generic;
using System.Linq;
using fluXis.Scoring;
using fluXis.Scoring.Processing;
using fluXis.Screens.Gameplay.HUD.Leaderboard;
using JetBrains.Annotations;
using osu.Framework.Extensions.IEnumerableExtensions;

namespace fluXis.Screens.Multiplayer.Gameplay.HUD.Leaderboard;

public partial class MultiGameplayLeaderboard : GameplayLeaderboard
{
    public MultiGameplayLeaderboard([NotNull] [ItemNotNull] List<ScoreInfo> scores)
        : base(scores)
    {
    }

    protected override LeaderboardEntry CreateEntry(ScoreInfo score) => new MultiLeaderboardEntry(this, score);
    protected override LeaderboardEntry CreateSelfEntry(ScoreProcessor processor) => new MultiSelfLeaderboardEntry(this, processor);

    public void RequestingSkip(long[] ids)
    {
        Children.OfType<MultiLeaderboardEntry>().ForEach(l =>
            l.RequestingSkip.Value = ids.Contains(l.CurrentParticipant.ID)
        );
    }
}
