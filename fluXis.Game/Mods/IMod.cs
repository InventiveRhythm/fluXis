using System.Collections.Generic;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Mods;

public interface IMod
{
    string Name { get; }
    string Acronym { get; }
    string Description { get; }
    IconUsage Icon { get; }

    float ScoreMultiplier { get; }
    bool Rankable { get; }
    IEnumerable<string> IncompatibleMods { get; }
}
