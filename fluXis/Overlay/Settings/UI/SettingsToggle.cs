using fluXis.Graphics.UserInterface;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Overlay.Settings.UI;

public partial class SettingsToggle : SettingsItem
{
    protected override bool IsDefault => Bindable.IsDefault;

    public Bindable<bool> Bindable { get; init; } = new();

    protected override Drawable CreateContent() => new FluXisToggleSwitch
    {
        State = Bindable
    };

    protected override void Reset() => Bindable.SetDefault();
}
