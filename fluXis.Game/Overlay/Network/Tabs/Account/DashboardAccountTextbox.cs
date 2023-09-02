using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Overlay.Network.Tabs.Account;

public partial class DashboardAccountTextbox : Container
{
    public string Title { get; set; }
    public string Value { get; set; }
    public Action<string> OnTextChanged { get; set; }
    public bool ShowSaveButton { get; set; } = false;
    public Action<string> OnSave { get; set; }

    private FluXisTextBox textBox;
    private FluXisButton saveButton;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 32;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreRight,
                X = 140,
                FontSize = 16,
                Text = Title
            },
            textBox = new FluXisTextBox
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                BackgroundInactive = FluXisColors.Background3,
                BackgroundActive = FluXisColors.Background4,
                RelativeSizeAxes = Axes.Y,
                Width = 300,
                X = 150,
                Text = Value,
                OnTextChanged = textChanged
            },
            saveButton = new FluXisButton
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                RelativeSizeAxes = Axes.Y,
                Width = 100,
                X = 460,
                Text = "Save",
                Action = save,
                Alpha = 0
            }
        };
    }

    private void save()
    {
        saveButton.FadeOut(200);
        OnSave?.Invoke(textBox.Text);
        Value = textBox.Text;
    }

    private void textChanged()
    {
        OnTextChanged?.Invoke(textBox.Text);

        if (ShowSaveButton)
        {
            if (textBox.Text == Value)
                saveButton.FadeOut(200);
            else
                saveButton.FadeIn(200);
        }
    }
}
