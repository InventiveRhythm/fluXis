using System.IO;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Video;
using osu.Framework.Platform;

namespace fluXis.Game.Graphics.Background;

public partial class BackgroundVideo : CompositeDrawable
{
    public RealmMap Map { get; set; }
    public MapInfo Info { get; set; }

    public int Delay { get; set; } = 2000;
    public bool IsPlaying { get; private set; }

    private Video video;
    private Storage storage;

    public override bool RemoveWhenNotAlive => false;

    public BackgroundVideo()
    {
        RelativeSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        storage = this.storage = storage.GetStorageForDirectory("files");
    }

    public void LoadVideo()
    {
        if (Info.VideoFile == null) return;

        var file = Map.MapSet.GetFile(Info.VideoFile);

        if (file == null) return;

        try
        {
            string path = storage.GetFullPath(file.GetPath());
            Stream stream = File.OpenRead(path);

            InternalChild = video = new Video(stream, false)
            {
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0
            };

            Scheduler.AddDelayed(() =>
            {
                video.FadeIn(500);
                IsPlaying = true;
            }, Delay);
        }
        catch { }
    }

    protected override void Update()
    {
        base.Update();

        if (video == null) return;

        if (Conductor.CurrentTime > video.Duration)
        {
            video.FadeOut(500);
            return;
        }

        if (IsPlaying)
            video.PlaybackPosition = Conductor.CurrentTime;
    }

    public void Stop()
    {
        if (video == null) return;

        IsPlaying = false;
        video.FadeOut(500);
    }
}
