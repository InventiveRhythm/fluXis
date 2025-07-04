using fluXis.Configuration;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.UserInterface;

public partial class UserInterfaceGeneralSection : SettingsSubSection
{
    public override LocalisableString Title => strings.General;
    public override IconUsage Icon => FontAwesome6.Solid.Gear;

    private SettingsUIStrings strings => LocalizationStrings.Settings.UI;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<float>
            {
                Label = strings.UIScale,
                Description = strings.UIScaleDescription,
                Bindable = Config.GetBindable<float>(FluXisSetting.UIScale)
            },
            new SettingsSlider<float>
            {
                Label = strings.ConfirmDuration,
                Description = strings.ConfirmDurationDescription,
                Bindable = Config.GetBindable<float>(FluXisSetting.HoldToConfirm)
            },
            new SettingsToggle
            {
                Label = strings.SkipWarning,
                Description = strings.SkipWarningDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.SkipIntro)
            },
            new SettingsToggle
            {
                Label = strings.Parallax,
                Description = strings.ParallaxDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.Parallax)
            },
            new SettingsToggle
            {
                Label = strings.ShowStoryboardVideo,
                Description = strings.ShowStoryboardVideoDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.ShowStoryboardVideo)
            }
        });
    }
}
