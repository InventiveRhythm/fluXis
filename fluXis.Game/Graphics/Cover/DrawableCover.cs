using fluXis.Game.Database.Maps;
using fluXis.Game.Import;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics.Cover;

public partial class DrawableCover : Sprite
{
    [Resolved]
    private ImportManager importManager { get; set; }

    private RealmMapSet mapSet { get; }

    public DrawableCover(RealmMapSet mapSet)
    {
        this.mapSet = mapSet;
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textures)
    {
        FillMode = FillMode.Fill;
        Texture = mapSet.GetCover() ?? textures.Get("Covers/default.png");
    }
}
