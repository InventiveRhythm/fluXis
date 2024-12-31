using fluXis.Input;
using fluXis.Screens.Gameplay.Input;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Gameplay.Replays;

public partial class ReplayInput : GameplayInput
{
    public ReplayInput(GameplayScreen screen, int mode, bool dual)
        : base(screen, mode, dual)
    {
    }

    public override bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e) => true;
    public override void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e) { }
}
