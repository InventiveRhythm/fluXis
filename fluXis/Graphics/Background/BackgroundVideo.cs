using System;
using fluXis.Map;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Video;
using osu.Framework.Logging;
using osu.Framework.Timing;

namespace fluXis.Graphics.Background;

public partial class BackgroundVideo : CompositeDrawable
{
    public IFrameBasedClock VideoClock { get; init; }

    public Action PlaybackStarted { get; set; }
    public bool IsPlaying { get; private set; }

    private Container videoContainer;
    private Video video;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            videoContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    public void LoadVideo(MapInfo map)
    {
        if (video != null)
        {
            var old = video;

            Scheduler.ScheduleIfNeeded(() =>
            {
                if (old == null) return;

                if (!old.IsLoaded)
                {
                    old.OnLoadComplete += v => v.Expire();
                    return;
                }

                this.FadeOut(400);
                old.Delay(400).Expire();
                old = null;
            });

            video = null;
        }

        if (string.IsNullOrWhiteSpace(map?.VideoFile))
            return;

        try
        {
            var stream = map?.GetVideoStream();

            if (stream == null)
                return;

            var v = video = new Video(stream, false)
            {
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            };

            Scheduler.ScheduleIfNeeded(() => LoadComponentAsync(v, videoContainer.Add));
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load video!");
        }
    }

    protected override void Update()
    {
        base.Update();

        if (video is not { IsLoaded: true }) return;

        var clock = VideoClock ?? Clock;

        if (clock.CurrentTime > 2000 && Alpha == 0 && IsPlaying)
            this.FadeIn(400); // workaround for editor playtesting

        if (clock.CurrentTime > video.Duration)
        {
            this.FadeOut(400);
            return;
        }

        if (IsPlaying)
            video.PlaybackPosition = clock.CurrentTime;
    }

    public void Stop()
    {
        if (video == null) return;

        if (!video.IsLoaded)
        {
            video.OnLoadComplete += _ => Stop();
            return;
        }

        IsPlaying = false;
        this.FadeOut(400);
    }

    public void Start()
    {
        if (video == null)
            return;

        if (!video.IsLoaded)
        {
            video.OnLoadComplete += _ => Start();
            return;
        }

        IsPlaying = true;
        PlaybackStarted?.Invoke();
        this.FadeIn(400);
    }
}
