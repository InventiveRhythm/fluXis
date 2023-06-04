using fluXis.Game.Database.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Graphics.Background;

public partial class Background : CompositeDrawable
{
    public RealmMap Map => sprite.Map;
    public float Duration { get; set; } = 300;

    private readonly MapBackground sprite;
    private readonly BufferedContainer buffer;

    public float Blur
    {
        get => buffer.BlurSigma.X / 25;
        set => buffer.BlurSigma = new Vector2(value * 25);
    }

    public Background(RealmMap map)
    {
        RelativeSizeAxes = Axes.Both;

        AddInternal(buffer = new BufferedContainer(cachedFrameBuffer: true)
        {
            RelativeSizeAxes = Axes.Both,
            RedrawOnScale = false,
            Child = sprite = new MapBackground(map)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill
            }
        });
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures, BackgroundTextureStore backgrounds)
    {
        Alpha = 0f;
    }

    protected override void LoadComplete()
    {
        this.FadeInFromZero(Duration);
        base.LoadComplete();
    }
}
