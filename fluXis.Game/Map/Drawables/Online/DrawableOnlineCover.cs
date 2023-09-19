using fluXis.Game.Online.API.Maps;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Map.Drawables.Online;

public partial class DrawableOnlineCover : Container
{
    private readonly APIMapSet mapSet;

    public DrawableOnlineCover(APIMapSet mapSet)
    {
        this.mapSet = mapSet;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures, Fluxel fluxel)
    {
        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Black,
            Alpha = 0.5f
        };

        LoadComponentAsync(new Sprite
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.Both,
            FillMode = FillMode.Fill,
            Texture = textures.Get($"{fluxel.Endpoint.APIUrl}/assets/cover/{mapSet.Id}")
        }, AddInternal);
    }
}
