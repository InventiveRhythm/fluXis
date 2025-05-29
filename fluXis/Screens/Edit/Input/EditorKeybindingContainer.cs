using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Screens.Edit.Input;

public partial class EditorKeybindingContainer : KeyBindingContainer<EditorKeybinding>, IKeyBindingHandler<EditorKeybinding>
{
    protected override bool Prioritised => true;

    public EditorKeymap Keymap { get; private set; }

    private readonly IKeyBindingHandler<EditorKeybinding> parent;
    private Bindable<string> current { get; }
    private Storage storage { get; }

    public EditorKeybindingContainer(IKeyBindingHandler<EditorKeybinding> parent, Bindable<string> current, GameHost host)
        : base(SimultaneousBindingMode.None, KeyCombinationMatchingMode.Modifiers)
    {
        this.parent = parent;
        this.current = current;

        storage = host.Storage.GetStorageForDirectory("editor/keybindings");

        loadKeymap();
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
        new(new KeyCombination(InputKey.Control, InputKey.D), EditorKeybinding.CloneSelection),
        new(new KeyCombination(InputKey.BackSpace), EditorKeybinding.DeleteSelection),
        new(new KeyCombination(InputKey.Tab), EditorKeybinding.ToggleSidebar),
        new(new KeyCombination(InputKey.Control, InputKey.T), EditorKeybinding.AddTiming),
        new(new KeyCombination(InputKey.Control, InputKey.B), EditorKeybinding.AddNote),
        new(new KeyCombination(InputKey.Shift, InputKey.Left), EditorKeybinding.PreviousNote),
        new(new KeyCombination(InputKey.Shift, InputKey.Right), EditorKeybinding.NextNote),
    };

    private void loadKeymap()
    {
        Keymap = new EditorKeymap();

        try
        {
            var path = storage.GetFullPath("current.json");
            if (!File.Exists(path)) return;

            var json = File.ReadAllText(path);
            Keymap = json.Deserialize<EditorKeymap>();
        }
        catch (JsonException jex)
        {
            Logger.Log($"Failed to parse keymap: {jex.Message}", LoggingTarget.Runtime, LogLevel.Error);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to load keymap!");
        }
    }

    [CanBeNull]
    public KeyBinding GetBindFor(EditorKeybinding bind)
    {
        var keybind = KeyBindings.FirstOrDefault(x => x.Action.Equals(bind));
        return keybind as KeyBinding;
    }

    public void SaveCurrent()
    {
        var path = storage.GetFullPath("current.json");
        var json = Keymap.Serialize();
        File.WriteAllText(path, json);
    }

    public void UpdateBinding(EditorKeybinding bind, KeyCombination combo)
    {
        Keymap.Bindings[bind] = combo.Keys.ToArray();
        ReloadMappings();
    }

    protected override void ReloadMappings()
    {
        var binds = new List<IKeyBinding>();

        foreach (var kvp in Keymap.Bindings)
            binds.Add(new KeyBinding(new KeyCombination(kvp.Value), kvp.Key));

        foreach (var defaultBind in DefaultKeyBindings)
        {
            if (binds.Any(x => x.Action == defaultBind.Action))
                continue;

            binds.Add(defaultBind);
        }

        KeyBindings = binds;
    }

    public bool TryCreateNewMap(string name, out string error)
    {
        error = "";

        if (Path.GetInvalidFileNameChars().Any(name.Contains))
        {
            error = $"Keymap name contains invalid characters!";
            return false;
        }

        return true;
    }

    public bool OnPressed(KeyBindingPressEvent<EditorKeybinding> e) => parent?.OnPressed(e) ?? false;
    public void OnReleased(KeyBindingReleaseEvent<EditorKeybinding> e) => parent?.OnReleased(e);
}
