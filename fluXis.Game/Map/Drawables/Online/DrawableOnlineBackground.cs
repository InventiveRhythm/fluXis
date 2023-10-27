using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Map.Drawables.Online;

public partial class DrawableOnlineBackground : Sprite
{
    private readonly APIMapSet mapSet;

    public DrawableOnlineBackground(APIMapSet mapSet)
    {
        this.mapSet = mapSet;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures, Fluxel fluxel)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        RelativeSizeAxes = Axes.Both;
        FillMode = FillMode.Fill;
        Texture = textures.Get($"{fluxel.Endpoint.APIUrl}/assets/background/{mapSet.Id}");
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        this.FadeInFromZero(400);
    }
}

