using System;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Format;
using fluXis.Map.Structures.Bases;
using fluXis.Modes.Gameplay;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Platform;
using osu.Framework.Timing;
using rhym;

namespace fluXis.Modes;

#nullable enable

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract class GameMode : IFromAssembly
{
    public abstract ResourceLocation Location { get; }

    public abstract void RegisterObjects(RhymIO io);
    public virtual IMapFormat? GetFormat(Storage storage, string ext) => null;

    #region Editor

    public abstract EditorDrawableObject? CreateEditorObject(ITimedObject obj);
    public abstract ChartingTool[] GetEditorTools();

    #endregion

    public abstract AutoGenerator CreateAutoGenerator(PlayableMap map);

    public abstract PlayableGameMode CreatePlayable(RulesetContainer ruleset, PlayableMap map, IMod[] mods);

    public virtual HealthProcessor CreateHealthProcessor(PlayableMap map, IMod[] mods, IFrameBasedClock clock, Bindable<bool> inBreak, Action? onDeath = null)
    {
        HealthProcessor? processor = null;

        var difficulty = Math.Clamp(map.HealthDifficulty == 0 ? 8 : map.HealthDifficulty, 1, 10);
        difficulty *= mods.Any(m => m is HardMod) ? 1.2f : 1f;

        if (mods.Any(m => m is HardMod)) processor = new DrainHealthProcessor(difficulty);
        else if (mods.Any(m => m is EasyMod))
            processor = new RequirementHeathProcessor(difficulty) { HealthRequirement = EasyMod.HEALTH_REQUIREMENT };

        processor ??= new HealthProcessor(difficulty);
        processor.Clock = clock;
        processor.InBreak = inBreak;
        processor.OnFail = onDeath;

        foreach (var mod in mods.OfType<IApplicableToHealthProcessor>())
            mod.Apply(processor);

        return processor;
    }

    string IFromAssembly.AssemblyName { get; set; } = string.Empty;
    string IFromAssembly.AssemblyHash { get; set; } = string.Empty;
}
