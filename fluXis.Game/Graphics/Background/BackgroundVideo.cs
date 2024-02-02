using System;
using System.IO;
using fluXis.Game.Configuration;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Video;
using osu.Framework.Logging;

namespace fluXis.Game.Graphics.Background;

public partial class BackgroundVideo : CompositeDrawable
{
    public bool ShowDim { get; init; } = true;

    public RealmMap Map { get; set; }
    public MapInfo Info { get; set; }

    public bool IsPlaying { get; private set; }

    private Container videoContainer;
    private Video video;
    private Box dim;

    private Bindable<float> backgroundDim;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
        Alpha = 0;

        backgroundDim = config.GetBindable<float>(FluXisSetting.BackgroundDim);

        InternalChildren = new Drawable[]
        {
            videoContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            },
            dim = new Box
            {
                Colour = Colour4.Black,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (ShowDim)
            backgroundDim.BindValueChanged(v => dim.FadeTo(v.NewValue, 400, Easing.OutQuint), true);
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

                this.FadeOut(500);
                video.Delay(500).Expire();
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
                    Origin = Anchor.Centre
                }, loadedVideo =>
                {
                    Logger.Log("Video loaded, adding to scene tree.", LoggingTarget.Runtime, LogLevel.Debug);
                    videoContainer.Child = loadedVideo;
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

        if (Clock.CurrentTime > video.Duration)
        {
            this.FadeOut(500);
            return;
        }

        if (IsPlaying)
            video.PlaybackPosition = Clock.CurrentTime;
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
        this.FadeOut(500);
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
        this.FadeIn(500);
    }
}
