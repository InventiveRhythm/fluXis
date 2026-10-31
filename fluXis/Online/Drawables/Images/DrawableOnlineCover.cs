using fluXis.Graphics;
using fluXis.Online.API.Models.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Online.Drawables.Images;

[LongRunningLoad]
public partial class DrawableOnlineCover : Sprite
{
    private APIMapSet mapSet { get; }
    private OnlineTextureStore.AssetSize size { get; }

    public DrawableOnlineCover(APIMapSet mapSet, OnlineTextureStore.AssetSize size = OnlineTextureStore.AssetSize.Small)
    {
        this.mapSet = mapSet;
        this.size = size;
    }

    [BackgroundDependencyLoader]
    private void load(OnlineTextureStore online, TextureStore textures)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        RelativeSizeAxes = Axes.Both;
        FillMode = FillMode.Fill;
        Texture = online.GetCover(mapSet.ID, size) ?? textures.Get("Covers/default");
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        this.FadeInFromZero(400);
    }
}
