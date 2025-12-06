using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Database;
using fluXis.Database.Input;
using osu.Framework.Input.Bindings;
using Realms;

namespace fluXis.Input.Bindings;

public abstract partial class RealmKeyBindingContainer<T> : KeyBindingContainer<T>
    where T : struct
{
    protected virtual List<KeyBinding> GamepadBinds { get; } = new();

    private readonly FluXisRealm realm;

    protected RealmKeyBindingContainer(FluXisRealm realm, SimultaneousBindingMode simultaneousMode = SimultaneousBindingMode.None,
                                       KeyCombinationMatchingMode matchingMode = KeyCombinationMatchingMode.Any)
        : base(simultaneousMode, matchingMode)
    {
        this.realm = realm;
    }

    protected override void LoadComplete()
    {
        realm.Run(r =>
        {
            r.All<RealmKeybind>().SubscribeForNotifications((sender, changes) =>
            {
                if (changes == null)
                    return;

                ReloadMappings(sender.AsQueryable());
            });
        });

        base.LoadComplete();
    }

    protected override void ReloadMappings() => ReloadMappings(realm.Run(r => r.All<RealmKeybind>()));

    protected void ReloadMappings(IQueryable<RealmKeybind> keybindings)
    {
        var realmBindings = keybindings.AsEnumerable();
        var bindings = convertKeybindings(realmBindings).Where(k => DefaultKeyBindings.Any(d => d.Action.Equals(k.Action))).ToList();
        KeyBindings = (bindings.Count != 0 ? bindings : DefaultKeyBindings).Concat(GamepadBinds);
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
