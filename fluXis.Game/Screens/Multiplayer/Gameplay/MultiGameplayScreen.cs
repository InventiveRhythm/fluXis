using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Mods;
using fluXis.Game.Screens.Gameplay;

namespace fluXis.Game.Screens.Multiplayer.Gameplay;

public partial class MultiGameplayScreen : GameplayScreen
{
    protected override bool InstantlyExitOnPause => true;
    public override bool SubmitScore => false;

    public MultiGameplayScreen(RealmMap realmMap, List<IMod> mods)
        : base(realmMap, mods)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        HealthProcessor.CanFail = false;
    }

    protected override void End()
    {
        // send complete packet
    }
}
