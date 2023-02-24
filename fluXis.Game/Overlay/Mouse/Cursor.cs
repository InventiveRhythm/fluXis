using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Overlay.Mouse;

public partial class Cursor : Container
{
    private Sprite clickSprite;

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        Add(new Sprite
        {
            Texture = textures.Get("Cursor/cursor.png"),
        });

        Add(clickSprite = new Sprite
        {
            Texture = textures.Get("Cursor/cursorclick.png"),
            Alpha = 0,
        });
    }

    public override void Show()
    {
        clickSprite.FadeIn(200);
    }

    public override void Hide()
    {
        clickSprite.FadeOut(200);
    }
}
