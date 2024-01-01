using System.Collections.Generic;
using fluXis.Game.Input;

namespace fluXis.Game.Replays;

public class ReplayFrame
{
    public float Time { get; set; }

    public List<FluXisGameplayKeybind> Actions { get; init; }

    public ReplayFrame(float time, params FluXisGameplayKeybind[] actions)
    {
        Time = time;
        Actions = new List<FluXisGameplayKeybind>(actions);
    }
}
