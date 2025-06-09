using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input;
using osu.Framework.Logging;

namespace fluXis.Graphics.Gamepad;

public partial class GamepadIcon : Sprite
{
    [Resolved]
    private TextureStore textures { get; set; }

    public string ButtonName
    {
        get => buttonName;
        set
        {
            buttonName = value;
            if (textures != null) updateIcon();
        }
    }

    public JoystickButton Button
    {
        set
        {
            ButtonName = value switch
            {
                JoystickButton.Button1 => "X",
                JoystickButton.Button2 => "A",
                JoystickButton.Button3 => "B",
                JoystickButton.Button4 => "Y",
                JoystickButton.Button9 => "View",
                _ => ""
            };

            if (string.IsNullOrEmpty(ButtonName))
                Logger.Log($"missing button name for {value}");
        }
    }

    private string buttonName;

    [BackgroundDependencyLoader]
    private void load() => updateIcon();

    private void updateIcon() => Texture = textures.Get($"ButtonIcons/Xbox/{buttonName}");
}
