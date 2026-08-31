using fluXis.Map;
using fluXis.Mods;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Utils;

namespace fluXis.Modes.Keys;

public class KeysGameMode : GameMode
{
    public override ResourceLocation Location => new("flustix", "keys");
    public override PlayableGameMode CreatePlayable(RulesetContainer ruleset, MapInfo map, MapEvents events, IMod[] mods) => new KeysPlayableGameMode(ruleset, map, events, mods);
}
