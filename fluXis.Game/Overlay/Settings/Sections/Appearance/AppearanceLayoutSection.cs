using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.Sections.Appearance;

public partial class AppearanceLayoutSection : SettingsSubSection
{
    public override string Title => "HUD Layout";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<float>
            {
                Label = "Hit Error Bar Scale",
                Bindable = Config.GetBindable<float>(FluXisSetting.HitErrorScale),
                DisplayAsPercentage = true
            },
            new SettingsButton
            {
                Label = "Open Layout editor",
                Enabled = false,
                ButtonText = "Open"
            }
        });
    }
}
