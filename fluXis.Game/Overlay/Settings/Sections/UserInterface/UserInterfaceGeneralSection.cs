using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections.UserInterface;

public partial class UserInterfaceGeneralSection : SettingsSubSection
{
    public override string Title => "General";
    public override IconUsage Icon => FontAwesome6.Solid.Gear;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<float>
            {
                Label = "UI Scale",
                Description = "The scale of the UI.",
                Bindable = Config.GetBindable<float>(FluXisSetting.UIScale)
            },
            new SettingsSlider<float>
            {
                Label = "Hold to Confirm Duration",
                Description = "How long to hold a button to confirm an action.",
                Bindable = Config.GetBindable<float>(FluXisSetting.HoldToConfirm)
            },
            new SettingsToggle
            {
                Label = "Skip Warning",
                Description = "Skips the epliepsy warning at the start of the game.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.SkipIntro)
            },
            new SettingsToggle
            {
                Label = "Parallax",
                Description = "Enable the parallax effect on backgrounds and such.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.Parallax)
            }
        });
    }
}
