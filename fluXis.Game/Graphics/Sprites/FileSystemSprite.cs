using System.IO;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Platform;

namespace fluXis.Game.Graphics.Sprites;

public partial class FileSystemSprite : Sprite
{
    [Resolved]
    private GameHost host { get; set; }

    public int TextureWidth => Texture?.Width ?? 0;
    public int TextureHeight => Texture?.Height ?? 0;

    private readonly string path;

    public FileSystemSprite(string path)
    {
        this.path = path;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Texture = Texture.FromStream(host.Renderer, File.OpenRead(path));
    }
}
