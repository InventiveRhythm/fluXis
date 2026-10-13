using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Modes;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Verify;

public interface IVerifyContext
{
    GameModeManager Modes { get; }
    EditorConfiguration Configuration { get; }

    PlayableMap Map { get; }
    RealmMap RealmMap { get; }

    RealmMapSet MapSet => RealmMap.MapSet;

    void LoadComponent(Drawable drawable);
}
