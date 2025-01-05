using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Input;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Screens.Gameplay.Input;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Timing;

namespace fluXis.Screens.Gameplay.Replays;

public partial class ReplayRulesetContainer : RulesetContainer, IFrameBasedClock, IAdjustableClock
{
    public Replay Replay { get; }

    private List<ReplayFrame> frames { get; }
    private Stack<ReplayFrame> handledFrames { get; }
    private List<FluXisGameplayKeybind> currentPressed = new();

    public double CurrentTime { get; private set; }
    public double ElapsedFrameTime { get; private set; }
    public double FramesPerSecond => ParentClock.FramesPerSecond;
    public bool IsRunning => ParentClock.IsRunning;

    double IClock.Rate => ParentClock.Rate;

    double IAdjustableClock.Rate
    {
        get => ParentClock.Rate;
        set => ParentClock.Rate = value;
    }

    public ReplayRulesetContainer(Replay replay, MapInfo map, MapEvents events, List<IMod> mods)
        : base(map, events, mods)
    {
        Replay = replay;
        AllowReverting = true;

        frames = replay.Frames;
        handledFrames = new Stack<ReplayFrame>();

        Clock = this;
        CurrentTime = -4000;
    }

    protected override GameplayInput CreateInput() => new ReplayInput(IsPaused.GetBoundCopy(), MapInfo.RealmEntry!.KeyCount, MapInfo.IsDual);

    public override bool UpdateSubTree()
    {
        var target = ParentClock.CurrentTime;
        ElapsedFrameTime = target - CurrentTime;
        CatchingUp = Math.Abs(ElapsedFrameTime) > 20;

        if (target > CurrentTime)
        {
            if (frames.Count != 0)
            {
                var first = frames[0];

                if (target > first.Time)
                    target = first.Time;
            }
        }
        else if (target < CurrentTime)
        {
            if (handledFrames.Count != 0)
            {
                var frame = handledFrames.Peek();

                if (target < frame.Time)
                    target = frame.Time - 1;
            }
        }

        CurrentTime = target;
        return base.UpdateSubTree();
    }

    protected override void Update()
    {
        base.Update();

        if (frames.Count >= 0)
        {
            while (frames.Count > 0 && frames[0].Time <= Clock.CurrentTime)
            {
                var frame = frames[0];
                frames.RemoveAt(0);
                handledFrames.Push(frame);
                handlePresses(frame.Actions);
            }

            while (handledFrames.Count > 0)
            {
                var result = handledFrames.Peek();

                if (Clock.CurrentTime >= result.Time)
                    break;

                revertFrame(handledFrames.Pop());
            }
        }
    }

    private void revertFrame(ReplayFrame frame)
    {
        foreach (var keybind in currentPressed)
            Input.ReleaseKey(keybind);

        currentPressed.Clear();
        frames.Insert(0, frame);
    }

    private void handlePresses(List<int> frameActionsInt)
    {
        var frameActions = frameActionsInt.Select(i => (FluXisGameplayKeybind)i).ToList();

        foreach (var keybind in frameActions)
        {
            if (currentPressed.Contains(keybind))
                continue;

            Input.PressKey(keybind);
        }

        foreach (var keybind in currentPressed)
        {
            if (frameActions.Contains(keybind))
                continue;

            Input.ReleaseKey(keybind);
        }

        currentPressed = frameActions;
    }

    public void Reset() => ParentClock.Reset();
    public void Start() => ParentClock.Start();
    public void Stop() => ParentClock.Stop();
    public bool Seek(double position) => ParentClock.Seek(position);
    public void ResetSpeedAdjustments() => ParentClock.ResetSpeedAdjustments();

    public void ProcessFrame() { }
}
