using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio.Transforms;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Gameplay.Input;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Ruleset;

public partial class RulesetContainer : CompositeDrawable
{
    public MapInfo MapInfo { get; }
    public MapEvents MapEvents { get; }
    public List<IMod> Mods { get; }
    public APIUser CurrentPlayer { get; init; }

    public float Rate { get; }

    public GameplayInput Input { get; }
    public PlayfieldManager PlayfieldManager { get; }

    public HitWindows HitWindows { get; private set; }
    public ReleaseWindows ReleaseWindows { get; private set; }

    public event Action OnDeath;

    public bool AllowReverting { get; set; }
    public BindableBool IsPaused { get; } = new();

    public bool CatchingUp { get; protected set; }
    public TransformableClock ParentClock { get; set; }
    public Drawable ShakeTarget { get; set; }
    public DebugText DebugText { get; }

    private DependencyContainer dependencies;

    public RulesetContainer(MapInfo map, MapEvents events, List<IMod> mods)
    {
        MapInfo = map;
        MapEvents = events;
        Mods = mods;

        Rate = Mods.OfType<RateMod>().FirstOrDefault()?.Rate ?? 1;

        Input = CreateInput();
        PlayfieldManager = new PlayfieldManager(MapInfo);
        DebugText = new DebugText();

        ShakeTarget ??= this;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        dependencies.CacheAs(this);

        createHitWindows();

        InternalChildren = new Drawable[]
        {
            dependencies.CacheAsAndReturn(Input),
            PlayfieldManager,
            DebugText
        };
    }

    protected virtual GameplayInput CreateInput() => new(IsPaused.GetBoundCopy(), MapInfo.RealmEntry!.KeyCount, MapInfo.IsDual);

    public HealthProcessor CreateHealthProcessor()
    {
        var processor = null as HealthProcessor;

        var difficulty = Math.Clamp(MapInfo.HealthDifficulty == 0 ? 8 : MapInfo.HealthDifficulty, 1, 10);
        difficulty *= Mods.Any(m => m is HardMod) ? 1.2f : 1f;

        if (Mods.Any(m => m is HardMod)) processor = new DrainHealthProcessor(difficulty);
        else if (Mods.Any(m => m is EasyMod)) processor = new RequirementHeathProcessor(difficulty) { HealthRequirement = EasyMod.HEALTH_REQUIREMENT };

        processor ??= new HealthProcessor(difficulty);
        processor.Clock = Clock;
        processor.InBreak = PlayfieldManager.InBreak;
        processor.OnFail = () => OnDeath?.Invoke();

        foreach (var mod in Mods.OfType<IApplicableToHealthProcessor>())
            mod.Apply(processor);

        return processor;
    }

    private void createHitWindows()
    {
        var difficulty = Math.Clamp(MapInfo.AccuracyDifficulty == 0 ? 8 : MapInfo.AccuracyDifficulty, 1, 10);
        difficulty *= Mods.Any(m => m is HardMod) ? 1.5f : 1;

        HitWindows = new HitWindows(difficulty, Rate);
        ReleaseWindows = new ReleaseWindows(difficulty, Rate);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
