using fluXis.Game.Graphics;
using fluXis.Shared.Components.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Map.Drawables.Online;

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
    private void load(OnlineTextureStore textures)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        RelativeSizeAxes = Axes.Both;
        FillMode = FillMode.Fill;
        Texture = textures.GetCover(mapSet.ID, size);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        this.FadeInFromZero(400);
    }
}
