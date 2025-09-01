using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Skinning;

public partial class SkinnableSprite : Sprite
{
    public bool SkipResizing { get; set; }
    public int Index { get; set; }

    public SkinnableSprite(Texture texture = null, int index = 0)
    {
        Texture = texture;
        Index = index;
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

    public void SetColor(Colour4 color)
        => Colour = color;

    public void FadeColor(Colour4 color, double duration = 0, Easing easing = Easing.None)
        => this.FadeColour(color, duration, easing);
}
