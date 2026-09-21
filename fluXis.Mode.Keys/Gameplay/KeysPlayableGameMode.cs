using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Modes.Gameplay;
using fluXis.Mods;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;

namespace fluXis.Mode.Keys.Gameplay;

public partial class KeysPlayableGameMode : PlayableGameMode
{
    public int KeyCount { get; }
    public override int DefaultGroupCount => KeyCount;

    public override GameModePlayer[] Players { get; }

    private HitWindows? hitWindows;
    private HitWindows? releaseWindows;
    private HitWindows? landmineWindows;

    public KeysPlayableGameMode(RulesetContainer ruleset, PlayableMap map, IMod[] mods)
        : base(ruleset, map, mods)
    {
        KeyCount = KeysGameMode.ParseMode(map.Mode);
        var count = map.IsDual ? 2 : 1;

        Players = new GameModePlayer[count];
        for (var i = 0; i < Players.Length; i++) Players[i] = new KeysPlayer(i);
    }

    protected override KeyBindingContainer CreateBindContainer() => new KeysKeybindContainer(KeyCount, Map.IsDual);

    protected override GridContainer CreatePlayerGrid(IEnumerable<Drawable> drawable) => new()
    {
        RelativeSizeAxes = Axes.Both,
        Content = new[] { drawable.ToArray() }
    };

    public override HitWindows CreateHitWindowFor(HitObject? obj)
    {
        var difficulty = Math.Clamp(Map.AccuracyDifficulty == 0 ? 8 : Map.AccuracyDifficulty, 1, 10);
        difficulty *= Mods.Any(m => m is HardMod) ? 1.5f : 1;

        hitWindows ??= new HitWindows(difficulty, Ruleset.Rate);
        releaseWindows ??= new ReleaseWindows(difficulty, Ruleset.Rate);
        landmineWindows ??= new LandmineWindows(difficulty, Ruleset.Rate);

        return hitWindows;
    }
}
