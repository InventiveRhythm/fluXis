using fluXis.Game.Database.Maps;
using fluXis.Game.Import;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics.Background;

public partial class MapBackground : Sprite
{
    [Resolved(CanBeNull = true)]
    private ImportManager importManager { get; set; }

    public RealmMap Map { get; set; }
    public bool Cropped { get; set; }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        if (Map == null)
            Texture = textures.Get("Backgrounds/default.png");
        else
            Texture = (Cropped ? Map.GetPanelBackground() : Map.GetBackground()) ?? textures.Get("Backgrounds/default.png");
    }
}
