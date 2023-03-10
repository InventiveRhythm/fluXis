using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class GameplaySection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.Gamepad;
    public override string Title => "Gameplay";

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        Add(new SettingsSlider<float>(config.GetBindable<float>(FluXisSetting.ScrollSpeed), "Scroll Speed"));
        Add(new SettingsToggle(config.GetBindable<bool>(FluXisSetting.HideFlawless), "Hide Flawless Judgement"));
        Add(new SettingsToggle(config.GetBindable<bool>(FluXisSetting.ShowEarlyLate), "Show Early/Late"));
        Add(new SettingsToggle(config.GetBindable<bool>(FluXisSetting.BackgroundPulse), "Background Pulse"));
        Add(new SettingsSlider<float>(config.GetBindable<float>(FluXisSetting.HitErrorScale), "Hit Error Bar Scale", "", true));
    }
}
