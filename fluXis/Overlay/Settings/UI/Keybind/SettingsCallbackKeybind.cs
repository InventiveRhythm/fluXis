using System;
using osu.Framework.Input.Bindings;

namespace fluXis.Overlay.Settings.UI.Keybind;

public partial class SettingsCallbackKeybind<T> : SettingsAbstractKeybind<T>
    where T : Enum
{
    private Func<T, KeyBinding> find { get; }

    public Action<T, KeyCombination> BindUpdated { get; set; }

    public SettingsCallbackKeybind(Func<T, KeyBinding> find)
    {
        this.find = find;
    }

    protected override KeyBinding GetComboFor(T bind) => find.Invoke(bind);
    protected override void UpdateBinding(T bind, KeyCombination combo) => BindUpdated?.Invoke(bind, combo);
}
