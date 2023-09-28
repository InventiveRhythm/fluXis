using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Timing;

namespace fluXis.Game.Map;

public partial class FramedMapClock : Component, IFrameBasedClock, IAdjustableClock, ISourceChangeableClock
{
    public Track Track { get; private set; } = new TrackVirtual(10000);

    public IClock Source => decouple.Source;
    public double CurrentTime => decouple.CurrentTime;
    public bool IsRunning => decouple.IsRunning;
    public double ElapsedFrameTime => decouple.ElapsedFrameTime;
    public double FramesPerSecond => 0;
    public FrameTimeInfo TimeInfo => new() { Elapsed = ElapsedFrameTime, Current = CurrentTime };

    public double Rate { get => decouple.Rate; set => decouple.Rate = value; }
    public bool IsCoupled { get => decouple.IsCoupled; set => decouple.IsCoupled = value; }

    private readonly DecoupleableInterpolatingFramedClock decouple;

    public FramedMapClock()
    {
        decouple = new DecoupleableInterpolatingFramedClock { IsCoupled = true };
    }

    protected override void Update()
    {
        if (Source != null && Source is not IAdjustableClock && Source.CurrentTime < decouple.CurrentTime)
        {
            Seek(Source.CurrentTime);
            return;
        }

        decouple.ProcessFrame();
    }

    public void ChangeSource(IClock source)
    {
        Track = source as Track ?? new TrackVirtual(10000);
        decouple.ChangeSource(source);
    }

    public void Reset()
    {
        decouple.Reset();
        decouple.ProcessFrame();
    }

    public void Start()
    {
        decouple.Start();
        decouple.ProcessFrame();
    }

    public void Stop()
    {
        decouple.Stop();
        decouple.ProcessFrame();
    }

    public bool Seek(double position)
    {
        bool success = decouple.Seek(position);
        decouple.ProcessFrame();
        return success;
    }

    public void ResetSpeedAdjustments() => decouple.ResetSpeedAdjustments();

    public void ProcessFrame() { }
}
