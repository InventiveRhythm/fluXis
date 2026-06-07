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

    public DrawableStoryboardVideo(StoryboardElement element)
        : base(element)
    {
        if (element.Type != StoryboardElementType.Video)
            throw new ArgumentException("Element provided is not a video", nameof(element));
    }

    [BackgroundDependencyLoader]
    private void load(StoryboardStorage storage, IFrameBasedClock framedClock)
    {
        var path = Element.GetParameter("path", string.Empty);

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

        video.PlaybackPosition = Math.Max(framedClock.CurrentTime - Element.StartTime, 0);
    }
}
