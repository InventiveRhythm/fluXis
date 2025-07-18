using System;
using fluXis.Audio;
using fluXis.Audio.Transforms;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Localization;
using fluXis.Mods;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Select.Mods;

public partial class ModSelectRate : FillFlowContainer
{
    [Resolved]
    private GlobalClock clock { get; set; }

    public ModsOverlay Selector { get; set; }

    public BindableFloat RateBindable { get; private set; }
    private RateMod mod;

    private FluXisSpriteText rateText;
    private FluXisSpriteText multiplierText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RateBindable = new BindableFloat(1f)
        {
            MinValue = 0.5f,
            MaxValue = 2f,
            Precision = 0.05f
        };

        mod = new RateMod();

        var color = Theme.GetModTypeColor(ModType.Rate);

        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(8);
        CornerRadius = 10;
        AlwaysPresent = true;
        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 32,
                CornerRadius = 4,
                Masking = true,
                Shear = new Vector2(.2f, 0),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = color
                    },
                    new Box
                    {
                        Width = 100,
                        RelativeSizeAxes = Axes.Y,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Colour = Theme.Background2,
                        Alpha = .5f
                    },
                    new FluXisSpriteText
                    {
                        Text = LocalizationStrings.ModSelect.RateSection,
                        Colour = Theme.TextDark,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Shear = new Vector2(-.2f, 0),
                        WebFontSize = 16,
                        X = 12
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 96,
                Shear = new Vector2(.2f, 0),
                CornerRadius = 4,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        Colour = Theme.Background3,
                        RelativeSizeAxes = Axes.Both
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(6),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Padding = new MarginPadding(16),
                        Shear = new Vector2(-.2f, 0),
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 22,
                                Padding = new MarginPadding(8),
                                Children = new Drawable[]
                                {
                                    rateText = new FluXisSpriteText
                                    {
                                        Text = "1x",
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Colour = color,
                                        FontSize = 22
                                    },
                                    multiplierText = new FluXisSpriteText
                                    {
                                        Text = "0%",
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight,
                                        Colour = color,
                                        FontSize = 22
                                    }
                                }
                            },
                            new FluXisSlider<float>
                            {
                                Bindable = RateBindable,
                                RelativeSizeAxes = Axes.X,
                                Step = RateBindable.Precision,
                                CustomColor = color
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 22,
                                Padding = new MarginPadding { Horizontal = 20 },
                                Colour = color,
                                Children = new SliderTickMark[]
                                {
                                    new() { Value = 0.5f },
                                    new() { Value = 1 },
                                    new() { Value = 2 }
                                }
                            }
                        }
                    },
                }
            }
        };

        RateBindable.BindValueChanged(e =>
        {
            var rate = e.NewValue;

            if (rate == 1f && Selector.SelectedMods.Contains(mod))
                Selector.Deselect(mod);
            else if (rate != 1f && !Selector.SelectedMods.Contains(mod))
                Selector.Select(mod);

            mod.Rate = rate;
            clock.RateTo(rate, 400, Easing.Out);
            Selector.UpdateTotalMultiplier();
            Selector.RateChanged?.Invoke(rate);

            rateText.Text = $"{Math.Round(rate, 2).ToStringInvariant()}x";

            int multiplier = (int)Math.Round(mod.ScoreMultiplier * 100) - 100;
            multiplierText.Text = $"{(multiplier > 0 ? "+" : string.Empty)}{multiplier}%";
        });
    }

    public override void Show()
    {
        this.MoveToX(50).FadeOut()
            .Delay(ModsOverlay.STAGGER_DURATION)
            .FadeIn(200).MoveToX(0, 400, Easing.OutQuint);
    }

    public override void Hide()
    {
        this.Delay(ModsOverlay.STAGGER_DURATION)
            .FadeOut(200).MoveToX(-50, 400, Easing.OutQuint);
    }

    private partial class SliderTickMark : Container
    {
        public float Value { get; init; }

        [BackgroundDependencyLoader]
        private void load()
        {
            AutoSizeAxes = Axes.Both;
            RelativePositionAxes = Axes.Both;
            X = (Value - 0.5f) / 1.5f;
            Origin = Anchor.TopCentre;

            InternalChildren = new Drawable[]
            {
                new CircularContainer
                {
                    Size = new Vector2(3),
                    Masking = true,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Child = new Box { RelativeSizeAxes = Axes.Both }
                },
                new FluXisSpriteText
                {
                    Text = $"{Value.ToStringInvariant()}x",
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    FontSize = 16,
                    Margin = new MarginPadding { Top = 5 }
                }
            };
        }
    }
}
