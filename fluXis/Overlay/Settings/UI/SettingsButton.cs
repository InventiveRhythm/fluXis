using System;
using fluXis.Graphics.UserInterface.Buttons;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Overlay.Settings.UI;

public partial class SettingsButton : SettingsItem
{
    public Action Action { get; init; } = () => { };
    public string ButtonText { get; init; } = string.Empty;

    private FluXisButton button;

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(button = new FluXisButton
        {
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            Height = 30,
            Width = 150,
            FontSize = 20,
            Text = ButtonText,
            Action = Action
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        button.EnabledBindable.BindTo(EnabledBindable);
    }

    protected override void Reset()
    {
    }
}
