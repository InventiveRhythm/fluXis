using fluXis.Input;
using fluXis.Screens.Gameplay.Input;
using osu.Framework.Bindables;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Gameplay.Replays;

public partial class ReplayInput : GameplayInput
{
    public ReplayInput(Bindable<bool> paused, int mode = 4, bool dual = false)
        : base(paused, mode, dual)
    {
    }

    public override bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e) => true;
    public override void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e) { }
}
