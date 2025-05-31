using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Online.Fluxel;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Network;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace fluXis.Graphics.Sprites.Animated;

public class AnimatedSpriteStore : IResourceStore<AnimatedSpriteStore.AnimatedTexture[]>
{
    private IRenderer renderer { get; }
    private Storage storage { get; }
    private IAPIClient api { get; }

    private TextureAtlas atlas { get; }
    private Dictionary<string, AnimatedTexture[]> cache { get; }

    private SemaphoreSlim semaphore;

    public AnimatedSpriteStore(GameHost host, IAPIClient api)
    {
        renderer = host.Renderer;
        storage = host.CacheStorage.GetStorageForDirectory("animations");
        this.api = api;

        atlas = new TextureAtlas(renderer, TextureStore.MAX_ATLAS_SIZE, TextureStore.MAX_ATLAS_SIZE);
        cache = new Dictionary<string, AnimatedTexture[]>();
        semaphore = new SemaphoreSlim(1, 1);
    }

    public AnimatedTexture[] Get(string name, OnlineTextureStore.AssetType type)
    {
        try
        {
            semaphore.Wait();

            if (cache.TryGetValue(name, out var existing))
                return existing;

            var path = storage.GetFullPath($"{name}.gif");

            if (!storage.Exists(path) && !download(path, name, type))
                return null;

            using var stream = File.OpenRead(path);
            using var image = Image.Load<Rgba32>(stream);

            var frames = new List<AnimatedTexture>();

            for (var i = 0; i < image.Frames.Count; i++)
            {
                var frame = image.Frames[i];
                var meta = frame.Metadata.GetGifMetadata();

                var clone = image.Frames.CloneFrame(i);
                var upload = new TextureUpload(clone);

                var texture = atlas.Add(upload.Width, upload.Height) ?? renderer.CreateTexture(upload.Width, upload.Height);
                texture.SetData(upload);

                frames.Add(new AnimatedTexture(texture, meta.FrameDelay * 10));
            }

            var arr = frames.ToArray();
            cache[name] = arr;
            return arr;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to load animated texture!");
        }
        finally
        {
            semaphore.Release();
        }

        return null;
    }

    private bool download(string path, string hash, OnlineTextureStore.AssetType type)
    {
        var dir = type switch
        {
            OnlineTextureStore.AssetType.Avatar => "avatar",
            OnlineTextureStore.AssetType.Banner => "banner",
            _ => throw new ArgumentOutOfRangeException()
        };

        var req = new WebRequest($"{api.Endpoint.AssetUrl}/{dir}/{hash}_a.gif");
        req.AllowInsecureRequests = true;
        req.Perform();

        var data = req.GetResponseData();
        if (data is null) return false;

        File.WriteAllBytes(path, data);
        return true;
    }

    [Obsolete("Use Get(string, AssetType).")]
    public AnimatedTexture[] Get(string name) => throw new Exception("Use Get(string, AssetType).");

    public Task<AnimatedTexture[]> GetAsync(string name, CancellationToken cancellationToken = default) => throw new Exception("Not implemented.");
    public Stream GetStream(string name) => throw new Exception("Not implemented.");
    public IEnumerable<string> GetAvailableResources() => throw new Exception("Not implemented.");

    public void Dispose()
    {
        // TODO release managed resources here
    }

    public class AnimatedTexture
    {
        public Texture Texture { get; }
        public float Duration { get; }

        public AnimatedTexture(Texture texture, float duration)
        {
            Texture = texture;
            Duration = duration;
        }
    }
}
