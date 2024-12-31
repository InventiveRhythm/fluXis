using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace fluXis.Storyboards.Storage;

public class StoryboardStorage
{
    public NativeStorage Storage { get; }
    public LargeTextureStore Textures { get; }

    public StoryboardStorage(GameHost host, string path)
    {
        Storage = new NativeStorage(path);
        var resources = new StorageBackedResourceStore(Storage);

        Textures = new LargeTextureStore(host.Renderer, host.CreateTextureLoaderStore(resources));
    }
}
