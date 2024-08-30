using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Map.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Graphics.Background;

public partial class BlurableBackground : CompositeDrawable, IHasLoadedValue
{
    public RealmMap Map { get; }
    public bool Loaded { get; private set; }

    private float blur { get; }
    private float duration { get; }

    private MapBackground sprite;

    public BlurableBackground(RealmMap map, float blur, float duration = 300)
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;

        Map = map;
        this.blur = blur;
        this.duration = duration;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        sprite = new MapBackground(Map)
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            FillMode = FillMode.Fill
        };

        var wrapper = new LoadWrapper<Drawable>
        {
            RelativeSizeAxes = Axes.Both,
            OnComplete = d => d.FadeInFromZero(duration).OnComplete(_ => Loaded = true)
        };

        if (blur > 0)
        {
            wrapper.LoadContent = () => new BufferedContainer(cachedFrameBuffer: true)
            {
                RelativeSizeAxes = Axes.Both,
                BlurSigma = new Vector2(blur * 25),
                RedrawOnScale = false,
                Child = sprite
            };
        }
        else wrapper.LoadContent = () => sprite;

        InternalChild = wrapper;
    }
}
