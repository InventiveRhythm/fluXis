using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class GraphicsSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.Desktop;
    public override string Title => "Graphics";

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        Add(new SettingsSlider<float>(config.GetBindable<float>(FluXisSetting.BackgroundDim), "Background Dim", "", true));
        Add(new SettingsSlider<float>(config.GetBindable<float>(FluXisSetting.BackgroundBlur), "Background Blur", "", true));
    }
}
