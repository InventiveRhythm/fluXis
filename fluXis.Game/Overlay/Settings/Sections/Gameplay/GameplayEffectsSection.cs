using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.Sections.Gameplay;

public partial class GameplayEffectsSection : SettingsSubSection
{
    public override string Title => "Effects";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = "Lane Switch Alerts",
                Bindable = Config.GetBindable<bool>(FluXisSetting.LaneSwitchAlerts)
            },
            new SettingsToggle
            {
                Label = "Disable Epilepsy Intrusing Effects",
                Bindable = Config.GetBindable<bool>(FluXisSetting.DisableEpilepsyIntrusingEffects)
            }
        });
    }
}
