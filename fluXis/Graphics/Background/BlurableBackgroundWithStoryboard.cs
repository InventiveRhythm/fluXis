using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Configuration;
using fluXis.Database.Maps;
using fluXis.Graphics.Shaders;
using fluXis.Map.Drawables;
using fluXis.Storyboards;
using fluXis.Storyboards.Drawables;
using JetBrains.Annotations;
using osu.Framework.Extensions.IEnumerableExtensions;
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
        var info = Map?.GetMapInfo();
        MapBackground background = null;

        foreach (var drawable in base.CreateContent())
        {
            background ??= drawable as MapBackground;
            yield return drawable;
        }

        if (info is null)
            yield break;

        var framed = new FramedClock(GlobalClock.CurrentTrack, false);

        var effects = info.GetMapEvents([], true);
        var shaders = effects.ShaderEvents;

        if (shaders.Count != 0 && Config.Get<bool>(FluXisSetting.ShowBackgroundShaders))
        {
            var grouped = shaders.GroupBy(x => x.Type);

            ShaderStack = new ShaderStackContainer { RelativeSizeAxes = Axes.Both };

            var handlers = new PersistentTransformContainer
            {
                RelativeSizeAxes = Axes.Both,
                Clock = framed
            };

            LoadComponent(handlers);

            foreach (var group in grouped)
            {
                var handle = ShaderStack.AddShader(ShaderStackContainer.CreateForType(group.Key));
                handlers.Add(handle);

                group.ForEach(x => x.Apply(handle));
            }

            AddInternal(handlers);
        }

        var stream = info.GetVideoStream();

        if (stream != null)
        {
            if (background != null) background.Alpha = 0;

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
                yield return new DrawableStoryboardLayer(framed, storyboard, layer);
        }
    }

    protected override BufferedContainer CreateBlur(IEnumerable<Drawable> enu)
    {
        var children = enu.ToList();
        var cache = !children.Any(x => x is Video or DrawableStoryboardLayer);
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
            video.PlaybackPosition = video.Clock.CurrentTime;
    }

    private partial class PersistentTransformContainer : Container
    {
        public override bool RemoveCompletedTransforms => false;
    }
}
