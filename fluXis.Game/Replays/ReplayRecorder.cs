using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Input;
using fluXis.Shared.Replays;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Replays;

public partial class ReplayRecorder : Component
{
    private List<FluXisGameplayKeybind> currentPressed { get; } = new();

    public Replay Replay { get; } = new();
    public BindableBool IsRecording { get; } = new(true);

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
        if (!IsRecording.Value) return;

        Replay.Frames.Add(new ReplayFrame((float)Time.Current, currentPressed.Cast<int>().ToArray()));
    }
}
