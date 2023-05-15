using fluXis.Game.Input;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class InputSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.Keyboard;
    public override string Title => "Input";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsKeybind
            {
                Label = "Gameplay Layout (4 Keys)",
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
                Label = "Gameplay Layout (5 Keys)",
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
                Label = "Gameplay Layout (6 Keys)",
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
                Label = "Gameplay Layout (7 Keys)",
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
                Label = "Quick Restart",
                Keybinds = new[] { FluXisKeybind.Restart }
            },
            new SettingsKeybind
            {
                Label = "Quick Exit",
                Keybinds = new[] { FluXisKeybind.Exit }
            }
        });
    }
}
