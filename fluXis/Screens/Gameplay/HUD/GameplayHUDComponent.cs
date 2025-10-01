using fluXis.Scoring;
using fluXis.Scoring.Processing;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.HUD;

public partial class GameplayHUDComponent : Container
{
    protected IHUDDependencyProvider Deps { get; private set; }
    public HUDComponentSettings Settings { get; private set; }

    protected RulesetContainer Ruleset => Deps.Ruleset;
    protected JudgementProcessor JudgementProcessor => Deps.JudgementProcessor;
    protected HealthProcessor HealthProcessor => Deps.HealthProcessor;
    protected ScoreProcessor ScoreProcessor => Deps.ScoreProcessor;
    protected HitWindows HitWindows => Deps.HitWindows;

    public void Populate(HUDComponentSettings settings, IHUDDependencyProvider deps)
    {
        Settings = settings;
        Deps = deps;
    }
}
