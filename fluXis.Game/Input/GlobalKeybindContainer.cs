using System.Collections.Generic;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;

namespace fluXis.Game.Input
{
    public class GlobalKeybindContainer : KeyBindingContainer<GlobalKeybind>, IHandleGlobalKeyboardInput
    {
        public GlobalKeybindContainer(KeyCombinationMatchingMode keyCombinationMatchingMode = KeyCombinationMatchingMode.Any, SimultaneousBindingMode simultaneousBindingMode = SimultaneousBindingMode.All)
            : base(matchingMode: KeyCombinationMatchingMode.Modifiers)
        {
        }

        public override IEnumerable<IKeyBinding> DefaultKeyBindings => new[]
        {
            new KeyBinding(new[] { InputKey.T }, GlobalKeybind.ToggleToolbar)
        };
    }
}
