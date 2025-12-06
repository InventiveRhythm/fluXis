using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input;

namespace fluXis.Graphics.Gamepad;

public partial class GamepadIcon : Sprite
{
    [Resolved]
    private TextureStore textures { get; set; }

    private readonly string glyph;

    public GamepadIcon(ButtonGlyph glyph)
    {
        this.glyph = glyph.GetDescription();
    }

    public GamepadIcon(JoystickButton button)
    {
        var gl = button switch
        {
            JoystickButton.Button1 => ButtonGlyph.X,
            JoystickButton.Button2 => ButtonGlyph.A,
            JoystickButton.Button3 => ButtonGlyph.B,
            JoystickButton.Button4 => ButtonGlyph.Y,
            JoystickButton.Button9 => ButtonGlyph.View,
            JoystickButton.Button10 => ButtonGlyph.Menu,
            _ => ButtonGlyph.None
        };

        glyph = gl.GetDescription();
    }

    [BackgroundDependencyLoader]
    private void load() => updateIcon();

    private void updateIcon() => Texture = textures.Get($"ButtonIcons/Xbox/{glyph}");
}
