using System.Linq;
using fluXis.Map.Structures.Events;
using fluXis.Modes.Gameplay;
using fluXis.Mods;
using fluXis.Utils.Extensions;

namespace fluXis.Modes.Keys.Gameplay;

public partial class KeysPlayer : GameModePlayer
{
    public KeysPlayer(int index)
        : base(index)
    {
    }

    protected override void BeforeLoad()
    {
        AddInternal(Dependencies.CacheAsAndReturn(new LaneSwitchManager(
            Ruleset.Map.ObjectsOfType<LaneSwitchEvent>(),
            (Ruleset.PlayableMode as KeysPlayableGameMode)!.KeyCount,
            !Ruleset.Map.LegacyLaneSwitchLayout,
            Ruleset.Mods.Any(x => x is MirrorMod)
        )));
    }

    protected override Playfield CreatePlayfield(int player, int subIndex) => new KeysPlayfield(player, subIndex);
}
