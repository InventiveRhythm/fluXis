using System.Linq;
using fluXis.Mods;
using fluXis.Utils.Extensions;

namespace fluXis.Modes.Keys;

public partial class KeysPlayer : GameModePlayer
{
    public KeysPlayer(int index)
        : base(index)
    {
    }

    protected override void BeforeLoad()
    {
        AddInternal(Dependencies.CacheAsAndReturn(new LaneSwitchManager(
            Ruleset.MapEvents.LaneSwitchEvents,
            Ruleset.MapInfo.RealmEntry!.KeyCount,
            Ruleset.MapInfo.NewLaneSwitchLayout,
            Ruleset.Mods.Any(x => x is MirrorMod)
        )));
    }

    protected override Playfield CreatePlayfield(int player, int subIndex) => new KeysPlayfield(player, subIndex);
}
