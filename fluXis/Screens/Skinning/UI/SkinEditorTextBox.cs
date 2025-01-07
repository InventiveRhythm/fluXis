using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Skinning.UI;

public partial class SkinEditorTextBox : Container
{
    public string Text { get; set; } = string.Empty;
    public string DefaultText { get; set; } = string.Empty;
    public Action OnTextChanged { get; set; } = () => { };

    public FluXisTextBox TextBox { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 30;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Text = Text
            },
            TextBox = new FluXisTextBox
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                RelativeSizeAxes = Axes.Y,
                Width = 150,
                Text = DefaultText,
                OnTextChanged = OnTextChanged
            }
        };
    }
}
