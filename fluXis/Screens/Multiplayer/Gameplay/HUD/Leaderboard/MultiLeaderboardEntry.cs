using fluXis.Online.API.Models.Multi;
using fluXis.Online.Multiplayer;
using fluXis.Scoring;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Audio;
using fluXis.Screens.Gameplay.HUD.Leaderboard;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;

namespace fluXis.Screens.Multiplayer.Gameplay.HUD.Leaderboard;

public partial class MultiLeaderboardEntry : LeaderboardEntry
{
    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private GameplayClock clock { get; set; }

    [Resolved]
    [CanBeNull]
    private MultiplayerClient client { get; set; }

    public MultiplayerParticipant CurrentParticipant;

    public BindableBool RequestingSkip = new();

    private double lastUpdateTime = 0d;
    private const double update_interval = 100d; // very small optimization

    public MultiLeaderboardEntry(GameplayLeaderboard leaderboard, ScoreInfo score)
        : base(leaderboard, score)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        CurrentParticipant = client?.Room?.Participants.Find(p => p.ID == Player.ID)
                             ?? MultiplayerParticipant.CreateDummy(MultiplayerUserState.Playing);
    }

    protected override void Update()
    {
        base.Update();

        if (Clock.CurrentTime - lastUpdateTime < update_interval) return;

        bool skipVisible = screen.Map.StartTime - clock.CurrentTime > 2000;

        PrimaryText.Text = (RequestingSkip.Value && skipVisible) ? $"{Player.Username} (Skipping)" : $"{Player.Username}";

        lastUpdateTime = Clock.CurrentTime;
    }
}
