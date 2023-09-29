using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Input;
using osu.Framework.Input.Bindings;

namespace fluXis.Game.Input;

public abstract partial class RealmKeyBindingContainer<T> : KeyBindingContainer<T>
    where T : struct
{
    private readonly FluXisRealm realm;

    public RealmKeyBindingContainer(FluXisRealm realm, SimultaneousBindingMode simultaneousMode = SimultaneousBindingMode.None,
                                    KeyCombinationMatchingMode matchingMode = KeyCombinationMatchingMode.Any)
        : base(simultaneousMode, matchingMode)
    {
        this.realm = realm;
    }

    protected override void ReloadMappings()
    {
        IEnumerable<IKeyBinding> bindings = realm.Run(r => r.All<RealmKeybind>()).Select(b => convertKeybind(b)).AsEnumerable();

        if (bindings.Count() == 0)
        {
            KeyBindings = DefaultKeyBindings;
        } else {
            KeyBindings = bindings;
        }
    }

    private IKeyBinding convertKeybind(RealmKeybind b)
    {
        KeyBinding bind;

        if (b.Key.Contains(","))
        {
            bind = new KeyBinding(b.Key.Split(',').Select(k => Enum.Parse<InputKey>(k)).ToArray(), b.Action);
        }
        else
        {
            bind = new KeyBinding(Enum.Parse<InputKey>(b.Key), b.Action);
        }

        return bind;
    }
}
