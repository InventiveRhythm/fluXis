using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.Sections.Gameplay;

public partial class GameplayBackgroundSection : SettingsSubSection
{
    public override string Title => "Background";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<float>
            {
                Label = "Background Dim",
                Bindable = Config.GetBindable<float>(FluXisSetting.BackgroundDim),
                DisplayAsPercentage = true
            },
            new SettingsSlider<float>
            {
                Label = "Background Blur",
                Bindable = Config.GetBindable<float>(FluXisSetting.BackgroundBlur),
                DisplayAsPercentage = true
            },
            new SettingsToggle
            {
                Label = "Background Pulse",
                Bindable = Config.GetBindable<bool>(FluXisSetting.BackgroundPulse)
            }
        });
    }
}
