using System;
using System.Globalization;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsSlider<T> : SettingsItem
    where T : struct, IComparable<T>, IConvertible, IEquatable<T>
{
    public Bindable<T> Bindable { get; init; }
    public string ValueLabel { get; init; } = "{value}";
    public bool DisplayAsPercentage { get; init; } = false;
    public float Step { get; init; } = .01f;

    [BackgroundDependencyLoader]
    private void load()
    {
        SpriteText valueLabel;
        AddRange(new Drawable[]
        {
            new SpriteText
            {
                Text = Label,
                Font = FluXisFont.Default(24),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            valueLabel = new SpriteText
            {
                Text = ValueLabel,
                Font = FluXisFont.Default(24),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Margin = new MarginPadding { Right = 410 }
            },
            new Slider(Bindable, valueLabel, DisplayAsPercentage, Step)
        });
    }

    public partial class Slider : CircularContainer
    {
        public Bindable<T> Bindable { get; }

        private readonly SpriteText valueLabel;
        private readonly string originalText;
        private readonly bool displayAsPercentage;

        private readonly BasicSliderBar<T> sliderBar;

        public Slider(Bindable<T> bindable, SpriteText valueLabel, bool displayAsPercentage, float step)
        {
            Bindable = bindable;
            this.valueLabel = valueLabel;
            this.displayAsPercentage = displayAsPercentage;
            originalText = valueLabel.Text.ToString();

            Height = 20;
            Width = 400;
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            Masking = true;

            Add(sliderBar = new BasicSliderBar<T>
            {
                RelativeSizeAxes = Axes.Both,
                Current = bindable,
                BackgroundColour = FluXisColors.Accent2.Opacity(.2f),
                SelectionColour = FluXisColors.Accent2,
                KeyboardStep = step
            });
        }

        protected override void Update()
        {
            valueLabel.Text = displayAsPercentage
                ? $"{Math.Round(Bindable.Value.ToDouble(null) * 100)}%"
                : originalText.Replace("{value}", Math.Round(Bindable.Value.ToDouble(null), 2).ToString(CultureInfo.InvariantCulture));

            if (Bindable is BindableNumber<T> numBind)
            {
                float percent = (float)(numBind.Value.ToDouble(null) - numBind.MinValue.ToDouble(null)) / (float)(numBind.MaxValue.ToDouble(null) - numBind.MinValue.ToDouble(null));
                Colour4 colour = FluXisColors.AccentGradient.Interpolate(new Vector2(percent, 1));
                sliderBar.SelectionColour = colour;
                sliderBar.BackgroundColour = colour.Opacity(.2f);
            }

            base.Update();
        }
    }
}
