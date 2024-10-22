using fluXis.Game.Scoring.Processing;
using fluXis.Game.Scoring.Processing.Health;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class GameplayHUDComponent : Container
{
    [Resolved]
    protected GameplayScreen Screen { get; private set; }

    public JudgementProcessor JudgementProcessor { get; set; }
    public HealthProcessor HealthProcessor { get; set; }
    public ScoreProcessor ScoreProcessor { get; set; }

    public HUDComponentSettings Settings { get; set; } = new();
}
