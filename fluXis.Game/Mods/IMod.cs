using System.Collections.Generic;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public interface IMod
{
    string Name { get; }
    string Acronym { get; }
    string Description { get; }
    IconUsage Icon { get; }
    ModType Type { get; }

    float ScoreMultiplier { get; }
    bool Rankable { get; }
    IEnumerable<string> IncompatibleMods { get; }
}

public enum ModType
{
    Rate,
    DifficultyDecrease,
    DifficultyIncrease,
    Automation,
    Misc,
    Special
}
