using System.Collections.Generic;
using fluXis.Screens.Gameplay.Ruleset;

namespace fluXis.Map.Structures.Bases;

public interface IHasGroups
{
    List<string> Groups { get; set; }

    void Apply(ScrollGroup group);
}
