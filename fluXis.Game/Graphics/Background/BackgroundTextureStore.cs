using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace fluXis.Game.Graphics.Background
{
    public class BackgroundTextureStore : TextureStore
    {
        public BackgroundTextureStore(GameHost host, Storage storage)
            : base(host.Renderer, host.CreateTextureLoaderStore(new StorageBackedResourceStore(storage.GetStorageForDirectory("maps"))))
        {
        }
    }
}
