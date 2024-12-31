using fluXis.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Overlay.Settings.UI;

public partial class SettingsToggle : SettingsItem
{
    protected override bool IsDefault => Bindable.IsDefault;

    public Bindable<bool> Bindable { get; init; } = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new FluXisToggleSwitch
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                State = Bindable
            }
        });
    }

    protected override void Reset() => Bindable.SetDefault();
}
