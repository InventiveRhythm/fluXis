using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace fluXis.Game.Storyboards.Storage;

public class StoryboardStorage
{
    public LargeTextureStore Textures { get; }

    public StoryboardStorage(GameHost host, string path)
    {
        var storage = new NativeStorage(path);
        var resources = new StorageBackedResourceStore(storage);

        Textures = new LargeTextureStore(host.Renderer, host.CreateTextureLoaderStore(resources));
    }
}
