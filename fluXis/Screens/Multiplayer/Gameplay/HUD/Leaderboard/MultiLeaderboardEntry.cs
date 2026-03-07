using System;
using fluXis.Online.API.Models.Multi;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.HUD.Leaderboard;
using JetBrains.Annotations;
using osu.Framework.Bindables;

namespace fluXis.Screens.Multiplayer.Gameplay.HUD.Leaderboard;

public partial class MultiLeaderboardEntry : LeaderboardEntry
{
    [CanBeNull]
    private readonly MultiplayerRoom room;

    public MultiplayerParticipant CurrentParticipant;

    public Func<bool> SkipVisiblePredicate = () => false;
    public BindableBool RequestingSkip = new();

    public MultiLeaderboardEntry(GameplayLeaderboard leaderboard, ScoreInfo score, MultiplayerRoom room)
        : base(leaderboard, score)
    {
        this.room = room;
    }

    protected override void AfterInitialLoad()
    {
        CurrentParticipant = room?.Participants.Find(p => p.ID == Player.ID)
                             ?? MultiplayerParticipant.CreateDummy(MultiplayerUserState.Playing);
    }

    protected override void UpdateValues()
    {
        base.UpdateValues();

        PrimaryText.Text = (RequestingSkip.Value && SkipVisiblePredicate()) ? $"{Player.Username} (Skipping)" : $"{Player.Username}";
    }
}
