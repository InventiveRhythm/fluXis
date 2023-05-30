using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Skinning;

public partial class SkinnableSprite : Sprite
{
    protected override void Update()
    {
        if (Texture != null)
        {
            if (RelativeSizeAxes == Axes.X)
                Height = DrawWidth / Texture.DisplayWidth * Texture.DisplayHeight;
            else if (RelativeSizeAxes == Axes.Y)
                Width = DrawHeight / Texture.DisplayHeight * Texture.DisplayWidth;
        }
    }
}
