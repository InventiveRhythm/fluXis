using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.Tabs.Charting.Effect.UI;

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

