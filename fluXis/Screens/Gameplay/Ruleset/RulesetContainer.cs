using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio.Transforms;
using fluXis.Map;
using fluXis.Map.Structures.Bases;
using fluXis.Mods;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Gameplay.Input;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
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
    public Bindable<float> ScrollSpeed { get; set; } = new(3);

    public GameplayInput Input { get; }
    public PlayfieldManager PlayfieldManager { get; }

    public HitWindows HitWindows { get; private set; }
    public ReleaseWindows ReleaseWindows { get; private set; }
    public LandmineWindows LandmineWindows { get; private set; }

    private readonly Dictionary<string, ScrollGroup> scrolls = new();
    public IReadOnlyDictionary<string, ScrollGroup> ScrollGroups => scrolls;

    public event Action OnDeath;

    public bool AllowReverting { get; set; }
    public bool AlwaysShowKeys { get; set; }
    public BindableBool IsPaused { get; } = new();

    public bool CatchingUp { get; protected set; }
    public TransformableClock ParentClock { get; set; }
    public Drawable ShakeTarget { get; set; }
    public DebugText DebugText { get; }

    private DependencyContainer dependencies;

    protected override bool ForceChildUpdate => true;

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
        createScrollGroups();

        InternalChildrenEnumerable = new Drawable[]
        {
            dependencies.CacheAsAndReturn(Input),
            PlayfieldManager,
            DebugText
        }.Concat(scrolls.Values.ToArray());
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
        LandmineWindows = new LandmineWindows(difficulty, Rate);
    }

    private void createScrollGroups()
    {
        // creating groups
        for (int i = 0; i < MapInfo.RealmEntry!.KeyCount; i++)
            scrolls[$"${i + 1}"] = new ScrollGroup { Name = $"${i + 1}" };

        var events = MapInfo.ScrollVelocities.Cast<IHasGroups>().Concat(MapEvents.ScrollMultiplyEvents).ToList();
        var groups = events.SelectMany(x => x.Groups).Distinct().Order().ToList();

        foreach (var group in groups)
        {
            if (group.StartsWith('$'))
                continue;

            if (!scrolls.ContainsKey(group))
                scrolls[group] = new ScrollGroup { Name = group };
        }

        scrolls.ForEach(x => LoadComponent(x.Value));

        // populating groups
        foreach (var ev in events)
        {
            if (ev.Groups.Count == 0)
            {
                foreach (var (_, group) in scrolls.Where(x => x.Key.StartsWith('$')))
                    ev.Apply(group);
            }
            else
            {
                foreach (var group in ev.Groups)
                    ev.Apply(scrolls[group]);
            }
        }

        scrolls.ForEach(x => x.Value.InitMarkers());
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
