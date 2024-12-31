using System.Linq;
using fluXis.Database;
using fluXis.Database.Input;
using osu.Framework.Input.Bindings;

namespace fluXis.Input;

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

                r.Add(new RealmKeybind
                {
                    Action = binding.Action.ToString(),
                    Key = binding.KeyCombination.ToString()
                });
            }
        });
    }
}
