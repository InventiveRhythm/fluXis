using fluXis.Game.Database.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Graphics.Background;

public partial class Background : CompositeDrawable
{
    public RealmMap Map => map;
    public float Duration { get; set; } = 300;

    private RealmMap map { get; }

    private MapBackground sprite;
    private BufferedContainer buffer;

    public float Blur { get; init; }

    public Background(RealmMap map)
    {
        RelativeSizeAxes = Axes.Both;
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Alpha = 0f;

        sprite = new MapBackground(map)
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            FillMode = FillMode.Fill
        };

        if (Blur > 0)
        {
            AddInternal(buffer = new BufferedContainer(cachedFrameBuffer: true)
            {
                RelativeSizeAxes = Axes.Both,
                BlurSigma = new Vector2(Blur),
                RedrawOnScale = false,
                Child = sprite
            });
        }
        else AddInternal(sprite);
    }

    protected override void LoadComplete()
    {
        this.FadeInFromZero(Duration);
        base.LoadComplete();
    }
}
