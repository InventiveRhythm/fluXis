using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace fluXis.Game.Graphics;

public class OnlineTextureStore : LargeTextureStore
{
    public OnlineTextureStore(GameHost host)
        : base(host.Renderer)
    {
        AddTextureSource(host.CreateTextureLoaderStore(new OnlineStore()));
        AddTextureSource(host.CreateTextureLoaderStore(new HttpOnlineStore()));
    }
}
