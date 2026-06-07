using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio.Transforms;
using fluXis.Graphics;
using fluXis.Online.Activity;
using fluXis.Online.Multiplayer;
using fluXis.Scoring;
using fluXis.Scoring.Structs;
using fluXis.Screens.Gameplay.Capabilities.Bases;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Screens.Multiplayer.Gameplay;
using fluXis.Utils.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Screens.Gameplay.Capabilities;

#nullable enable

public partial class MultiplayerCapability : Component, IEndingCapability, IUserActivityCapability
{
    public GameplayScreen Screen { get; set; } = null!;

    private PlayfieldPlayer player => Screen.PlayfieldManager.FirstPlayer;
    private MultiplayerClient client { get; }

    public MultiplayerCapability(MultiplayerClient client)
    {
        this.client = client;
    }

    void IGameplayCapability.PreLoad()
    {
        Screen.AllowPausing = false;
        Screen.AllowRestarting = false;
        Screen.InstantlyExitOnPause = true;
    }

    void IUserActivityCapability.Modify(UserActivity activity)
    {
        if (activity is UserActivity.Playing playing)
            playing.Room = client.Room;
    }

    Screen? IEndingCapability.OnEnd(ScoreInfo score, Action complete)
    {
        client.Finish(score);
        return null;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        player.HealthProcessor.CanFail = false;
        player.JudgementProcessor.ResultAdded += sendScore;

        client.OnScore += onScoreUpdate;
        client.OnResultsReady += onOnResultsReady;
        client.OnDisconnect += onDisconnect;
    }

    public void Exit()
    {
        player.JudgementProcessor.ResultAdded -= sendScore;

        client.OnScore -= onScoreUpdate;
        client.OnResultsReady -= onOnResultsReady;
        client.OnDisconnect -= onDisconnect;
    }

    private void sendScore(HitResult _) => client.UpdateScore(player.ScoreProcessor.Score);

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
            if (Screen.IsCurrentScreen())
                Screen.Push(new MultiplayerResults(Screen.RealmMap, scores, client));
        });
    }

    private void onDisconnect()
    {
        Screen.CursorVisible = true;
        Screen.GameplayClock.RateTo(0, Styling.TRANSITION_MOVE);
    }
}
