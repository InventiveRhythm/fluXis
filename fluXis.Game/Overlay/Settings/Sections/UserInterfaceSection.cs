using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class UserInterfaceSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.LayerGroup;
    public override string Title => "UI";

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        AddRange(new SettingsItem[]
        {
            new SettingsToggle
            {
                Label = "Main Menu Visualizer",
                Bindable = config.GetBindable<bool>(FluXisSetting.MainMenuVisualizer)
            },
            new SettingsToggle
            {
                Label = "Skip Intro",
                Bindable = config.GetBindable<bool>(FluXisSetting.SkipIntro)
            }
        });
    }
}
