using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Graphics.Panel;

public partial class PanelBackground : Sprite
{
    public PanelBackground()
    {
        RelativeSizeAxes = Axes.Both;
        FillMode = FillMode.Fill;
        Anchor = Origin = Anchor.BottomLeft;
        Size = new Vector2(1);
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Texture = textures.Get("Backgrounds/panel");
    }
}
