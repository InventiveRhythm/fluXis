using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Mods;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.Multiplayer;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Shared.Scoring;
using fluXis.Shared.Scoring.Structs;
using osu.Framework.Allocation;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Multiplayer.Gameplay;

public partial class MultiGameplayScreen : GameplayScreen
{
    protected override bool InstantlyExitOnPause => true;
    protected override bool AllowRestart => false;
    public override bool SubmitScore => false;

    private Playfield field => PlayfieldManager.Playfields[0];

    [Resolved]
    private PanelContainer panels { get; set; }

    private MultiplayerClient client { get; }

    public MultiGameplayScreen(MultiplayerClient client, RealmMap realmMap, List<IMod> mods)
        : base(realmMap, mods)
    {
        this.client = client;
    }

    protected override UserActivity GetPlayingActivity()
    {
        var activity = base.GetPlayingActivity();

        if (activity is UserActivity.Playing playing)
            playing.Room = client.Room;

        return activity;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        field.HealthProcessor.CanFail = false;
        field.JudgementProcessor.ResultAdded += sendScore;

        client.OnScore += onScoreUpdate;
        client.OnResultsReady += onOnResultsReady;
        client.OnDisconnect += onDisconnect;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        field.JudgementProcessor.ResultAdded -= sendScore;

        client.OnScore -= onScoreUpdate;
        client.OnResultsReady -= onOnResultsReady;
        client.OnDisconnect -= onDisconnect;
    }

    protected override void End()
    {
        client.Finish(field.ScoreProcessor.ToScoreInfo());
    }

    private void sendScore(HitResult _) => client.UpdateScore(field.ScoreProcessor.Score);

    private void onScoreUpdate(long user, int score)
    {
        var si = client.Room?.Scores?.FirstOrDefault(s => s.PlayerID == user);

        if (si is null)
            return;

        si.Score = score;
    }

    private void onOnResultsReady(List<ScoreInfo> scores)
    {
        if (this.IsCurrentScreen())
            this.Push(new MultiResults(RealmMap, scores));
    }

    private void onDisconnect()
    {
        CursorVisible = true;
        GameplayClock.Stop();
        panels.Content = new DisconnectedPanel(this.Exit);
    }
}
