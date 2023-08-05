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

    [Resolved]
    private TextureStore textures { get; set; }

    public RealmMapSet MapSet
    {
        get => mapSet;
        set
        {
            mapSet = value;
            if (IsLoaded) setTexture();
        }
    }

    private RealmMapSet mapSet { get; set; }

    public DrawableCover(RealmMapSet mapSet)
    {
        MapSet = mapSet;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        FillMode = FillMode.Fill;
        setTexture();
    }

    private void setTexture()
    {
        Texture = mapSet.GetCover() ?? textures.Get("Covers/default.png");
    }
}
