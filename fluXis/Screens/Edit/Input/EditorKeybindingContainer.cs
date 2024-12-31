using System.Collections.Generic;
using fluXis.Database;
using fluXis.Input.Bindings;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Input;

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
        new(new KeyCombination(InputKey.Tab), EditorKeybinding.ToggleSidebar),
        new(new KeyCombination(InputKey.Shift, InputKey.Left), EditorKeybinding.PreviousNote),
        new(new KeyCombination(InputKey.Shift, InputKey.Right), EditorKeybinding.NextNote),
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
    ToggleSidebar,
    PreviousNote,
    NextNote,
}
