using System;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Effect.EffectEdit;

public partial class LabelledTextBox : Container
{
    public string LabelText { get; init; }
    public string Text { get; init; }
    public Action<FluXisTextBox> OnTextChanged { get; init; }

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
            textBox = new FluXisTextBox
            {
                Height = 30,
                Width = 200,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Text = Text,
                OnTextChanged = () => OnTextChanged?.Invoke(textBox)
            }
        };
    }
}

public partial class BeatsTextBox : Container
{
    public string LabelText { get; init; }
    public string Text { get; init; }
    public Action<FluXisTextBox> OnTextChanged { get; init; }

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
                            OnTextChanged = () => OnTextChanged?.Invoke(textBox)
                        },
                        Empty(),
                        new FluXisSpriteText
                        {
                            Text = "beat(s)",
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            FontSize = 18
                        }
                    }
                }
            }
        };
    }
}
