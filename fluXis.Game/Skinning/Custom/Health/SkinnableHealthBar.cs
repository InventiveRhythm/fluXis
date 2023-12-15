using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Skinning.Custom.Health;

public partial class SkinnableHealthBar : Container
{
    private Texture texture { get; }

    public SkinnableHealthBar(Texture texture)
    {
        this.texture = texture;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Masking = true;

        Children = new Drawable[]
        {
            new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(1),
                Texture = texture,
                FillMode = FillMode.Fill,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre
            }
        };
    }
}
