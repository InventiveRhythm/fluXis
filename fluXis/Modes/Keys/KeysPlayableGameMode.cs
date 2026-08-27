using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Map.Structures;
using fluXis.Mods;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Modes.Keys;

#nullable enable

public partial class KeysPlayableGameMode : PlayableGameMode
{
    public override GameModePlayer[] Players { get; }

    private HitWindows? hitWindows;
    private HitWindows? releaseWindows;
    private HitWindows? landmineWindows;

    public KeysPlayableGameMode(RulesetContainer ruleset, MapInfo map, MapEvents events, IMod[] mods)
        : base(ruleset, map, events, mods)
    {
        var count = map.IsDual ? 2 : 1;

        Players = new GameModePlayer[count];
        for (var i = 0; i < Players.Length; i++) Players[i] = new KeysPlayer(i);
    }

    protected override GridContainer CreatePlayerGrid(IEnumerable<Drawable> drawable) => new()
    {
        RelativeSizeAxes = Axes.Both,
        Content = new[] { drawable.ToArray() }
    };

    protected override HitWindows CreateHitWindowFor(HitObject? obj)
    {
        var difficulty = Math.Clamp(Map.AccuracyDifficulty == 0 ? 8 : Map.AccuracyDifficulty, 1, 10);
        difficulty *= Mods.Any(m => m is HardMod) ? 1.5f : 1;

        hitWindows ??= new HitWindows(difficulty, Ruleset.Rate);
        releaseWindows ??= new ReleaseWindows(difficulty, Ruleset.Rate);
        landmineWindows ??= new LandmineWindows(difficulty, Ruleset.Rate);

        return hitWindows;
    }
}
