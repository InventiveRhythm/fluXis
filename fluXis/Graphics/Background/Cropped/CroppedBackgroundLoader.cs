using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace fluXis.Graphics.Background.Cropped;

public class CroppedBackgroundLoader : IResourceStore<TextureUpload>
{
    private readonly IResourceStore<TextureUpload> textureStore;

    public CroppedBackgroundLoader(IResourceStore<TextureUpload> textureStore)
    {
        this.textureStore = textureStore;
    }

    public void Dispose()
    {
        textureStore.Dispose();
        GC.SuppressFinalize(this);
    }

    public TextureUpload Get(string name)
    {
        var textureUpload = textureStore?.Get(name);
        return textureUpload == null ? null : cropTexture(textureUpload);
    }

    public async Task<TextureUpload> GetAsync(string name, CancellationToken cancellationToken = new())
    {
        if (textureStore == null) return null;

        var textureUpload = await textureStore.GetAsync(name, cancellationToken).ConfigureAwait(false);
        if (textureUpload == null) return null;

        return await Task.Run(() => cropTexture(textureUpload), cancellationToken).ConfigureAwait(false);
    }

    public Stream GetStream(string name) => textureStore.GetStream(name);
    public IEnumerable<string> GetAvailableResources() => textureStore.GetAvailableResources();

    private static TextureUpload cropTexture(TextureUpload tex)
    {
        var image = Image.LoadPixelData<Rgba32>(tex.Data.ToArray(), tex.Width, tex.Height);

        var visibleSize = new Size(1000, 100);

        if (visibleSize.Width > image.Width)
        {
            var ratio = image.Width / (float)visibleSize.Width;
            visibleSize = (Size)(visibleSize * ratio);
        }

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = visibleSize,
            Mode = ResizeMode.Crop
        }));

        return new TextureUpload(image);
    }
}
