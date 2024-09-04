using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Map.Drawables;

public partial class MapCover : Sprite, IHasLoadedValue
{
    [Resolved]
    private TextureStore textures { get; set; }

    public RealmMapSet MapSet
    {
        get => mapSet;
        set
        {
            mapSet = value;

            if (IsLoaded)
                setTexture();
        }
    }

    public bool Loaded { get; private set; }

    private RealmMapSet mapSet { get; set; }

    public MapCover(RealmMapSet mapSet)
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
        Texture = MapSet?.GetCover() ?? textures.Get("Covers/default.png");
    }

    public override void Show() => this.FadeInFromZero(400).OnComplete(_ => Loaded = true);
}
