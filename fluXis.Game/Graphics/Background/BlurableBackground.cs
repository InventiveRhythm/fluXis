using fluXis.Game.Database.Maps;
using fluXis.Game.Map.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Graphics.Background;

public partial class BlurableBackground : CompositeDrawable
{
    public RealmMap Map { get; }

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
        Alpha = 0f;

        sprite = new MapBackground(Map)
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            FillMode = FillMode.Fill
        };

        if (blur > 0)
        {
            AddInternal(new BufferedContainer(cachedFrameBuffer: true)
            {
                RelativeSizeAxes = Axes.Both,
                BlurSigma = new Vector2(blur * 25),
                RedrawOnScale = false,
                Child = sprite
            });
        }
        else AddInternal(sprite);
    }

    protected override void LoadComplete()
    {
        this.FadeInFromZero(duration);
        base.LoadComplete();
    }
}
