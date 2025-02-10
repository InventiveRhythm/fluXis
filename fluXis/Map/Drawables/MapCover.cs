using fluXis.Database.Maps;
using fluXis.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Map.Drawables;

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
    public float FadeDuration { get; init; } = 400;

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

    public override void Show() => this.FadeInFromZero(FadeDuration).OnComplete(_ => Loaded = true);
}
