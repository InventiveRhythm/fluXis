using System;
using System.Globalization;
using fluXis.Game.Graphics;
using fluXis.Game.Map.Events;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Effect.EffectEdit;

public partial class FlashEffectEditor : FluXisPopover
{
    public FlashEvent FlashEvent { get; init; }

    [Resolved]
    private EditorClock clock { get; set; }

    private float beatLength => clock.MapInfo.GetTimingPoint(FlashEvent.Time).MsPerBeat;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Y,
            Width = 250,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 5),
            Children = new Drawable[]
            {
                new LabelledTextBox
                {
                    LabelText = "Time",
                    Text = FlashEvent.Time.ToString(CultureInfo.InvariantCulture),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                            FlashEvent.Time = result;
                        else
                            textBox.NotifyError();
                    }
                },
                new BeatsTextBox
                {
                    LabelText = "Duration",
                    Text = (FlashEvent.Duration / beatLength).ToString(CultureInfo.InvariantCulture),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                            FlashEvent.Duration = result * beatLength;
                        else
                            textBox.NotifyError();
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "Start Color",
                    Text = FlashEvent.StartColor.ToHex(),
                    OnTextChanged = textBox =>
                    {
                        try
                        {
                            string text = textBox.Text;
                            if (text.StartsWith("#"))
                                text = text.Remove(0, 1);

                            if (text.Length == 3)
                                text = $"{text[0]}{text[0]}{text[1]}{text[1]}{text[2]}{text[2]}";

                            if (text.Length != 6)
                                throw new Exception();

                            var color = Colour4.FromHex(text);
                            FlashEvent.StartColor = color;
                        }
                        catch
                        {
                            textBox.NotifyError();
                        }
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "Start Opacity",
                    Text = FlashEvent.StartOpacity.ToString(CultureInfo.InvariantCulture),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                        {
                            if (result is >= 0 and <= 1)
                                FlashEvent.StartOpacity = result;
                            else
                                textBox.NotifyError();
                        }
                        else
                            textBox.NotifyError();
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "End Color",
                    Text = FlashEvent.EndColor.ToHex(),
                    OnTextChanged = textBox =>
                    {
                        try
                        {
                            string text = textBox.Text;
                            if (text.StartsWith("#"))
                                text = text.Remove(0, 1);

                            if (text.Length == 3)
                                text = $"{text[0]}{text[0]}{text[1]}{text[1]}{text[2]}{text[2]}";

                            if (text.Length != 6)
                                throw new Exception();

                            var color = Colour4.FromHex(text);
                            FlashEvent.EndColor = color;
                        }
                        catch
                        {
                            textBox.NotifyError();
                        }
                    }
                },
                new LabelledTextBox
                {
                    LabelText = "End Opacity",
                    Text = FlashEvent.EndOpacity.ToString(CultureInfo.InvariantCulture),
                    OnTextChanged = textBox =>
                    {
                        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
                        {
                            if (result is >= 0 and <= 1)
                                FlashEvent.EndOpacity = result;
                            else
                                textBox.NotifyError();
                        }
                        else
                            textBox.NotifyError();
                    }
                }
            }
        };
    }

    private partial class LabelledTextBox : Container
    {
        public string LabelText { get; init; }
        public string Text { get; init; }
        public Action<FluXisTextBox> OnTextChanged { get; init; }

        private FluXisTextBox textBox;

        [BackgroundDependencyLoader]
        private void load()
        {
            Height = 20;
            RelativeSizeAxes = Axes.X;

            Children = new Drawable[]
            {
                new SpriteText
                {
                    Text = LabelText,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Font = FluXisFont.Default()
                },
                textBox = new FluXisTextBox
                {
                    Height = 20,
                    Width = 100,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Text = Text,
                    OnTextChanged = () => OnTextChanged?.Invoke(textBox)
                }
            };
        }
    }

    private partial class BeatsTextBox : Container
    {
        public string LabelText { get; init; }
        public string Text { get; init; }
        public Action<FluXisTextBox> OnTextChanged { get; init; }

        private FluXisTextBox textBox;

        [BackgroundDependencyLoader]
        private void load()
        {
            Height = 20;
            RelativeSizeAxes = Axes.X;

            Children = new Drawable[]
            {
                new SpriteText
                {
                    Text = LabelText,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Font = FluXisFont.Default()
                },
                new GridContainer
                {
                    Width = 100,
                    Height = 20,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    ColumnDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension(GridSizeMode.AutoSize)
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            textBox = new FluXisTextBox
                            {
                                RelativeSizeAxes = Axes.Both,
                                Text = Text,
                                OnTextChanged = () => OnTextChanged?.Invoke(textBox)
                            },
                            new SpriteText
                            {
                                Text = "beat(s)",
                                Anchor = Anchor.BottomRight,
                                Origin = Anchor.BottomRight,
                                Font = FluXisFont.Default(12)
                            }
                        }
                    }
                }
            };
        }
    }
}
