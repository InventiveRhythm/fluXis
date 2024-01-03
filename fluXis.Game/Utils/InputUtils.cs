using System;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Input;
using fluXis.Game.Input;
using osu.Framework.Input.Bindings;

namespace fluXis.Game.Utils;

public static class InputUtils
{
    public static KeyBinding GetBindingFor(FluXisGlobalKeybind bind, FluXisRealm realm) => GetBindingFor(bind.ToString(), realm);
    public static KeyBinding GetBindingFor(FluXisGameplayKeybind bind, FluXisRealm realm) => GetBindingFor(bind.ToString(), realm);

    public static KeyBinding GetBindingFor(string bind, FluXisRealm realm)
    {
        var keybind = realm.Run(r => r.All<RealmKeybind>().FirstOrDefault(k => k.Action == bind).Detach());
        if (keybind == null) return null;

        return keybind.Key.Contains(',')
            ? new KeyBinding(keybind.Key.Split(',').Select(Enum.Parse<InputKey>).ToArray(), bind)
            : new KeyBinding(Enum.Parse<InputKey>(keybind.Key), bind);
    }
}
