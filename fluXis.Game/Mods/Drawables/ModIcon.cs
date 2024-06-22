using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Mods.Drawables;

public partial class ModIcon : Container
{
    public IMod Mod { get; set; }

    private SpriteIcon icon;

    [BackgroundDependencyLoader]
    private void load()
    {
        if (Mod is null) return;

        Width = 80;
        Height = 40;
        CornerRadius = 10;
        Masking = true;
        Shear = new Vector2(0.2f, 0);
        EdgeEffect = FluXisStyles.ShadowSmall;

        var color = FluXisColors.GetModTypeColor(Mod.Type);

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = color
            },
            icon = new SpriteIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Icon = Mod.Icon,
                Size = new Vector2(24),
                Colour = FluXisColors.IsBright(color) ? FluXisColors.TextDark : FluXisColors.Text,
                Shear = new Vector2(-0.2f, 0)
            }
        };

        if (Mod is RateMod)
        {
            Add(new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
                Text = Mod.Acronym,
                Colour = FluXisColors.IsBright(color) ? FluXisColors.TextDark : FluXisColors.Text,
                FontSize = 18,
                Shear = new Vector2(-0.2f, 0)
            });

            icon.Scale = new Vector2(0.8f);
            icon.Origin = Anchor.CentreRight;
            icon.X = -5;
        }
    }
}
