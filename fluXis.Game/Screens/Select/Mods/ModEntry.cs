using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Localization;
using fluXis.Game.Mods;
using fluXis.Game.Mods.Drawables;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Mods;

public partial class ModEntry : Container, IHasCustomTooltip<ModEntry>
{
    public ModEntry TooltipContent => this;

    public ModSelector Selector { get; set; }

    public IMod Mod { get; init; }
    public string HexColour { get; init; }

    [Resolved]
    private UISamples samples { get; set; }

    public bool Selected;

    private Box background;
    private Box hover;
    private Box flash;
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
                Colour = FluXisColors.Background3
            },
            hover = new Box
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
                        Text = LocalizationStrings.Mods.GetName(Mod),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.BottomLeft,
                        Shadow = true,
                        X = 35,
                        Y = 4
                    },
                    description = new FluXisSpriteText
                    {
                        FontSize = 14,
                        Text = LocalizationStrings.Mods.GetDescription(Mod),
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
            },
            flash = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        samples.Click();

        Selected = !Selected;

        UpdateSelected();

        if (Selected) Selector.Select(Mod);
        else Selector.Deselect(Mod);

        return base.OnClick(e);
    }

    public void UpdateSelected()
    {
        var color = Selected ? FluXisColors.TextDark : FluXisColors.Text;

        background.FadeColour(Selected ? Colour4.FromHex(HexColour) : FluXisColors.Background3);
        icon.FadeColour(color);
        name.FadeColour(color);
        description.FadeColour(color);
        scoreMultiplier.FadeColour(color);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 50);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeTo(0, 200);
    }

    public ITooltip<ModEntry> GetCustomTooltip() => new ModEntryTooltip();

    private partial class ModEntryTooltip : CustomTooltipContainer<ModEntry>
    {
        private FluXisSpriteText name { get; }
        private FluXisSpriteText description { get; }
        private FluXisSpriteText incompatibleMods { get; }
        private ModList modList { get; }

        public ModEntryTooltip()
        {
            CornerRadius = 10;
            Child = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(10),
                Children = new Drawable[]
                {
                    name = new FluXisSpriteText
                    {
                        FontSize = 28,
                        Shadow = true
                    },
                    description = new FluXisSpriteText
                    {
                        FontSize = 20,
                        Colour = FluXisColors.Text2,
                        Shadow = true
                    },
                    incompatibleMods = new FluXisSpriteText
                    {
                        Text = LocalizationStrings.ModSelect.IncompatibleMods,
                        FontSize = 20,
                        Margin = new MarginPadding { Top = 10, Bottom = 5 },
                        Shadow = true
                    },
                    modList = new ModList
                    {
                        Scale = new Vector2(.8f),
                        Mods = new List<IMod>()
                    }
                }
            };
        }

        public override void SetContent(ModEntry content)
        {
            var mod = content.Mod;

            name.Text = LocalizationStrings.Mods.GetName(mod);
            description.Text = LocalizationStrings.Mods.GetDescription(mod);

            if (mod.IncompatibleMods.Length > 0)
            {
                incompatibleMods.FadeIn();
                modList.FadeIn();
                modList.Mods = mod.IncompatibleMods.Select(ModUtils.Create).ToList();
            }
            else
            {
                incompatibleMods.FadeOut();
                modList.FadeOut();
            }
        }
    }
}
