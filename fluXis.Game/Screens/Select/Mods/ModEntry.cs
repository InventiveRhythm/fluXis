using System;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Mods;
using fluXis.Game.Mods.Drawables;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Mods;

public partial class ModEntry : Container, IHasDrawableTooltip
{
    public ModSelector Selector { get; set; }

    public IMod Mod { get; init; }
    public string HexColour { get; init; }

    public bool Selected;

    private Box background;
    private Box hoverBox;
    private SpriteIcon icon;
    private FluXisSpriteText name;
    private FluXisSpriteText description;
    private FluXisSpriteText scoreMultiplier;

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
                Colour = FluXisColors.Surface
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
                    name = new FluXisSpriteText
                    {
                        Text = Mod.Name,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.BottomLeft,
                        Shadow = true,
                        X = 35,
                        Y = 4
                    },
                    description = new FluXisSpriteText
                    {
                        FontSize = 14,
                        Text = Mod.Description,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.TopLeft,
                        Colour = FluXisColors.Text2,
                        Shadow = true,
                        X = 35
                    },
                    scoreMultiplier = new FluXisSpriteText
                    {
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

        if (Selected) Selector.Select(Mod);
        else Selector.Deselect(Mod);

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

    public Drawable GetTooltip()
    {
        var flow = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Padding = new MarginPadding(10),
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = Mod.Name,
                    FontSize = 28,
                    Shadow = true
                },
                new FluXisSpriteText
                {
                    Text = Mod.Description,
                    FontSize = 20,
                    Colour = FluXisColors.Text2,
                    Shadow = true
                }
            }
        };

        if (Mod.IncompatibleMods.Length > 0)
        {
            flow.AddRange(new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = "Incompatible Mods:",
                    FontSize = 20,
                    Margin = new MarginPadding { Top = 10, Bottom = 5 },
                    Shadow = true
                },
                new ModList
                {
                    Scale = new Vector2(.8f),
                    Mods = Mod.IncompatibleMods.Select(ModUtils.GetFromAcronym).ToList()
                }
            });
        }

        return flow;
    }
}
