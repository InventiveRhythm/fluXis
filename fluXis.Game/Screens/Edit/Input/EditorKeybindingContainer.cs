using System.Collections.Generic;
using fluXis.Game.Database;
using fluXis.Game.Input.Bindings;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Input;

public partial class EditorKeybindingContainer : RealmKeyBindingContainer<EditorKeybinding>, IKeyBindingHandler<EditorKeybinding>
{
    private readonly IKeyBindingHandler<EditorKeybinding> parent;

    protected override bool Prioritised => true;

    public EditorKeybindingContainer(IKeyBindingHandler<EditorKeybinding> parent, FluXisRealm realm)
        : base(realm, SimultaneousBindingMode.None, KeyCombinationMatchingMode.Modifiers)
    {
        this.parent = parent;
    }

    public override IEnumerable<IKeyBinding> DefaultKeyBindings => new KeyBinding[]
    {
        new(new KeyCombination(InputKey.F1), EditorKeybinding.OpenHelp),
        new(new KeyCombination(InputKey.Control, InputKey.S), EditorKeybinding.Save),
        new(new KeyCombination(InputKey.Control, InputKey.Shift, InputKey.O), EditorKeybinding.OpenFolder),
        new(new KeyCombination(InputKey.Control, InputKey.Z), EditorKeybinding.Undo),
        new(new KeyCombination(InputKey.Control, InputKey.Y), EditorKeybinding.Redo),
        new(new KeyCombination(InputKey.Control, InputKey.F), EditorKeybinding.FlipSelection),
        new(new KeyCombination(InputKey.Control, InputKey.Q), EditorKeybinding.ShuffleSelection),
        new(new KeyCombination(InputKey.Tab), EditorKeybinding.ToggleSidebar)
    };

    public bool OnPressed(KeyBindingPressEvent<EditorKeybinding> e) => parent?.OnPressed(e) ?? false;
    public void OnReleased(KeyBindingReleaseEvent<EditorKeybinding> e) => parent?.OnReleased(e);
}

public enum EditorKeybinding
{
    OpenHelp,
    Save,
    OpenFolder,
    Undo,
    Redo,
    FlipSelection,
    ShuffleSelection,
    ToggleSidebar
}
