using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osuTK;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace fluXis.Game.Graphics.Background.Cropped;

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

        var visibleSize = new Vector2(1000, 100);
        var ratio = visibleSize.X / visibleSize.Y;

        var imageRatio = image.Width / image.Height;

        if (imageRatio > ratio)
        {
            var newWidth = (int)(image.Height * ratio);
            var cropX = (image.Width - newWidth) / 2;
            image.Mutate(x => x.Crop(new Rectangle(Math.Max(1, cropX), 0, Math.Max(1, newWidth), image.Height)));
        }
        else
        {
            var newHeight = (int)(image.Width / ratio);
            var cropY = (image.Height - newHeight) / 2;
            image.Mutate(x => x.Crop(new Rectangle(0, Math.Max(1, cropY), image.Width, Math.Max(1, newHeight))));
        }

        return new TextureUpload(image);
    }
}
