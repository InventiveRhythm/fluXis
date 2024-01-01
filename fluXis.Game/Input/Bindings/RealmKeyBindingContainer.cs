using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Input;
using osu.Framework.Input.Bindings;
using Realms;

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

    protected override void LoadComplete()
    {
        realm.Run(r =>
        {
            r.All<RealmKeybind>().SubscribeForNotifications((sender, changes, error) =>
            {
                if (changes == null)
                {
                    return;
                }

                ReloadMappings(sender.AsQueryable());
            });
        });

        base.LoadComplete();
    }

    protected override void ReloadMappings() => ReloadMappings(realm.Run(r => r.All<RealmKeybind>()));

    protected void ReloadMappings(IQueryable<RealmKeybind> keybindings)
    {
        var realmBindings = keybindings.AsEnumerable();
        var bindings = convertKeybindings(realmBindings).Where(k => DefaultKeyBindings.Any(d => d.Action.Equals(k.Action)));

        KeyBindings = bindings.Any()
            ? bindings
            : DefaultKeyBindings;
    }

    private IEnumerable<IKeyBinding> convertKeybindings(IEnumerable<RealmKeybind> bindings)
    {
        foreach (var binding in bindings)
        {
            if (Enum.TryParse(binding.Action, out T action))
            {
                yield return binding.Key.Contains(',')
                    ? new KeyBinding(binding.Key.Split(',').Select(Enum.Parse<InputKey>).ToArray(), action)
                    : new KeyBinding(Enum.Parse<InputKey>(binding.Key), action);
            }
        }
    }
}
