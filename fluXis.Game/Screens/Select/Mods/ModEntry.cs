using System;
using fluXis.Game.Graphics;
using fluXis.Game.Mods;
using fluXis.Game.Overlay.Mouse;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Mods;

public partial class ModEntry : Container, IHasTooltip
{
    public string Tooltip => Mod.Description;

    public ModSelector Selector { get; set; }

    public IMod Mod { get; init; }
    public string HexColour { get; init; }

    public bool Selected;

    private Box background;
    private Box hoverBox;
    private SpriteIcon icon;
    private SpriteText name;
    private SpriteText description;
    private SpriteText scoreMultiplier;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;
        CornerRadius = 3;
        Masking = true;

        int multiplier = (int)Math.Round((Mod.ScoreMultiplier - 1) * 100);
        string multiplierText = multiplier > 0 ? $"+{multiplier}" : multiplier.ToString();

        InternalChildren = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Surface,
            },
            hoverBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 12 },
                Children = new Drawable[]
                {
                    icon = new SpriteIcon
                    {
                        Size = new Vector2(25),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Shadow = true,
                        Icon = Mod.Icon
                    },
                    name = new SpriteText
                    {
                        Font = FluXisFont.Default(),
                        Text = Mod.Name,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.BottomLeft,
                        Shadow = true,
                        X = 35,
                        Y = 4
                    },
                    description = new SpriteText
                    {
                        Font = FluXisFont.Default(14),
                        Text = Mod.Description,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.TopLeft,
                        Colour = FluXisColors.Text2,
                        Shadow = true,
                        X = 35
                    },
                    scoreMultiplier = new SpriteText
                    {
                        Font = FluXisFont.Default(),
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Text = $"{multiplierText}%"
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        hoverBox.FadeTo(.2f).FadeTo(.1f, 200);

        Selected = !Selected;

        UpdateSelected();

        if (Selected) Selector.Select(this);
        else Selector.Deselect(this);

        return base.OnClick(e);
    }

    public void UpdateSelected()
    {
        var color = Selected ? FluXisColors.TextDark : FluXisColors.Text;

        background.FadeColour(Selected ? Colour4.FromHex(HexColour) : FluXisColors.Surface, 200);
        icon.FadeColour(color, 200);
        name.FadeColour(color, 200);
        description.FadeColour(color, 200);
        scoreMultiplier.FadeColour(color, 200);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hoverBox.FadeTo(0.1f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hoverBox.FadeTo(0, 200);
    }
}
