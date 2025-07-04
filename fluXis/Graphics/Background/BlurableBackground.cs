using System.Collections.Generic;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Map.Drawables;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Graphics.Background;

public partial class BlurableBackground : CompositeDrawable
{
    [Resolved]
    protected GlobalClock GlobalClock { get; private set; }

    public RealmMap Map { get; }

    public float BlurStrength
    {
        get => blur;
        set
        {
            blur = value;
            updateBlur();
        }
    }

    private float blur;

    [CanBeNull]
    private BufferedContainer blurContainer;

    public BlurableBackground(RealmMap map, float blur)
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;

        Map = map;
        this.blur = blur;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        var content = CreateContent();

        if (blur > 0)
            InternalChild = blurContainer = CreateBlur(content);
        else
            InternalChildrenEnumerable = content;

        updateBlur();
    }

    private void updateBlur()
    {
        if (blurContainer is null)
            return;

        blurContainer.BlurSigma = new Vector2(blur * 12);
        blurContainer.FrameBufferScale = new Vector2(1 - blur * .5f);
    }

    protected virtual IEnumerable<Drawable> CreateContent() => new MapBackground(Map)
    {
        RelativeSizeAxes = Axes.Both,
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        FillMode = FillMode.Fill
    }.Yield();

    protected virtual BufferedContainer CreateBlur(IEnumerable<Drawable> children) => new(cachedFrameBuffer: true)
    {
        RelativeSizeAxes = Axes.Both,
        RedrawOnScale = false,
        ChildrenEnumerable = children
    };
}
