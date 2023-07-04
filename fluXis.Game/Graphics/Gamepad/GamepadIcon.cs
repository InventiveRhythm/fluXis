using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics.Gamepad;

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

    private string buttonName;

    [BackgroundDependencyLoader]
    private void load() => updateIcon();

    private void updateIcon() => Texture = textures.Get($"ButtonIcons/Xbox/{buttonName}");
}
