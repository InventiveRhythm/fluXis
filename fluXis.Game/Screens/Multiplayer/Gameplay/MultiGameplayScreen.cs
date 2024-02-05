using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Mods;
using fluXis.Game.Online.Multiplayer;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Result;
using osu.Framework.Logging;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Multiplayer.Gameplay;

public partial class MultiGameplayScreen : GameplayScreen
{
    protected override bool InstantlyExitOnPause => true;
    public override bool SubmitScore => false;

    private MultiplayerClient client { get; }

    public MultiGameplayScreen(MultiplayerClient client, RealmMap realmMap, List<IMod> mods)
        : base(realmMap, mods)
    {
        this.client = client;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        HealthProcessor.CanFail = false;

        client.ResultsReady += onResultsReady;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        client.ResultsReady -= onResultsReady;
    }

    protected override void End()
    {
        client.Finished(ScoreProcessor.ToScoreInfo(client.Player));
    }

    private void onResultsReady(List<ScoreInfo> scores)
    {
        Logger.Log($"Received {scores.Count} scores for results");
        var score = scores.Find(s => s.PlayerID == client.Player.ID);

        if (score == null)
        {
            // uhh
            // should never happen
            // but just in case?
            this.Exit();
            return;
        }

        this.Push(new SoloResults(RealmMap, score, client.Player));
    }
}
