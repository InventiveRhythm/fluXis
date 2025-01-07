using System;
using System.Globalization;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.Tabs.Charting.Effect.UI;

public partial class PercentTextBox : Container
{
    public string LabelText { get; init; }
    public string Text { get; init; }
    public Action<float> OnValueChanged { get; init; }

    private FluXisTextBox textBox;

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 30;
        RelativeSizeAxes = Axes.X;

        Children = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = LabelText,
                FontSize = 30,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            new GridContainer
            {
                Width = 200,
                Height = 30,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                ColumnDimensions = new[]
                {
                    new Dimension(),
                    new Dimension(GridSizeMode.Absolute, 5),
                    new Dimension(GridSizeMode.AutoSize)
                },
                Content = new[]
                {
                    new[]
                    {
                        textBox = new FluXisTextBox
                        {
                            RelativeSizeAxes = Axes.Both,
                            Text = Text,
                            OnTextChanged = onTextChanged
                        },
                        Empty(),
                        new FluXisSpriteText
                        {
                            Text = "%",
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            Margin = new MarginPadding { Left = 3 },
                            FontSize = 30
                        }
                    }
                }
            }
        };
    }

    private void onTextChanged()
    {
        if (float.TryParse(textBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) && result is >= 0 and <= 100)
            OnValueChanged?.Invoke(result / 100f);
        else
            textBox.NotifyError();
    }
}
