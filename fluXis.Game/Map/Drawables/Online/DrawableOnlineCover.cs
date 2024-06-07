using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Components.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Map.Drawables.Online;

public partial class DrawableOnlineCover : Sprite
{
    private readonly APIMapSet mapSet;

    public DrawableOnlineCover(APIMapSet mapSet)
    {
        this.mapSet = mapSet;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures, FluxelClient fluxel)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        RelativeSizeAxes = Axes.Both;
        FillMode = FillMode.Fill;
        Texture = textures.Get($"{fluxel.Endpoint.AssetUrl}/cover/{mapSet.ID}");
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        this.FadeInFromZero(400);
    }
}
