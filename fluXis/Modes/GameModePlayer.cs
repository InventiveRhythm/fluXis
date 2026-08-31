using System.Linq;
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

    public Playfield MainPlayfield { get; private set; } = null!;
    public Playfield[] SubPlayfields { get; private set; } = null!;

    public bool IsFinished => MainPlayfield.IsFinished && SubPlayfields.All(p => p.IsFinished);

    protected GameModePlayer(int index)
    {
        PlayerIndex = index;
    }

    /// <summary>
    /// Gets called before the rest of load() executes. Should be used to register needed dependencies for playfields.
    /// </summary>
    protected virtual void BeforeLoad() { }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Dependencies.CacheAs(this);
        Dependencies.Cache(this);

        BeforeLoad();

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

        MainPlayfield = CreatePlayfield(PlayerIndex, 0);
        SubPlayfields = Enumerable.Range(1, Ruleset.MapInfo.ExtraPlayfields).Select(x => CreatePlayfield(PlayerIndex, x)).ToArray();

        var content = new SortingContainer { RelativeSizeAxes = Axes.Both };
        content.Child = MainPlayfield;
        content.AddRange(SubPlayfields);
        AddInternal(content);
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

    protected abstract Playfield CreatePlayfield(int player, int subIndex);

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => Dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    protected override void Dispose(bool isDisposing)
    {
        ScoreProcessor.Dispose();
        base.Dispose(isDisposing);
    }

    private partial class SortingContainer : Container<Playfield>
    {
        protected override int Compare(Drawable x, Drawable y)
        {
            var a = (Playfield)x;
            var b = (Playfield)y;

            var result = -a.AnimationZ.CompareTo(b.AnimationZ);

            if (result != 0)
                return result;

            return -a.PlayfieldIndex.CompareTo(b.PlayfieldIndex);
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            SortInternal();
        }
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
