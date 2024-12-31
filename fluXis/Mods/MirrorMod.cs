using System;
using System.Linq;
using fluXis.Graphics.Sprites;
using fluXis.Map;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Mods;

public class MirrorMod : IMod, IApplicableToMap
{
    public string Name => "Mirror";
    public string Acronym => "MR";
    public string Description => "Flips the notes horizontally.";
    public IconUsage Icon => FontAwesome6.Solid.LeftRight;
    public ModType Type => ModType.Misc;
    public float ScoreMultiplier => 1f;
    public bool Rankable => true;
    public Type[] IncompatibleMods => Array.Empty<Type>();

    public void Apply(MapInfo map)
    {
        var keycount = map.Map?.KeyCount ?? map.HitObjects.MaxBy(x => x.Lane).Lane;

        foreach (var hit in map.HitObjects)
            hit.Lane = keycount - hit.Lane + 1;
    }
}
