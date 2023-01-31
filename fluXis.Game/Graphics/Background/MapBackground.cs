using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics.Background;

public class MapBackground : Sprite
{
    private readonly MapSet mapset;

    public MapBackground(MapSet mapset)
    {
        this.mapset = mapset;
    }

    [BackgroundDependencyLoader]
    private void load(BackgroundTextureStore backgrounds, TextureStore textures)
    {
        Texture = backgrounds.Get($"{mapset.GetBackgroundPath()}") ?? textures.Get("Backgrounds/default.png");
    }
}
