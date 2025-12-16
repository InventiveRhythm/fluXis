using System;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Storyboards;
using fluXis.Storyboards.Drawables;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Timing;
using SixLabors.ImageSharp;

namespace fluXis.Screens;

/// <summary>
/// a very stupid way to render storyboards, if anyone actually wants to properly implement it, go ahead
/// </summary>
public partial class StoryboardRenderer : FluXisScreen
{
    public override float ParallaxStrength => 0;
    public override bool ShowToolbar => false;
    public override float BackgroundDim => 0;
    public override float BackgroundBlur => 0;
    public override bool AllowMusicControl => false;
    public override bool AllowMusicPausing => false;
    public override bool ShowCursor => false;
    public override bool AutoPlayNext => false;

    [Resolved]
    private GameHost host { get; set; }

    private RealmMap map { get; }
    private Track track;
    private double end => track.Length;

    private double start;
    private ManualFramedClock clock;

    public StoryboardRenderer(RealmMap map)
    {
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load(GlobalClock gc)
    {
        gc.Stop();

        start = Clock.CurrentTime;

        track = map.GetTrack();
        track.Start();
        track.Volume.Value = 0f;

        var info = map.GetMapInfo()!;
        var sb = info.CreateDrawableStoryboard()!;
        LoadComponent(sb);

        clock = new ManualFramedClock();

        var layers = Enum.GetValues<StoryboardLayer>();

        foreach (var layer in layers)
        {
            var wrapper = new DrawableStoryboardLayer(clock, sb, layer);
            AddInternal(wrapper);
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        SchedulerAfterChildren.AddDelayed(updateFrame, 2000);
    }

    private long count = 1;

    private void updateFrame()
    {
        var current = Clock.CurrentTime;
        var elapsed = current - start;
        var progress = clock.CurrentTime / end;

        var estimatedTotal = elapsed * (1 / progress);

        var remaining = estimatedTotal - elapsed;

        var estimatedFormatted = TimeSpan.FromMilliseconds(estimatedTotal).ToString(@"hh\:mm\:ss");
        var elapsedFormatted = TimeSpan.FromMilliseconds(elapsed).ToString(@"hh\:mm\:ss");
        var remainingFormatted = TimeSpan.FromMilliseconds(remaining).ToString(@"hh\:mm\:ss");

        Logger.Log($"[{count}] {TimeUtils.Format(clock.CurrentTime)}/{TimeUtils.Format(end)} ({elapsedFormatted}) - {progress:P2} - Estimated Total: {estimatedFormatted} - Remaining: {remainingFormatted}", LoggingTarget.Runtime,
            LogLevel.Debug);

        var image = host.TakeScreenshotAsync().Result;
        var path = host.Storage.GetFullPath($"render/frame_{count++}.png", true);
        image.SaveAsPng(path);

        if (clock.CurrentTime >= end)
        {
            this.Exit();
            return;
        }

        const double ms_per_frame = 1000 / 60d;
        clock.CurrentTime += ms_per_frame;

        ScheduleAfterChildren(updateFrame);
    }
}
