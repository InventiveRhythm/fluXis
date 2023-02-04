using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics.Background;

public partial class MapBackground : Sprite
{
    private readonly RealmMap map;

    public MapBackground(RealmMap map)
    {
        this.map = map;
    }

    [BackgroundDependencyLoader]
    private void load(BackgroundTextureStore backgrounds, TextureStore textures)
    {
        if (map == null)
            return;

        RealmFile background = map.MapSet.GetFile(map.Metadata.Background);
        Texture = backgrounds.Get($"{background?.GetPath()}") ?? textures.Get("Backgrounds/default.png");
    }
}
