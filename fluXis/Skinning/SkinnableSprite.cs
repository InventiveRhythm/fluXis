using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Skinning;

public partial class SkinnableSprite : Sprite
{
    public bool SkipResizing { get; set; }

    public SkinnableSprite(Texture texture = null)
    {
        Texture = texture;
    }

    protected override void Update()
    {
        if (SkipResizing)
            return;

        if (Texture != null)
        {
            switch (RelativeSizeAxes)
            {
                case Axes.X:
                    Height = DrawWidth / Texture.DisplayWidth * Texture.DisplayHeight;
                    break;

                case Axes.Y:
                    Width = DrawHeight / Texture.DisplayHeight * Texture.DisplayWidth;
                    break;
            }
        }
    }
}
