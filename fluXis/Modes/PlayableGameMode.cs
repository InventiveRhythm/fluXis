using System.Collections.Generic;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Mods;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Modes;

#nullable enable

public abstract partial class PlayableGameMode : CompositeDrawable
{
    protected RulesetContainer Ruleset { get; }
    protected MapInfo Map { get; }
    protected MapEvents Events { get; }
    protected IMod[] Mods { get; }

    public HitWindows HitWindows { get; private set; } = null!;

    public BindableBool InBreak { get; } = new();

    public abstract GameModePlayer[] Players { get; }
    public GameModePlayer FirstPlayer => Players[0];

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
        InternalChild = CreatePlayerGrid(Players);

        HitWindows = CreateHitWindowFor(null);
    }

    protected abstract GridContainer CreatePlayerGrid(IEnumerable<Drawable> drawable);
    protected abstract HitWindows CreateHitWindowFor(HitObject? obj);
}
