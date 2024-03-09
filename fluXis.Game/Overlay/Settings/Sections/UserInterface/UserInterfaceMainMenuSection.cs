using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.UserInterface;

public partial class UserInterfaceMainMenuSection : SettingsSubSection
{
    public override LocalisableString Title => strings.MainMenu;
    public override IconUsage Icon => FontAwesome6.Solid.House;

    private SettingsUIStrings strings => LocalizationStrings.Settings.UI;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = strings.IntroMusic,
                Description = strings.IntroMusicDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.IntroTheme)
            },
            new SettingsToggle
            {
                Label = strings.ForceSnow,
                Description = strings.ForceSnowDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.ForceSnow)
            },
            new SettingsToggle
            {
                Label = strings.BubbleVisualizer,
                Description = strings.BubbleVisualizerDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.MainMenuVisualizer)
            },
            new SettingsToggle
            {
                Label = strings.BubbleSway,
                Description = strings.BubbleSwayDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.MainMenuVisualizerSway)
            },
        });
    }
}
