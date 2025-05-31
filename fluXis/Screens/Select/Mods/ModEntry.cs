using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Localization;
using fluXis.Mods;
using fluXis.Mods.Drawables;
using fluXis.Overlay.Mouse;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Mods;

public partial class ModEntry : CompositeDrawable, IHasCustomTooltip<ModEntry>
{
    public ModEntry TooltipContent => this;

    [Resolved]
    private UISamples samples { get; set; }

    private Container content { get; }
    private Box background { get; }
    private HoverLayer hover { get; }
    private FlashLayer flash { get; }

    private Box line { get; }
    private FillFlowContainer flow { get; }
    private SpriteIcon icon { get; }
    private ForcedHeightText name { get; }
    private ForcedHeightText description { get; }
    private FluXisSpriteText scoreMultiplier { get; }

    private IMod mod { get; }
    private ModsOverlay overlay { get; }
    private Colour4 accent { get; }

    private bool selected => overlay.SelectedMods.Any(x => x.GetType() == mod.GetType());

    public ModEntry(IMod mod, ModsOverlay overlay)
    {
        this.mod = mod;
        this.overlay = overlay;
        accent = FluXisColors.GetModTypeColor(mod.Type);

        RelativeSizeAxes = Axes.X;
        Height = 48;
        AlwaysPresent = true;
        Alpha = 0;

        var multiplier = (int)Math.Round((mod.ScoreMultiplier - 1) * 100);
        var multiplierText = multiplier > 0 ? $"+{multiplier}" : multiplier.ToString();

        if (multiplier == 0)
            multiplierText = "\u00b1" + multiplierText;

        InternalChild = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Shear = new Vector2(.2f, 0),
            CornerRadius = 4,
            Masking = true,
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                },
                hover = new HoverLayer { Colour = accent },
                line = new Box
                {
                    Width = 6,
                    RelativeSizeAxes = Axes.Y,
                    Colour = accent
                },
                flow = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Padding = new MarginPadding { Horizontal = 20 },
                    Spacing = new Vector2(12),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Shear = new Vector2(-.2f, 0),
                    Children = new Drawable[]
                    {
                        icon = new FluXisSpriteIcon
                        {
                            Size = new Vector2(20),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Icon = mod.Icon
                        },
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(2),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Children = new Drawable[]
                            {
                                name = new ForcedHeightText
                                {
                                    Height = 12,
                                    Text = LocalizationStrings.Mods.GetName(mod),
                                    TextColor = Colour4.White,
                                    WebFontSize = 16,
                                },
                                description = new ForcedHeightText
                                {
                                    Height = 8,
                                    Text = LocalizationStrings.Mods.GetDescription(mod),
                                    TextColor = Colour4.White,
                                    WebFontSize = 12,
                                    Alpha = .8f
                                }
                            }
                        }
                    }
                },
                scoreMultiplier = new FluXisSpriteText
                {
                    Text = $"{multiplierText}%",
                    WebFontSize = 14,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Shear = new Vector2(-.2f, 0),
                    X = -20
                },
                flash = new FlashLayer { Colour = accent }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        updateSelected();
        FinishTransforms(true);

        overlay.SelectedMods.BindCollectionChanged((_, _) => updateSelected());
    }

    private void updateSelected()
    {
        var sel = selected;
        var color = sel ? FluXisColors.TextDark : accent;

        background.FadeColour(sel ? accent : FluXisColors.Background2);
        line.FadeColour(sel ? accent.Darken(.75f) : accent).ResizeWidthTo(sel ? 12 : 6, 400, Easing.OutQuint);
        flow.MoveToX(sel ? 6 : 0, 400, Easing.OutQuint);
        icon.FadeColour(color);
        name.FadeColour(color);
        description.FadeColour(color);
        scoreMultiplier.FadeColour(color);
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.Show();
        samples.Click();

        if (selected) overlay.Deselect(mod);
        else overlay.Select(mod);

        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    public override void Show()
    {
        this.FadeIn(200);
        content.MoveToX(50).MoveToX(0, 400, Easing.OutQuint);
    }

    public override void Hide()
    {
        this.FadeOut(200);
        content.MoveToX(-50, 400, Easing.OutQuint);
    }

    public ITooltip<ModEntry> GetCustomTooltip() => new ModEntryTooltip();

    private partial class ModEntryTooltip : CustomTooltipContainer<ModEntry>
    {
        private ForcedHeightText type { get; }
        private ForcedHeightText name { get; }
        private ForcedHeightText description { get; }
        private ForcedHeightText incompatibleMods { get; }
        private ModList modList { get; }

        public ModEntryTooltip()
        {
            CornerRadius = 12;

            Child = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(4),
                Spacing = new Vector2(6),
                Children = new Drawable[]
                {
                    type = new ForcedHeightText
                    {
                        Height = 9,
                        WebFontSize = 12,
                        Margin = new MarginPadding { Horizontal = 6, Top = 2 },
                        Colour = FluXisColors.TextDark
                    },
                    new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        CornerRadius = 8,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = FluXisColors.Background2
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding(12),
                                Spacing = new Vector2(2),
                                Children = new Drawable[]
                                {
                                    name = new ForcedHeightText()
                                    {
                                        Height = 15,
                                        WebFontSize = 20
                                    },
                                    description = new ForcedHeightText
                                    {
                                        Height = 10,
                                        WebFontSize = 14,
                                        Alpha = .6f
                                    },
                                    incompatibleMods = new ForcedHeightText
                                    {
                                        Height = 9,
                                        Text = LocalizationStrings.ModSelect.IncompatibleMods,
                                        WebFontSize = 12,
                                        Margin = new MarginPadding { Top = 10, Bottom = 5 },
                                        Shadow = true
                                    },
                                    modList = new ModList
                                    {
                                        Scale = new Vector2(.6f),
                                        Mods = new List<IMod>()
                                    }
                                }
                            }
                        }
                    }
                }
            };

            /*Child = ;*/
        }

        public override void SetContent(ModEntry content)
        {
            var mod = content.mod;

            this.TransformTo(nameof(BackgroundColor), (ColourInfo)FluXisColors.GetModTypeColor(mod.Type), 50);
            type.Text = mod.Type switch
            {
                ModType.Rate => LocalizationStrings.ModSelect.RateSection,
                ModType.DifficultyDecrease => LocalizationStrings.ModSelect.DifficultyDecreaseSection,
                ModType.DifficultyIncrease => LocalizationStrings.ModSelect.DifficultyIncreaseSection,
                ModType.Automation => LocalizationStrings.ModSelect.AutomationSection,
                ModType.Misc => LocalizationStrings.ModSelect.MiscSection,
                ModType.Special => "Special",
                _ => throw new ArgumentOutOfRangeException()
            };
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
