using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Localization;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Mods.Drawables;

public partial class ModIcon : Container, IHasTooltip
{
    public LocalisableString TooltipText => LocalizationStrings.Mods.GetName(Mod);

    public IMod Mod { get; set; }

    private FillFlowContainer flow;
    private SpriteIcon icon;

    public float IconWidthRate { get; set; } = 80;
    public float FlowSpacing { get; set; } = 4;
    public float IconSize { get; set; } = 24;

    public ModIcon()
    {
        Width = 80;
        Height = 40;
        CornerRadius = 10;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        if (Mod is null) return;

        Masking = true;
        Shear = new Vector2(0.2f, 0);
        EdgeEffect = Styling.ShadowSmall;

        var color = Theme.GetModTypeColor(Mod.Type);

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = color
            },
            flow = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(FlowSpacing),
                Children = new Drawable[]
                {
                    icon = new FluXisSpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = Mod.Icon,
                        Size = new Vector2(IconSize),
                        Colour = Theme.IsBright(color) ? Theme.TextDark : Theme.Text,
                        Shear = new Vector2(-0.2f, 0)
                    }
                }
            }
        };

        if (Mod is RateMod)
        {
            Width = IconWidthRate;

            flow.Add(new FluXisSpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Text = Mod.Acronym,
                Colour = Theme.IsBright(color) ? Theme.TextDark : Theme.Text,
                FontSize = 18,
                Shear = new Vector2(-0.2f, 0)
            });

            icon.Scale = new Vector2(0.8f);
        }
    }
}
