using System;
using System.Globalization;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Timing;

public partial class PointSettings<T> : PointSettings
    where T : TimedObject
{
    public T Point { get; }

    public PointSettings(T point)
    {
        Point = point;
        TimeTextBox.Text = point.Time.ToString(CultureInfo.InvariantCulture);
    }
}

public partial class PointSettings : FillFlowContainer
{
    public TimingTab Tab { get; set; }

    public PointSettingsTextBox TimeTextBox { get; }

    public PointSettings()
    {
        Direction = FillDirection.Vertical;
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Spacing = new Vector2(0, 10);

        Add(TimeTextBox = new PointSettingsTextBox("Time", ""));
    }

    public virtual void OnTextChanged() { }

    public override void Add(Drawable drawable)
    {
        if (drawable is PointSettingsTextBox textBox)
            textBox.OnTextChanged += OnTextChanged;

        base.Add(drawable);
    }

    public partial class PointSettingsTextBox : Container
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

        public PointSettingsTextBox(string text, string value)
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;

            AddRange(new Drawable[]
            {
                new SpriteText
                {
                    Text = text,
                    Font = FluXisFont.Default(32),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                },
                textBox = new TextBox { Text = value }
            });
        }

        private partial class TextBox : FluXisTextBox
        {
            protected override Color4 SelectionColour => FluXisColors.Accent2;

            public Action OnTextChanged { get; set; } = () => { };

            public TextBox()
            {
                RelativeSizeAxes = Axes.Y;
                Width = 200;
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

            protected override void OnUserTextAdded(string added) => OnTextChanged.Invoke();
            protected override void OnUserTextRemoved(string removed) => OnTextChanged.Invoke();
        }
    }
}
