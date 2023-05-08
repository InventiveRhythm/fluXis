using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class GameplaySection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.Gamepad;
    public override string Title => "Gameplay";

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        AddRange(new SettingsItem[]
        {
            new SettingsSlider<float>
            {
                Label = "Scroll Speed",
                Bindable = config.GetBindable<float>(FluXisSetting.ScrollSpeed),
            },
            new SettingsToggle
            {
                Label = "Show Early/Late",
                Bindable = config.GetBindable<bool>(FluXisSetting.ShowEarlyLate)
            },
            new SettingsToggle
            {
                Label = "Lane Switch Alerts",
                Bindable = config.GetBindable<bool>(FluXisSetting.LaneSwitchAlerts)
            }
        });
    }
}
