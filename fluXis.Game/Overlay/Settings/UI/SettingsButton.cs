using System;
using fluXis.Game.Graphics.UserInterface.Buttons;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsButton : SettingsItem
{
    public Action Action { get; init; } = () => { };
    public string ButtonText { get; init; } = string.Empty;

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(new FluXisButton
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

    public override void Reset()
    {
    }
}
