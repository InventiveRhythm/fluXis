using System;
using System.Linq;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Bindables;
using osu.Framework.Timing;

namespace fluXis.Modes;

#nullable enable

public abstract class GameMode
{
    public abstract PlayableGameMode CreatePlayable(RulesetContainer ruleset, MapInfo map, MapEvents events, IMod[] mods);

    public virtual HealthProcessor CreateHealthProcessor(MapInfo map, IMod[] mods, IFrameBasedClock clock, Bindable<bool> inBreak, Action? onDeath = null)
    {
        HealthProcessor? processor = null;

        var difficulty = Math.Clamp(map.HealthDifficulty == 0 ? 8 : map.HealthDifficulty, 1, 10);
        difficulty *= mods.Any(m => m is HardMod) ? 1.2f : 1f;

        if (mods.Any(m => m is HardMod)) processor = new DrainHealthProcessor(difficulty);
        else if (mods.Any(m => m is EasyMod)) processor = new RequirementHeathProcessor(difficulty) { HealthRequirement = EasyMod.HEALTH_REQUIREMENT };

        processor ??= new HealthProcessor(difficulty);
        processor.Clock = clock;
        processor.InBreak = inBreak;
        processor.OnFail = onDeath;

        foreach (var mod in mods.OfType<IApplicableToHealthProcessor>())
            mod.Apply(processor);

        return processor;
    }
}
