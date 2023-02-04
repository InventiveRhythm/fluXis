using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Graphics;

public partial class StripePattern : InfiniteScrollingPattern
{
    public StripePattern()
    {
        RelativeSizeAxes = Axes.Both;
        TextureScale = new Vector2(2);
        Extent = new Vector2(2, 1);
        Speed = new Vector2(-10);
        Blending = BlendingParameters.Additive;
        Alpha = .02f;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Texture = textures.Get("Backgrounds/overlay.png");
    }
}
