using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring;
using fluXis.Scoring.Processing;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Modes;

#nullable enable

public abstract partial class GameModePlayer : CompositeDrawable, IHUDDependencyProvider
{
    [Resolved]
    private GameplaySamples? samples { get; set; }

    [Resolved]
    protected RulesetContainer Ruleset { get; private set; } = null!;

    public JudgementProcessor JudgementProcessor { get; protected set; } = null!;
    public HealthProcessor HealthProcessor { get; protected set; } = null!;
    public ScoreProcessor ScoreProcessor { get; protected set; } = null!;

    protected new DependencyContainer Dependencies { get; private set; } = null!;

    public int PlayerIndex { get; }

    protected GameModePlayer(int index)
    {
        PlayerIndex = index;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        JudgementProcessor = new JudgementProcessor();
        JudgementProcessor.AddDependants([
            HealthProcessor = Ruleset.CreateHealthProcessor(),
            ScoreProcessor = ScoreProcessor = new ScoreProcessor(x => Schedule(x), Ruleset.AsyncScoreCalculations)
            {
                Player = Ruleset.CurrentPlayer ?? APIUser.Default,
                HitWindows = Ruleset.HitWindows,
                MapInfo = Ruleset.MapInfo,
                Mods = Ruleset.Mods
            }
        ]);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        JudgementProcessor.ApplyMap(Ruleset.MapInfo);
        HealthProcessor.OnSavedDeath += () => samples?.EarlyFail();
        ScoreProcessor.OnComboBreak += () =>
        {
            if (Ruleset.CatchingUp)
                return;

            samples?.Miss();
        };
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => Dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    protected override void Dispose(bool isDisposing)
    {
        ScoreProcessor.Dispose();
        base.Dispose(isDisposing);
    }

    #region IHUDDependencyProvider

    RulesetContainer IHUDDependencyProvider.Ruleset => Ruleset;
    HitWindows IHUDDependencyProvider.HitWindows => Ruleset.PlayableMode.HitWindows;
    RealmMap IHUDDependencyProvider.RealmMap => Ruleset.MapInfo.RealmEntry;
    MapInfo IHUDDependencyProvider.MapInfo => Ruleset.MapInfo;
    float IHUDDependencyProvider.PlaybackRate => Ruleset.Rate;
    double IHUDDependencyProvider.CurrentTime => Ruleset.Time.Current;

    #endregion
}
