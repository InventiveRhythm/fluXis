using System;
using System.Globalization;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.Settings;

public partial class TimingPointSettings : PointSettings<TimingPointInfo>
{
    [Resolved]
    private EditorChangeHandler changeHandler { get; set; }

    public PointSettingsTextBox BPMTextBox { get; }

    private readonly SignatureTextBoxContainer signatureTextBox;

    public TimingPointSettings(TimingPointInfo point)
        : base(point)
    {
        Add(BPMTextBox = new PointSettingsTextBox("BPM", point.BPM.ToString(CultureInfo.InvariantCulture)));
        Add(signatureTextBox = new SignatureTextBoxContainer(OnTextChanged)
        {
            Text = point.Signature.ToString()
        });
    }

    public override void OnTextChanged()
    {
        if (float.TryParse(BPMTextBox.Text, out var bpm))
            Point.BPM = bpm;

        if (int.TryParse(signatureTextBox.Text, out var signature))
            Point.Signature = signature;

        changeHandler.OnTimingPointChanged();
    }

    private partial class SignatureTextBoxContainer : Container
    {
        private readonly TextBox textBox;

        public string Text
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        public SignatureTextBoxContainer(Action onChange)
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;

            AddRangeInternal(new Drawable[]
            {
                new SpriteText
                {
                    Text = "Signature",
                    Font = FluXisFont.Default(32),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                },
                textBox = new TextBox(onChange)
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = 70,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Margin = new MarginPadding { Right = 35 }
                },
                new SpriteText
                {
                    Text = "/4",
                    Font = FluXisFont.Default(32),
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight
                }
            });
        }

        private partial class TextBox : FluXisTextBox
        {
            protected override Color4 SelectionColour => FluXisColors.Accent2;

            private readonly Action onChange;

            public TextBox(Action onChange)
            {
                this.onChange = onChange;
                RelativeSizeAxes = Axes.Y;
                Anchor = Anchor.CentreRight;
                Origin = Anchor.CentreRight;
                CornerRadius = 5;
                Masking = true;
                BackgroundUnfocused = FluXisColors.Surface;
                BackgroundFocused = FluXisColors.Surface2;
            }

            protected override Drawable GetDrawableCharacter(char c) => new FallingDownContainer
            {
                AutoSizeAxes = Axes.Both,
                Child = new SpriteText
                {
                    Text = c.ToString(),
                    Font = FluXisFont.Default(CalculatedTextSize)
                }
            };

            protected override void OnUserTextAdded(string added) => onChange();
            protected override void OnUserTextRemoved(string removed) => onChange();
        }
    }
}
