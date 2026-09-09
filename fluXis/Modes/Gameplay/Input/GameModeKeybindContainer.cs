using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Input.States;

namespace fluXis.Modes.Gameplay.Input;

public abstract partial class GameModeKeybindContainer<T> : KeyBindingContainer<T>, IGameModeActions
    where T : struct, Enum
{
    public event Action<int> OnActionPress;
    public event Action<int> OnActionRelease;

    protected override bool HandleRepeats => false;
    public bool HandlePlayerInput { get; set; } = true;

    public abstract override IEnumerable<IKeyBinding> DefaultKeyBindings { get; }

    private readonly Dictionary<int, T> intToActionMap;
    private readonly Dictionary<T, int> actionToIntMap;

    protected GameModeKeybindContainer()
    {
        intToActionMap = Enum.GetValues<T>().ToDictionary(x => Convert.ToInt32(x), x => x);
        actionToIntMap = Enum.GetValues<T>().ToDictionary(x => x, x => Convert.ToInt32(x));
    }

    protected override bool Handle(UIEvent e) => HandlePlayerInput && base.Handle(e);

    protected override Drawable PropagatePressed(IEnumerable<Drawable> drawables, InputState state, T pressed, float scrollAmount = 0, bool isPrecise = false, bool repeat = false)
    {
        OnActionPress?.Invoke(actionToIntMap[pressed]);
        return base.PropagatePressed(drawables, state, pressed, scrollAmount, isPrecise, repeat);
    }

    protected override void PropagateReleased(IEnumerable<Drawable> drawables, InputState state, T released)
    {
        OnActionRelease?.Invoke(actionToIntMap[released]);
        base.PropagateReleased(drawables, state, released);
    }

    public void TriggerPress(int bind) => TriggerPressed(intToActionMap[bind]);
    public void TriggerRelease(int bind) => TriggerReleased(intToActionMap[bind]);
}

public interface IGameModeActions
{
    bool HandlePlayerInput { get; set; }

    event Action<int> OnActionPress;
    event Action<int> OnActionRelease;

    void TriggerPress(int bind);
    void TriggerRelease(int bind);
}
