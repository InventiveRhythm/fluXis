using System.Collections.Generic;
using System.Linq;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Database.Maps;
using fluXis.Graphics.Shaders;
using fluXis.Map.Drawables;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Graphics.Background;

public partial class BlurableBackground : Container
{
    [Resolved]
    protected GlobalClock GlobalClock { get; private set; }

    [Resolved]
    protected FluXisConfig Config { get; private set; }

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

    [CanBeNull]
    protected ShaderStackContainer ShaderStack;

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
        var content = CreateContent().ToList();
        var target = (Container)ShaderStack ?? this;

        if (blur > 0)
            target.Add(blurContainer = CreateBlur(content));
        else
            target.AddRange(content);

        if (ShaderStack != null)
            Add(ShaderStack);

        updateBlur();
    }

    private void updateBlur()
    {
        if (blurContainer is null && blur > 0)
        {
            var children = InternalChildren.ToList();
            ClearInternal(false);
            InternalChild = blurContainer = CreateBlur(children);
        }

        if (blurContainer is null)
            return;

        blurContainer!.BlurSigma = new Vector2(blur * 12);
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

    public bool IsSameAs(BlurableBackground other)
    {
        return GetType() == other.GetType() && Equals(Map, other.Map);
    }
}
