using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Skinning;

public partial class SkinnableSprite : Sprite
{
    public SkinnableSprite(Texture texture = null)
    {
        Texture = texture;
    }

    protected override void Update()
    {
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
