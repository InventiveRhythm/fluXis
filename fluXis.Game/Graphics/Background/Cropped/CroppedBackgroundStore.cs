using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace fluXis.Game.Graphics.Background.Cropped;

public class CroppedBackgroundStore : LargeTextureStore
{
    public CroppedBackgroundStore(GameHost host, Storage storage)
        : base(host.Renderer, new CroppedBackgroundLoader(host.CreateTextureLoaderStore(new StorageBackedResourceStore(storage.GetStorageForDirectory("files")))))
    {
    }
}
