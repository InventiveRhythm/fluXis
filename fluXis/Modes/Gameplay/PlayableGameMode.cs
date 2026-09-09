using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Mods;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;

namespace fluXis.Modes.Gameplay;

#nullable enable

public abstract partial class PlayableGameMode : CompositeDrawable
{
    protected RulesetContainer Ruleset { get; }
    protected MapInfo Map { get; }
    protected MapEvents Events { get; }
    protected IMod[] Mods { get; }

    public HitWindows HitWindows { get; private set; } = null!;
    public KeyBindingContainer Keybinds { get; private set; } = null!;

    public BindableBool InBreak { get; } = new();
    public bool Finished { get; private set; }
    public event Action? OnFinish;

    public abstract GameModePlayer[] Players { get; }
    public GameModePlayer FirstPlayer => Players[0];

    public bool AnyFailed => Players.Any(p => p.HealthProcessor.Failed);

    protected PlayableGameMode(RulesetContainer ruleset, MapInfo map, MapEvents events, IMod[] mods)
    {
        Ruleset = ruleset;
        Map = map;
        Events = events;
        Mods = mods;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        InternalChild = Keybinds = CreateBindContainer()
                                   .WithRelativeSize(Axes.Both)
                                   .WithChild(CreatePlayerGrid(Players));

        Dependencies.CacheAs(Keybinds);
        Dependencies.Cache(Keybinds);

        HitWindows = CreateHitWindowFor(null);
    }

    protected override void Update()
    {
        base.Update();

        Players.ForEach(p => p.HealthProcessor.Update(Time.Elapsed));

        if (!Finished && Players.All(p => p.IsFinished))
        {
            OnFinish?.Invoke();
            Finished = true;
        }
    }

    public bool OnComplete() => Players.All(p => p.HealthProcessor.OnComplete());

    #region Required Overrides

    protected abstract KeyBindingContainer CreateBindContainer();
    protected abstract GridContainer CreatePlayerGrid(IEnumerable<Drawable> drawable);
    public abstract HitWindows CreateHitWindowFor(HitObject? obj);

    #endregion

    #region Dependencies

    protected new DependencyContainer Dependencies { get; private set; } = null!;

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => Dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    #endregion
}
