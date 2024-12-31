using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Graphics.UserInterface.Panel;

public partial class PanelBackground : Container
{
    public PanelBackground()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.BottomLeft;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background1
            },
            new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Texture = textures.Get("Backgrounds/panel_base"),
                Colour = FluXisColors.Background2,
                Alpha = 0.5f,
                Size = new Vector2(1)
            },
            new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Texture = textures.Get("Backgrounds/panel_lines"),
                Colour = FluXisColors.Background2,
                Size = new Vector2(1)
            }
        };
    }
}
