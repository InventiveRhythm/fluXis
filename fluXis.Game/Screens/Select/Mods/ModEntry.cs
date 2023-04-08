using fluXis.Game.Graphics;
using fluXis.Game.Mods;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Mods;

public partial class ModEntry : Container
{
    public ModSelector ParentSelector { get; set; }
    public bool Selected = false;
    public IMod Mod { get; }

    private readonly Box hoverBox;

    public ModEntry(IMod mod)
    {
        Mod = mod;

        Width = 625;
        Height = 100;
        CornerRadius = 10;
        Masking = true;
        EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Glow,
            Colour = FluXisColors.Accent2,
            Radius = 10,
        };

        FadeEdgeEffectTo(0);

        InternalChildren = new Drawable[]
        {
            new Box
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
                Padding = new MarginPadding { Horizontal = 25 },
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Size = new Vector2(50),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Icon = mod.Icon
                    },
                    new SpriteText
                    {
                        Font = FluXisFont.Default(30),
                        Text = mod.Name,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.BottomLeft,
                        Margin = new MarginPadding { Left = 75 },
                        Y = 5
                    },
                    new SpriteText
                    {
                        Font = FluXisFont.Default(),
                        Text = mod.Description,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.TopLeft,
                        Colour = FluXisColors.Text2,
                        Margin = new MarginPadding { Left = 75 }
                    },
                    new SpriteText
                    {
                        Font = FluXisFont.Default(),
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Text = $"{mod.ScoreMultiplier}x"
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        hoverBox.FadeTo(.2f).FadeTo(.1f, 200);

        Selected = !Selected;
        FadeEdgeEffectTo(Selected ? 1f : 0, 200);

        if (Selected) ParentSelector.OnModSelected(this);
        else ParentSelector.OnModDeselected(this);

        return base.OnClick(e);
    }

    public void Deselect()
    {
        Selected = false;
        FadeEdgeEffectTo(0, 200);
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
