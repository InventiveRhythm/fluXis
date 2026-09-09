using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Replays;

public partial class ReplayRecorder : Component
{
    private List<int> currentPressed { get; } = new();

    public Replay Replay { get; } = new();
    public BindableBool IsRecording { get; } = new(true);

    public event Action<ReplayFrame> OnFrameCreated;

    public void PressKey(int keybind)
    {
        currentPressed.Add(keybind);
        captureFrame();
    }

    public void ReleaseKey(int keybind)
    {
        currentPressed.Remove(keybind);
        captureFrame();
    }

    private void captureFrame()
    {
        if (!IsRecording.Value) return;

        var frame = new ReplayFrame(Time.Current, currentPressed.Cast<int>().ToArray());
        Replay.Frames.Add(frame);
        OnFrameCreated?.Invoke(frame);
    }
}
