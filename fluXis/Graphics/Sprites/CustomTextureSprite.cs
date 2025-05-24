using osu.Framework.Allocation;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Graphics.Sprites;

public partial class CustomTextureSprite : Sprite
{
    private TextureUpload upload { get; }

    public CustomTextureSprite(TextureUpload upload)
    {
        this.upload = upload;
    }

    [BackgroundDependencyLoader]
    private void load(IRenderer renderer)
    {
        var texture = renderer.CreateTexture(upload.Width, upload.Height);
        texture.SetData(upload);
        Texture = texture;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Texture?.Dispose();
        upload?.Dispose();
    }
}
