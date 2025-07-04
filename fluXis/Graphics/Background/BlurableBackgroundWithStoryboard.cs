using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Storyboards;
using fluXis.Storyboards.Drawables;
using JetBrains.Annotations;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Video;
using osu.Framework.Timing;

namespace fluXis.Graphics.Background;

public partial class BlurableBackgroundWithStoryboard : BlurableBackground
{
    [CanBeNull]
    private Video video;

    public BlurableBackgroundWithStoryboard(RealmMap map, float blur)
        : base(map, blur)
    {
    }

    protected override IEnumerable<Drawable> CreateContent()
    {
        var info = Map.GetMapInfo();

        foreach (var drawable in base.CreateContent())
            yield return drawable;

        if (info is null)
            yield break;

        var framed = new FramedClock(GlobalClock.CurrentTrack, false);

        var stream = info.GetVideoStream();

        if (stream != null)
        {
            yield return video = new Video(stream)
            {
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Clock = framed
            };
        }

        var storyboard = info.CreateDrawableStoryboard();

        if (storyboard != null)
        {
            LoadComponent(storyboard);
            var layers = Enum.GetValues<StoryboardLayer>();

            foreach (var layer in layers)
                yield return new DrawableStoryboardWrapper(framed, storyboard, layer);
        }
    }

    protected override BufferedContainer CreateBlur(IEnumerable<Drawable> enu)
    {
        var children = enu.ToList();
        var cache = !children.Any(x => x is Video or DrawableStoryboardWrapper);
        return new BufferedContainer(cachedFrameBuffer: cache)
        {
            RelativeSizeAxes = Axes.Both,
            RedrawOnScale = false,
            ChildrenEnumerable = children
        };
    }

    protected override void Update()
    {
        base.Update();

        if (video != null)
            video.PlaybackPosition = GlobalClock.CurrentTime;
    }
}
