using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Input;
using osu.Framework.Input.Bindings;

namespace fluXis.Game.Input;

public class KeybindStore
{
    private readonly FluXisRealm realm;

    public KeybindStore(FluXisRealm realm)
    {
        this.realm = realm;
    }

    public void AssignDefaults(KeyBindingContainer container)
    {
        realm.RunWrite(r =>
        {
            var existingBindings = r.All<RealmKeybind>().ToList();

            foreach (var binding in container.DefaultKeyBindings)
            {
                if (existingBindings.Any(b => b.Action == binding.Action.ToString()))
                    continue;

                var defaultBinding = container.DefaultKeyBindings.Where(k => k.Action == binding.Action && k.KeyCombination.ToString() == binding.KeyCombination.ToString()).Select(k => new RealmKeybind()
                {
                    Action = k.Action.ToString(),
                    Key = k.KeyCombination.ToString()
                });

                r.Add(defaultBinding);
            }
        });
    }
}
