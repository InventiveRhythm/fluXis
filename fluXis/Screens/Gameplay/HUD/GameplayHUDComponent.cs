using fluXis.Scoring;
using fluXis.Scoring.Processing;
using fluXis.Scoring.Processing.Health;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.HUD;

public partial class GameplayHUDComponent : Container
{
    [Resolved]
    protected GameplayScreen Screen { get; private set; }

    protected HUDComponentSettings Settings { get; private set; }

    protected JudgementProcessor JudgementProcessor { get; private set; }
    protected HealthProcessor HealthProcessor { get; private set; }
    protected ScoreProcessor ScoreProcessor { get; private set; }

    public HitWindows HitWindows { get; private set; }

    public void Populate(HUDComponentSettings settings, JudgementProcessor judgement, HealthProcessor health, ScoreProcessor score, HitWindows windows)
    {
        Settings = settings;
        JudgementProcessor = judgement;
        HealthProcessor = health;
        ScoreProcessor = score;
        HitWindows = windows;
    }
}
