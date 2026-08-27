using fluXis.Map;
using fluXis.Mods;
using fluXis.Screens.Gameplay.Ruleset;

namespace fluXis.Modes.Keys;

public class KeysGameMode : GameMode
{
    public override PlayableGameMode CreatePlayable(RulesetContainer ruleset, MapInfo map, MapEvents events, IMod[] mods) => new KeysPlayableGameMode(ruleset, map, events, mods);
}
