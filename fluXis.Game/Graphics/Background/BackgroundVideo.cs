using System;
using System.IO;
using fluXis.Game.Audio;
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
    private AudioClock clock { get; set; }

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
        if (Info.VideoFile == null)
        {
            Schedule(() =>
            {
                ClearInternal();
                video = null;
            });
            return;
        }

        var file = Map.MapSet.GetFile(Info.VideoFile);

        if (file == null) return;

        try
        {
            string path = storage.GetFullPath($"files/{file.GetPath()}");
            Stream stream = File.OpenRead(path);

            LoadComponent(video = new Video(stream, false)
            {
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0
            });

            Schedule(() => InternalChild = video);
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to load video: {file.GetPath()}");
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

        IsPlaying = false;
        video.FadeOut(500);
    }

    public void Start()
    {
        if (video == null) return;

        IsPlaying = true;
        video.FadeIn(500);
    }
}
