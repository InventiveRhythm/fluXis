using System;
using System.Linq;
using fluXis.Database;
using fluXis.Database.Input;
using fluXis.Input;
using fluXis.Screens.Edit.Input;
using osu.Framework.Input.Bindings;

namespace fluXis.Utils;

public static class InputUtils
{
    public static KeyBinding GetBindingFor(FluXisGlobalKeybind bind, FluXisRealm realm) => GetBindingFor(bind.ToString(), realm);
    public static KeyBinding GetBindingFor(FluXisGameplayKeybind bind, FluXisRealm realm) => GetBindingFor(bind.ToString(), realm);

    public static KeyBinding GetBindingFor(string bind, FluXisRealm realm)
    {
        var keybind = realm.Run(r => r.All<RealmKeybind>().FirstOrDefault(k => k.Action == bind)?.Detach());
        if (keybind == null) return null;

        return keybind.Key.Contains(',')
            ? new KeyBinding(keybind.Key.Split(',').Select(Enum.Parse<InputKey>).ToArray(), bind)
            : new KeyBinding(Enum.Parse<InputKey>(keybind.Key), bind);
    }

    // using a string as an arg is painful for this, It's best to just use the types
    public static KeyBinding GetDefaultBindingFor<T>(T bind) where T : Enum
    {
        return bind switch
        {
            FluXisGlobalKeybind globalBind => GlobalKeybindContainer.GlobalKeyBindings
                .Concat(GlobalKeybindContainer.InGameKeyBindings)
                .FirstOrDefault(kb => kb.Action.Equals(globalBind)),

            EditorKeybinding editorBind => EditorKeybindingContainer.EditorKeyBindings
                .FirstOrDefault(kb => kb.Action.Equals(editorBind)),

            FluXisGameplayKeybind gameplayBind => new KeyBinding(
                GameplayKeybindContainer.GetDefaultFor(gameplayBind), 
                gameplayBind),

            _ => throw new ArgumentException($"keybinding type not found for: {typeof(T).Name}", nameof(bind))
        };
    }
}