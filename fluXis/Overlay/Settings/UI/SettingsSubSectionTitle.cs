using fluXis.Graphics.Sprites;
using osu.Framework.Bindables;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.UI;

public partial class SettingsSubSectionTitle : FluXisSpriteText
{
    public Bindable<bool> Visible { get; init; } = new(true);

    public SettingsSubSectionTitle(LocalisableString text)
    {
        Text = text;
        WebFontSize = 20;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Visible.BindValueChanged(e =>
        {
            if (e.NewValue) Show();
            else Hide();
        }, true);
    }
}
