using System.Collections.Generic;
using fluXis.Game.Input;
using osu.Framework.Graphics;

namespace fluXis.Game.Replays;

public partial class ReplayRecorder : Component
{
    private List<FluXisGameplayKeybind> currentPressed { get; } = new();

    public Replay Replay { get; } = new();

    public void PressKey(FluXisGameplayKeybind keybind)
    {
        currentPressed.Add(keybind);
        captureFrame();
    }

    public void ReleaseKey(FluXisGameplayKeybind keybind)
    {
        currentPressed.Remove(keybind);
        captureFrame();
    }

    private void captureFrame()
    {
        Replay.Frames.Add(new ReplayFrame((float)Time.Current, currentPressed.ToArray()));
    }
}
