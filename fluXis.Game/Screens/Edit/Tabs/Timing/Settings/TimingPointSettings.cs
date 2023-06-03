using System;
using System.Globalization;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Timing.Settings.UI;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.Settings;

public partial class TimingPointSettings : PointSettings<TimingPointInfo>
{
    [Resolved]
    private EditorChangeHandler changeHandler { get; set; }

    private BasicPointSettingsField bpmField;
    private BasicPointSettingsField signatureField;

    public TimingPointSettings(TimingPointInfo point)
        : base(point)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            bpmField = new BasicPointSettingsField
            {
                Label = "BPM",
                Text = Point.BPM.ToStringInvariant(),
                OnTextChanged = saveChanges
            },
            signatureField = new BasicPointSettingsField
            {
                Label = "Signature",
                Text = Point.Signature.ToString(),
                OnTextChanged = saveChanges
            }
        });

        TimeField.OnTextChanged = saveChanges;
    }

    private void saveChanges()
    {
        if (!float.TryParse(TimeField.Text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var time) || time < 0)
        {
            TimeField.TextBox.NotifyError();
            return;
        }

        if (!float.TryParse(bpmField.Text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var bpm) || bpm <= 0)
        {
            bpmField.TextBox.NotifyError();
            return;
        }

        if (!int.TryParse(signatureField.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var signature) || signature <= 0)
        {
            signatureField.TextBox.NotifyError();
            return;
        }

        Point.Time = time;
        Point.BPM = bpm;
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
                new FluXisSpriteText
                {
                    Text = "Signature",
                    FontSize = 32,
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
                new FluXisSpriteText
                {
                    Text = "/4",
                    FontSize = 32,
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
                Child = new FluXisSpriteText
                {
                    Text = c.ToString(),
                    FontSize = CalculatedTextSize
                }
            };

            protected override void OnUserTextAdded(string added) => onChange();
            protected override void OnUserTextRemoved(string removed) => onChange();
        }
    }
}
