using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Graphics.Sprites;

// couldn't think of a better name
public partial class CustomSprite : Sprite
{
    private string path { get; }

    public CustomSprite(string path)
    {
        this.path = path;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Texture = textures.Get(path);
    }
}
