using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Online.API.Models.Multi;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.HUD.Leaderboard;
using JetBrains.Annotations;
using osu.Framework.Extensions.IEnumerableExtensions;

namespace fluXis.Screens.Multiplayer.Gameplay.HUD.Leaderboard;

public partial class MultiGameplayLeaderboard : GameplayLeaderboard
{
    [CanBeNull]
    private readonly MultiplayerRoom room;

    public Func<bool> SkipVisiblePredicate = () => false;

    public MultiGameplayLeaderboard([NotNull] [ItemNotNull] List<ScoreInfo> scores, MultiplayerRoom room)
        : base(scores)
    {
        this.room = room;
    }

    protected override LeaderboardEntry CreateEntry(ScoreInfo score) => new MultiLeaderboardEntry(this, score, room) { SkipVisiblePredicate = SkipVisiblePredicate };

    public void RequestingSkip(long[] ids)
    {
        Children.Cast<MultiLeaderboardEntry>().ForEach(l =>
        {
            if (ids.Contains(l.CurrentParticipant.ID))
                l.RequestingSkip.Value = true;
        });
    }
}
