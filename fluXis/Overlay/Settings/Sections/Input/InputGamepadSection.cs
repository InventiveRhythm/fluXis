using fluXis.Graphics.Sprites.Icons;
using fluXis.Input;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using fluXis.Overlay.Settings.UI.Keybind;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Input;

public partial class InputGamepadSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Gamepad;
    public override IconUsage Icon => FontAwesome6.Solid.Gamepad;

    private SettingsInputStrings strings => LocalizationStrings.Settings.Input;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsSubSectionTitle(strings.Keymodes),
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.FourKey,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key4k1,
                    FluXisGameplayKeybind.Key4k2,
                    FluXisGameplayKeybind.Key4k3,
                    FluXisGameplayKeybind.Key4k4
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.FiveKey,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key5k1,
                    FluXisGameplayKeybind.Key5k2,
                    FluXisGameplayKeybind.Key5k3,
                    FluXisGameplayKeybind.Key5k4,
                    FluXisGameplayKeybind.Key5k5
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.SixKey,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key6k1,
                    FluXisGameplayKeybind.Key6k2,
                    FluXisGameplayKeybind.Key6k3,
                    FluXisGameplayKeybind.Key6k4,
                    FluXisGameplayKeybind.Key6k5,
                    FluXisGameplayKeybind.Key6k6
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.SevenKey,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key7k1,
                    FluXisGameplayKeybind.Key7k2,
                    FluXisGameplayKeybind.Key7k3,
                    FluXisGameplayKeybind.Key7k4,
                    FluXisGameplayKeybind.Key7k5,
                    FluXisGameplayKeybind.Key7k6,
                    FluXisGameplayKeybind.Key7k7
                }
            },
            new SettingsRealmKeybind<FluXisGameplayKeybind>
            {
                Label = strings.EightKey,
                Keybinds = new[]
                {
                    FluXisGameplayKeybind.Key8k1,
                    FluXisGameplayKeybind.Key8k2,
                    FluXisGameplayKeybind.Key8k3,
                    FluXisGameplayKeybind.Key8k4,
                    FluXisGameplayKeybind.Key8k5,
                    FluXisGameplayKeybind.Key8k6,
                    FluXisGameplayKeybind.Key8k7,
                    FluXisGameplayKeybind.Key8k8
                }
            },
        });
    }
}
