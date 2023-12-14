using System;
using System.IO;
using fluXis.Game.Audio;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Video;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Graphics.Background;

public partial class BackgroundVideo : CompositeDrawable
{
    [Resolved]
    private GlobalClock clock { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    public RealmMap Map { get; set; }
    public MapInfo Info { get; set; }

    public bool IsPlaying { get; private set; }

    private Video video;

    public override bool RemoveWhenNotAlive => false;

    public BackgroundVideo()
    {
        RelativeSizeAxes = Axes.Both;
    }

    public void LoadVideo()
    {
        if (string.IsNullOrWhiteSpace(Info.VideoFile))
        {
            Schedule(() =>
            {
                if (video == null) return;

                if (!video.IsLoaded)
                {
                    video.OnLoadComplete += v => v.Expire();
                    Logger.Log("[LoadVideo] Video is not loaded, waiting for load to complete.", LoggingTarget.Runtime, LogLevel.Debug);
                    return;
                }

                video.FadeOut(500).Expire();
                Logger.Log("Video is loaded, fading out.", LoggingTarget.Runtime, LogLevel.Debug);
                video = null;
            });
            return;
        }

        var file = Map.MapSet.GetPathForFile(Info.VideoFile);

        if (file == null) return;

        try
        {
            var path = MapFiles.GetFullPath(file);
            Logger.Log($"Loading video: {path}", LoggingTarget.Runtime, LogLevel.Debug);
            Stream stream = File.OpenRead(path);

            Schedule(() =>
            {
                LoadComponentAsync(video = new Video(stream, false)
                {
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fill,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0
                }, loadedVideo =>
                {
                    Logger.Log("Video loaded, adding to scene tree.", LoggingTarget.Runtime, LogLevel.Debug);
                    InternalChild = loadedVideo;
                });
            });
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to load video: {file}");
        }
    }

    protected override void Update()
    {
        base.Update();

        if (video is not { IsLoaded: true }) return;

        if (clock.CurrentTime > video.Duration)
        {
            video.FadeOut(500);
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
            Logger.Log("[Stop] Video is not loaded, waiting for load to complete.", LoggingTarget.Runtime, LogLevel.Debug);
            return;
        }

        IsPlaying = false;
        Logger.Log("Stopping video.", LoggingTarget.Runtime, LogLevel.Debug);
        video.FadeOut(500);
    }

    public void Start()
    {
        if (video == null) return;

        if (!video.IsLoaded)
        {
            video.OnLoadComplete += _ => Start();
            Logger.Log("[Start] Video is not loaded, waiting for load to complete.", LoggingTarget.Runtime, LogLevel.Debug);
            return;
        }

        IsPlaying = true;
        Logger.Log("Starting video.", LoggingTarget.Runtime, LogLevel.Debug);
        video.FadeIn(500);
    }
}
