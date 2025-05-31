using osu.Framework.Allocation;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Graphics.Sprites;

public partial class CustomTextureSprite : Sprite
{
    private TextureAtlas atlas { get; }
    private TextureUpload upload { get; }

    public CustomTextureSprite(TextureAtlas atlas, TextureUpload upload)
    {
        this.atlas = atlas;
        this.upload = upload;
    }

    [BackgroundDependencyLoader]
    private void load(IRenderer renderer)
    {
        var texture = atlas.Add(upload.Width, upload.Height) ?? renderer.CreateTexture(upload.Width, upload.Height);
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
