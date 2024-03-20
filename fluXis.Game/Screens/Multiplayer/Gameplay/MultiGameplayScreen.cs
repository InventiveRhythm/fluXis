using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Mods;
using fluXis.Game.Online.Multiplayer;
using fluXis.Game.Screens.Gameplay;
using fluXis.Shared.Scoring;
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
        client.Finish(ScoreProcessor.ToScoreInfo(client.Player));
    }

    private void onResultsReady(List<ScoreInfo> scores) => this.Push(new MultiResults(RealmMap, scores));
}
