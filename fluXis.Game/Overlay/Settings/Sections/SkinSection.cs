using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class SkinSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.PaintBrush;
    public override string Title => "Skin";

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        AddRange(new SettingsItem[]
        {
            new SettingsToggle
            {
                Label = "Hide Flawless Judgement",
                Bindable = config.GetBindable<bool>(FluXisSetting.HideFlawless)
            },
            new SettingsSlider<float>
            {
                Label = "Hit Error Bar Scale",
                Bindable = config.GetBindable<float>(FluXisSetting.HitErrorScale),
                DisplayAsPercentage = true
            }
        });
    }
}
