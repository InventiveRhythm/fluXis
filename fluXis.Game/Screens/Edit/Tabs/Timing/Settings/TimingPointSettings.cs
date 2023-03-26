using System;
using System.Globalization;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.Settings;

public partial class TimingPointSettings : PointSettings<TimingPointInfo>
{
    public PointSettingsTextBox BPMTextBox { get; }

    private readonly SignatureTextBoxContainer signatureTextBox;

    public TimingPointSettings(TimingPointInfo point)
        : base(point)
    {
        Add(BPMTextBox = new PointSettingsTextBox("BPM", point.BPM.ToString(CultureInfo.InvariantCulture)));
        Add(signatureTextBox = new SignatureTextBoxContainer
        {
            Text = point.Signature.ToString(),
            OnTextChanged = OnTextChanged
        });
    }

    public override void OnTextChanged()
    {
        if (float.TryParse(TimeTextBox.Text, out var time))
            Point.Time = time;

        if (float.TryParse(BPMTextBox.Text, out var bpm))
            Point.BPM = bpm;

        if (int.TryParse(signatureTextBox.Text, out var signature))
            Point.Signature = signature;

        Tab.OnTimingPointChanged();
    }

    private partial class SignatureTextBoxContainer : Container
    {
        private readonly TextBox textBox;

        public string Text
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        public Action OnTextChanged
        {
            get => textBox.OnTextChanged;
            set => textBox.OnTextChanged = value;
        }

        public SignatureTextBoxContainer()
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;

            AddRangeInternal(new Drawable[]
            {
                new SpriteText
                {
                    Text = "Signature",
                    Font = new FontUsage("Quicksand", 32, "Bold"),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Y = -2
                },
                textBox = new TextBox
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
                    Font = new FontUsage("Quicksand", 32, "Bold"),
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight
                }
            });
        }

        private partial class TextBox : FluXisTextBox
        {
            protected override Color4 SelectionColour => FluXisColors.Accent2;

            public Action OnTextChanged { get; set; }

            public TextBox()
            {
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
                    Font = new FontUsage("Quicksand", 32, "SemiBold").With(size: CalculatedTextSize)
                }
            };

            protected override void OnUserTextAdded(string added) => OnTextChanged.Invoke();
            protected override void OnUserTextRemoved(string removed) => OnTextChanged.Invoke();
        }
    }
}
