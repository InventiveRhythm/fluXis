using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio.Transforms;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Mods;
using fluXis.Online.Activity;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.Multiplayer;
using fluXis.Scoring;
using fluXis.Scoring.Structs;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Screens.Multiplayer.Gameplay.HUD;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Screens;

namespace fluXis.Screens.Multiplayer.Gameplay;

public partial class MultiGameplayScreen : GameplayScreen
{
    protected override bool InstantlyExitOnPause => true;
    protected override bool AllowRestart => false;
    protected override bool SubmitScore => false;

    private PlayfieldPlayer player => PlayfieldManager.FirstPlayer;

    private MultiplayerClient client { get; }

    protected new MultiGameplayHUD GameplayHUD;

    private int playingParticipants => client.Room?.Participants.Count(p => p.State == MultiplayerUserState.Playing) ?? 1;
    private int minVoteMajority => (int)Math.Ceiling(playingParticipants * MultiplayerRoom.MIN_VOTE_SKIP_MAJORITY);

    public MultiGameplayScreen(MultiplayerClient client, RealmMap realmMap, List<IMod> mods)
        : base(realmMap, mods)
    {
        this.client = client;
    }

    protected override GameplayHUD CreateGameplayHUD() => GameplayHUD = new MultiGameplayHUD(RulesetContainer, room: client?.Room);

    protected override UserActivity GetPlayingActivity()
    {
        var activity = base.GetPlayingActivity();

        if (activity is UserActivity.Playing playing)
            playing.Room = client.Room;

        return activity;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        GameplayDependencies.CacheAs(client);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        player.HealthProcessor.CanFail = false;
        player.JudgementProcessor.ResultAdded += sendScore;

        client.OnScore += onScoreUpdate;
        client.OnVoteSkipUpdate += onVoteSkipUpdate;
        client.OnResultsReady += onOnResultsReady;
        client.OnDisconnect += onDisconnect;

        if (client.Room != null)
            ScheduleAfterChildren(() => SkipOverlay.SkipText.Text = $"Skip (0/{minVoteMajority})");
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        player.JudgementProcessor.ResultAdded -= sendScore;

        client.OnScore -= onScoreUpdate;
        client.OnVoteSkipUpdate -= onVoteSkipUpdate;
        client.OnResultsReady -= onOnResultsReady;
        client.OnDisconnect -= onDisconnect;
    }

    protected override void End()
    {
        client.Finish(player.ScoreProcessor.ToScoreInfo());
    }

    protected override bool RequestSkip()
    {
        var doSkip = base.RequestSkip();
        client.VoteSkip(!doSkip);

        // to prevent skipping by other means
        return false;
    }

    private void sendScore(HitResult _) => client.UpdateScore(player.ScoreProcessor.Score);

    private void onVoteSkipUpdate(long[] playersVoted, bool canSkip)
    {
        Scheduler.ScheduleIfNeeded(() =>
        {
            if (client.Room != null)
                SkipOverlay.SkipText.Text = $"Skip ({playersVoted.Length}/{minVoteMajority})";

            GameplayHUD.Leaderboard.RequestingSkip(playersVoted);

            if (canSkip) SkipIntro();
        });
    }

    private void onScoreUpdate(long user, int score)
    {
        Scheduler.ScheduleIfNeeded(() =>
        {
            var si = client.Room?.Scores?.FirstOrDefault(s => s.PlayerID == user);

            if (si is null)
                return;

            si.Score = score;
        });
    }

    private void onOnResultsReady(List<ScoreInfo> scores)
    {
        Scheduler.ScheduleOnceIfNeeded(() =>
        {
            if (this.IsCurrentScreen())
                this.Push(new MultiplayerResults(RealmMap, scores, client));
        });
    }

    private void onDisconnect()
    {
        CursorVisible = true;
        GameplayClock.RateTo(0, Styling.TRANSITION_MOVE);
    }
}
