using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections.UserInterface;

public partial class UserInterfaceMainMenuSection : SettingsSubSection
{
    public override string Title => "Main Menu";
    public override IconUsage Icon => FontAwesome.Solid.Home;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = "Bubble Visualizer",
                Description = "Enable the bubble visualizer on the main menu.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.MainMenuVisualizer)
            },
            new SettingsToggle
            {
                Label = "Bubble Sway",
                Description = "Moves the bubbles in a sine wave pattern.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.MainMenuVisualizerSway)
            },
        });
    }
}
