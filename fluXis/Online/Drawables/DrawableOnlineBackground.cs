using fluXis.Graphics;
using fluXis.Online.API.Models.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Online.Drawables;

[LongRunningLoad]
public partial class DrawableOnlineBackground : Sprite
{
    private long id { get; }
    private OnlineTextureStore.AssetSize size { get; }

    public DrawableOnlineBackground(APIMapSet set, OnlineTextureStore.AssetSize size = OnlineTextureStore.AssetSize.Small)
        : this(size)
    {
        id = set.ID;
    }

    public DrawableOnlineBackground(APIMap map, OnlineTextureStore.AssetSize size = OnlineTextureStore.AssetSize.Small)
        : this(size)
    {
        id = map.MapSetID;
    }

    private DrawableOnlineBackground(OnlineTextureStore.AssetSize size = OnlineTextureStore.AssetSize.Small)
    {
        this.size = size;
    }

    [BackgroundDependencyLoader]
    private void load(OnlineTextureStore textures)
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        FillMode = FillMode.Fill;
        Texture = textures.GetBackground(id, size);
    }
}

