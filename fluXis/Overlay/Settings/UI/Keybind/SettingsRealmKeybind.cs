using System;
using System.Linq;
using fluXis.Database;
using fluXis.Database.Input;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Input.Bindings;

namespace fluXis.Overlay.Settings.UI.Keybind;

public partial class SettingsRealmKeybind<T> : SettingsAbstractKeybind<T>
    where T : Enum
{
    [Resolved]
    private FluXisRealm realm { get; set; }

    protected override KeyBinding GetComboFor(T bind) => InputUtils.GetBindingFor(bind.ToString(), realm);

    protected override void UpdateBinding(T keybind, KeyCombination combo) => realm.RunWrite(r =>
    {
        var bind = r.All<RealmKeybind>().FirstOrDefault(x => x.Action == keybind.ToString());

        if (bind == null)
        {
            bind = new RealmKeybind
            {
                Action = keybind.ToString(),
                Key = combo.ToString()
            };

            r.Add(bind);
        }
        else bind.Key = combo.ToString();
    });
}
