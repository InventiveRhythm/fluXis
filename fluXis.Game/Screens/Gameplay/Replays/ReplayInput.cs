using fluXis.Game.Input;
using fluXis.Game.Screens.Gameplay.Input;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Gameplay.Replays;

public partial class ReplayInput : GameplayInput
{
    public ReplayInput(GameplayScreen screen, int mode = 4)
        : base(screen, mode)
    {
    }

    public override bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e) => true;
    public override void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e) { }
}
