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
                Keybinds = new[] { FluXisKeybind.Previous }
            },
            new SettingsKeybind
            {
                Label = "Next selection",
                Keybinds = new[] { FluXisKeybind.Next }
            },
            new SettingsKeybind
            {
                Label = "Previous group",
                Keybinds = new[] { FluXisKeybind.PreviousGroup }
            },
            new SettingsKeybind
            {
                Label = "Next group",
                Keybinds = new[] { FluXisKeybind.NextGroup }
            },
            new SettingsKeybind
            {
                Label = "Back",
                Keybinds = new[] { FluXisKeybind.Back }
            },
            new SettingsKeybind
            {
                Label = "Select",
                Keybinds = new[] { FluXisKeybind.Select }
            },
            new SettingsKeybind
            {
                Label = "Delete Selected",
                Keybinds = new[] { FluXisKeybind.Delete }
            },
            new KeybindSectionTitle { Text = "Overlays" },
            new SettingsKeybind
            {
                Label = "Toggle Settings",
                Keybinds = new[] { FluXisKeybind.ToggleSettings }
            },
            new KeybindSectionTitle { Text = "Audio" },
            new SettingsKeybind
            {
                Label = "Decrease Volume",
                Keybinds = new[] { FluXisKeybind.VolumeDecrease }
            },
            new SettingsKeybind
            {
                Label = "Increase Volume",
                Keybinds = new[] { FluXisKeybind.VolumeIncrease }
            },
            new SettingsKeybind
            {
                Label = "Previous Volume Category",
                Keybinds = new[] { FluXisKeybind.VolumePreviousCategory }
            },
            new SettingsKeybind
            {
                Label = "Next Volume Category",
                Keybinds = new[] { FluXisKeybind.VolumeNextCategory }
            },
            new SettingsKeybind
            {
                Label = "Previous Track",
                Keybinds = new[] { FluXisKeybind.MusicPrevious }
            },
            new SettingsKeybind
            {
                Label = "Next Track",
                Keybinds = new[] { FluXisKeybind.MusicNext }
            },
            new SettingsKeybind
            {
                Label = "Pause/Resume Track",
                Keybinds = new[] { FluXisKeybind.MusicPause }
            },
            new KeybindSectionTitle { Text = "Keymodes" },
            new SettingsKeybind
            {
                Label = "1 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key1k1
                }
            },
            new SettingsKeybind
            {
                Label = "2 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key2k1,
                    FluXisKeybind.Key2k2
                }
            },
            new SettingsKeybind
            {
                Label = "3 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key3k1,
                    FluXisKeybind.Key3k2,
                    FluXisKeybind.Key3k3
                }
            },
            new SettingsKeybind
            {
                Label = "4 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key4k1,
                    FluXisKeybind.Key4k2,
                    FluXisKeybind.Key4k3,
                    FluXisKeybind.Key4k4
                }
            },
            new SettingsKeybind
            {
                Label = "5 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key5k1,
                    FluXisKeybind.Key5k2,
                    FluXisKeybind.Key5k3,
                    FluXisKeybind.Key5k4,
                    FluXisKeybind.Key5k5
                }
            },
            new SettingsKeybind
            {
                Label = "6 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key6k1,
                    FluXisKeybind.Key6k2,
                    FluXisKeybind.Key6k3,
                    FluXisKeybind.Key6k4,
                    FluXisKeybind.Key6k5,
                    FluXisKeybind.Key6k6
                }
            },
            new SettingsKeybind
            {
                Label = "7 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key7k1,
                    FluXisKeybind.Key7k2,
                    FluXisKeybind.Key7k3,
                    FluXisKeybind.Key7k4,
                    FluXisKeybind.Key7k5,
                    FluXisKeybind.Key7k6,
                    FluXisKeybind.Key7k7
                }
            },
            new SettingsKeybind
            {
                Label = "8 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key8k1,
                    FluXisKeybind.Key8k2,
                    FluXisKeybind.Key8k3,
                    FluXisKeybind.Key8k4,
                    FluXisKeybind.Key8k5,
                    FluXisKeybind.Key8k6,
                    FluXisKeybind.Key8k7,
                    FluXisKeybind.Key8k8
                }
            },
            new SettingsKeybind
            {
                Label = "9 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key9k1,
                    FluXisKeybind.Key9k2,
                    FluXisKeybind.Key9k3,
                    FluXisKeybind.Key9k4,
                    FluXisKeybind.Key9k5,
                    FluXisKeybind.Key9k6,
                    FluXisKeybind.Key9k7,
                    FluXisKeybind.Key9k8,
                    FluXisKeybind.Key9k9
                }
            },
            new SettingsKeybind
            {
                Label = "10 Key Layout",
                Keybinds = new[]
                {
                    FluXisKeybind.Key10k1,
                    FluXisKeybind.Key10k2,
                    FluXisKeybind.Key10k3,
                    FluXisKeybind.Key10k4,
                    FluXisKeybind.Key10k5,
                    FluXisKeybind.Key10k6,
                    FluXisKeybind.Key10k7,
                    FluXisKeybind.Key10k8,
                    FluXisKeybind.Key10k9,
                    FluXisKeybind.Key10k10
                }
            },
            new KeybindSectionTitle { Text = "In-Game" },
            new SettingsKeybind
            {
                Label = "Skip Intro",
                Keybinds = new[] { FluXisKeybind.Skip }
            },
            new SettingsKeybind
            {
                Label = "Decrease Scroll Speed",
                Keybinds = new[] { FluXisKeybind.ScrollSpeedDecrease }
            },
            new SettingsKeybind
            {
                Label = "Increase Scroll Speed",
                Keybinds = new[] { FluXisKeybind.ScrollSpeedIncrease }
            },
            new SettingsKeybind
            {
                Label = "Quick Restart",
                Keybinds = new[] { FluXisKeybind.QuickRestart }
            },
            new SettingsKeybind
            {
                Label = "Quick Exit",
                Keybinds = new[] { FluXisKeybind.QuickExit }
            },
            new SettingsKeybind
            {
                Label = "Seek Backward",
                Keybinds = new[] { FluXisKeybind.SeekBackward }
            },
            new SettingsKeybind
            {
                Label = "Seek Forward",
                Keybinds = new[] { FluXisKeybind.SeekForward }
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
