using System;
using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Timing.Settings.UI;

public partial class BasicPointSettingsField : Container
{
    public readonly FluXisTextBox TextBox;
    private readonly SpriteText labelText;

    public string Label { set => labelText.Text = value; }

    public string Text
    {
        get => TextBox.Text;
        set => TextBox.Text = value;
    }

    public Action OnTextChanged
    {
        set
        {
            TextBox.OnTextChanged = value;
            TextBox.OnCommit += (_, _) => value?.Invoke();
        }
    }

    public BasicPointSettingsField()
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;

        Children = new Drawable[]
        {
            labelText = new SpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Font = FluXisFont.Default(32)
            },
            TextBox = new FluXisTextBox
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Width = 200,
                Height = 40
            }
        };
    }
}
