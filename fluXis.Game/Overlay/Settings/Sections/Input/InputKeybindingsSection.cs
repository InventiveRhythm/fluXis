using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Input;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.Sections.Input;

public partial class InputKeybindingsSection : SettingsSubSection
{
    public override string Title => "Keybindings";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new KeybindSectionTitle { Text = "Navigation" },
            new SettingsKeybind
            {
                Label = "Previous selection",
                Keybinds = new object[] { FluXisGlobalKeybind.Previous }
            },
            new SettingsKeybind
            {
                Label = "Next selection",
                Keybinds = new object[] { FluXisGlobalKeybind.Next }
            },
            new SettingsKeybind
            {
                Label = "Previous group",
                Keybinds = new object[] { FluXisGlobalKeybind.PreviousGroup }
            },
            new SettingsKeybind
            {
                Label = "Next group",
                Keybinds = new object[] { FluXisGlobalKeybind.NextGroup }
            },
            new SettingsKeybind
            {
                Label = "Back",
                Keybinds = new object[] { FluXisGlobalKeybind.Back }
            },
            new SettingsKeybind
            {
                Label = "Select",
                Keybinds = new object[] { FluXisGlobalKeybind.Select }
            },
            new KeybindSectionTitle { Text = "Song Select" },
            new SettingsKeybind
            {
                Label = "Decrease Rate",
                Keybinds = new object[] { FluXisGlobalKeybind.DecreaseRate }
            },
            new SettingsKeybind
            {
                Label = "Increase Rate",
                Keybinds = new object[] { FluXisGlobalKeybind.IncreaseRate }
            },
            new KeybindSectionTitle { Text = "Editing" },
            new SettingsKeybind
            {
                Label = "Delete Selected",
                Keybinds = new object[] { FluXisGlobalKeybind.Delete }
            },
            new SettingsKeybind
            {
                Label = "Undo",
                Keybinds = new object[] { FluXisGlobalKeybind.Undo }
            },
            new SettingsKeybind
            {
                Label = "Redo",
                Keybinds = new object[] { FluXisGlobalKeybind.Redo }
            },
            new KeybindSectionTitle { Text = "Overlays" },
            new SettingsKeybind
            {
                Label = "Toggle Settings",
                Keybinds = new object[] { FluXisGlobalKeybind.ToggleSettings }
            },
            new KeybindSectionTitle { Text = "Audio" },
            new SettingsKeybind
            {
                Label = "Decrease Volume",
                Keybinds = new object[] { FluXisGlobalKeybind.VolumeDecrease }
            },
            new SettingsKeybind
            {
                Label = "Increase Volume",
                Keybinds = new object[] { FluXisGlobalKeybind.VolumeIncrease }
            },
            new SettingsKeybind
            {
                Label = "Previous Volume Category",
                Keybinds = new object[] { FluXisGlobalKeybind.VolumePreviousCategory }
            },
            new SettingsKeybind
            {
                Label = "Next Volume Category",
                Keybinds = new object[] { FluXisGlobalKeybind.VolumeNextCategory }
            },
            new SettingsKeybind
            {
                Label = "Previous Track",
                Keybinds = new object[] { FluXisGlobalKeybind.MusicPrevious }
            },
            new SettingsKeybind
            {
                Label = "Next Track",
                Keybinds = new object[] { FluXisGlobalKeybind.MusicNext }
            },
            new SettingsKeybind
            {
                Label = "Pause/Resume Track",
                Keybinds = new object[] { FluXisGlobalKeybind.MusicPause }
            },
            new KeybindSectionTitle { Text = "Keymodes" },
            new SettingsKeybind
            {
                Label = "1 Key Layout",
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key1k1
                }
            },
            new SettingsKeybind
            {
                Label = "2 Key Layout",
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key2k1,
                    FluXisGameplayKeybind.Key2k2
                }
            },
            new SettingsKeybind
            {
                Label = "3 Key Layout",
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key3k1,
                    FluXisGameplayKeybind.Key3k2,
                    FluXisGameplayKeybind.Key3k3
                }
            },
            new SettingsKeybind
            {
                Label = "4 Key Layout",
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key4k1,
                    FluXisGameplayKeybind.Key4k2,
                    FluXisGameplayKeybind.Key4k3,
                    FluXisGameplayKeybind.Key4k4
                }
            },
            new SettingsKeybind
            {
                Label = "5 Key Layout",
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key5k1,
                    FluXisGameplayKeybind.Key5k2,
                    FluXisGameplayKeybind.Key5k3,
                    FluXisGameplayKeybind.Key5k4,
                    FluXisGameplayKeybind.Key5k5
                }
            },
            new SettingsKeybind
            {
                Label = "6 Key Layout",
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key6k1,
                    FluXisGameplayKeybind.Key6k2,
                    FluXisGameplayKeybind.Key6k3,
                    FluXisGameplayKeybind.Key6k4,
                    FluXisGameplayKeybind.Key6k5,
                    FluXisGameplayKeybind.Key6k6
                }
            },
            new SettingsKeybind
            {
                Label = "7 Key Layout",
                Keybinds = new object[]
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
            new SettingsKeybind
            {
                Label = "8 Key Layout",
                Keybinds = new object[]
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
            new SettingsKeybind
            {
                Label = "9 Key Layout",
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key9k1,
                    FluXisGameplayKeybind.Key9k2,
                    FluXisGameplayKeybind.Key9k3,
                    FluXisGameplayKeybind.Key9k4,
                    FluXisGameplayKeybind.Key9k5,
                    FluXisGameplayKeybind.Key9k6,
                    FluXisGameplayKeybind.Key9k7,
                    FluXisGameplayKeybind.Key9k8,
                    FluXisGameplayKeybind.Key9k9
                }
            },
            new SettingsKeybind
            {
                Label = "10 Key Layout",
                Keybinds = new object[]
                {
                    FluXisGameplayKeybind.Key10k1,
                    FluXisGameplayKeybind.Key10k2,
                    FluXisGameplayKeybind.Key10k3,
                    FluXisGameplayKeybind.Key10k4,
                    FluXisGameplayKeybind.Key10k5,
                    FluXisGameplayKeybind.Key10k6,
                    FluXisGameplayKeybind.Key10k7,
                    FluXisGameplayKeybind.Key10k8,
                    FluXisGameplayKeybind.Key10k9,
                    FluXisGameplayKeybind.Key10k10
                }
            },
            new KeybindSectionTitle { Text = "In-Game" },
            new SettingsKeybind
            {
                Label = "Skip Intro",
                Keybinds = new object[] { FluXisGlobalKeybind.Skip }
            },
            new SettingsKeybind
            {
                Label = "Decrease Scroll Speed",
                Keybinds = new object[] { FluXisGlobalKeybind.ScrollSpeedDecrease }
            },
            new SettingsKeybind
            {
                Label = "Increase Scroll Speed",
                Keybinds = new object[] { FluXisGlobalKeybind.ScrollSpeedIncrease }
            },
            new SettingsKeybind
            {
                Label = "Quick Restart",
                Keybinds = new object[] { FluXisGlobalKeybind.QuickRestart }
            },
            new SettingsKeybind
            {
                Label = "Quick Exit",
                Keybinds = new object[] { FluXisGlobalKeybind.QuickExit }
            },
            new SettingsKeybind
            {
                Label = "Seek Backward",
                Keybinds = new object[] { FluXisGlobalKeybind.SeekBackward }
            },
            new SettingsKeybind
            {
                Label = "Seek Forward",
                Keybinds = new object[] { FluXisGlobalKeybind.SeekForward }
            }
        });
    }

    private partial class KeybindSectionTitle : FluXisSpriteText
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            FontSize = 30;
        }
    }
}
