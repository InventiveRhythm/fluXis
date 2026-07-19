using System;
using fluXis.Storyboards.Storage;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Video;
using osu.Framework.Timing;

namespace fluXis.Storyboards.Drawables.Elements;

public partial class DrawableStoryboardVideo : DrawableStoryboardElement
{
    [Resolved]
    private IFrameBasedClock framedClock { get; set; }

    [CanBeNull]
    private Video video;

    private float offset;
    private bool loop;

    public DrawableStoryboardVideo(StoryboardElement element)
        : base(element)
    {
        if (element.Type != StoryboardElementType.Video)
            throw new ArgumentException("Element provided is not a video", nameof(element));
    }

    [BackgroundDependencyLoader]
    private void load(StoryboardStorage storage)
    {
        var path = Element.GetParameter("path", string.Empty);
        offset = Element.GetParameter("videoOffset", 0);
        loop = Element.GetParameter("videoLoop", false);

        if (!storage.Storage.Exists(path))
        {
            // todo: add fallback container
            return;
        }

        AddInternal(video = new Video(storage.Storage.GetStream(path))
        {
            RelativeSizeAxes = Axes.Both,
            AlwaysPresent = true
        });
    }

    protected override void Update()
    {
        base.Update();

        if (video is null)
            return;

        var position = Math.Max((framedClock.CurrentTime + offset) - Element.StartTime, 0);

        if (loop && video.Duration > 0)
        {
            // just in case the last frame doesn't get skipped
            const float loop_margin = 50f;

            position %= video.Duration + loop_margin;
            if (position > video.Duration)
                position = video.Duration;
        }

        video.PlaybackPosition = position;
    }
}
