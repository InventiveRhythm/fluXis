using System;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Format;
using fluXis.Map.Structures.Bases;
using fluXis.Modes.Gameplay;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Edit;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using fluXis.Screens.Edit.Tabs.Verify;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Timing;
using rhym;

namespace fluXis.Modes;

#nullable enable

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract class GameMode : IFromAssembly
{
    public abstract ResourceLocation Location { get; }

    public IResourceStore<byte[]> Resources { get; private set; } = null!;
    public TextureStore Textures { get; private set; } = null!;
    public ISampleStore Samples { get; private set; } = null!;
    public ITrackStore Tracks { get; private set; } = null!;

    internal void SetupResources(Game game, GameHost host)
    {
        Resources = new NamespacedResourceStore<byte[]>(new DllResourceStore(GetType().Assembly), "Resources");
        Textures = new TextureStore(
            host.Renderer,
            host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, "Textures")),
            filteringMode: TextureFilteringMode.Linear
        );
        Samples = game.Audio.GetSampleStore(new NamespacedResourceStore<byte[]>(Resources, "Samples"));
        Tracks = game.Audio.GetTrackStore(new NamespacedResourceStore<byte[]>(Resources, "Tracks"));
    }

    public abstract void RegisterObjects(RhymIO io);
    public virtual IMapFormat? GetFormat(Storage storage, string ext) => null;

    #region Icons and Labels

    public abstract Texture? GetTexture(ResourceLocation mode);
    public abstract string GetShortString(ResourceLocation mode);
    public abstract Colour4 GetColor(ResourceLocation mode);

    #endregion

    #region Editor

    public virtual EditorConfiguration? CreateEditorConfiguration(PlayableMap map) => null;
    public abstract EditorDrawableObject? CreateEditorObject(ITimedObject obj);
    public abstract ChartingTool[] GetEditorTools();
    public virtual IVerifyCheck[] CreateEditorChecks() => [];

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
