using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using osu.Framework.Graphics;

namespace fluXis.Overlay.Settings.UI;

public partial class SettingsButton : SettingsItem
{
    public Action Action { get; init; } = () => { };
    public string ButtonText { get; init; } = string.Empty;

    private FluXisButton button;

    protected override Drawable CreateContent() => button = new FluXisButton
    {
        Text = ButtonText,
        Height = 32,
        Width = 150,
        FontSize = FluXisSpriteText.GetWebFontSize(14),
        Action = Action
    };

    protected override void LoadComplete()
    {
        base.LoadComplete();
        button.EnabledBindable.BindTo(EnabledBindable);
    }

    protected override void Reset()
    {
    }
}
