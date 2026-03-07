using System.Collections.Generic;
using fluXis.Online.API.Models.Multi;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Gameplay.HUD.Leaderboard;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Screens.Multiplayer.Gameplay.HUD.Leaderboard;
using JetBrains.Annotations;

namespace fluXis.Screens.Multiplayer.Gameplay.HUD;

public partial class MultiGameplayHUD : GameplayHUD
{
    [CanBeNull]
    private readonly MultiplayerRoom room;

    public new MultiGameplayLeaderboard Leaderboard { get; protected set; }

    public MultiGameplayHUD(RulesetContainer ruleset, HUDLayout layout = null, MultiplayerRoom room = null!)
        : base(ruleset, layout)
    {
        this.room = room;
    }

    protected override GameplayLeaderboard CreateLeaderboard() => Leaderboard = new MultiGameplayLeaderboard(Screen?.Scores ?? new List<ScoreInfo>(), room);
}
