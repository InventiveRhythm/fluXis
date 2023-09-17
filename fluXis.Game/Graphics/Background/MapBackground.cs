using fluXis.Game.Database.Maps;
using fluXis.Game.Import;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Graphics.Background;

public partial class MapBackground : Sprite
{
    [Resolved(CanBeNull = true)]
    private ImportManager importManager { get; set; }

    public RealmMap Map { get; set; }
    public bool Cropped { get; set; }

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        if (Map == null)
            Texture = skinManager.GetDefaultBackground();
        else
            Texture = (Cropped ? Map.GetPanelBackground() : Map.GetBackground()) ?? skinManager.GetDefaultBackground();
    }
}
