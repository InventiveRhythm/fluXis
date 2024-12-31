using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Skinning;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Map.Drawables;

public partial class MapBackground : Sprite, IHasLoadedValue
{
    [CanBeNull]
    private RealmMap map;

    [Resolved]
    private SkinManager skinManager { get; set; }

    [CanBeNull]
    public RealmMap Map
    {
        get => map;
        set
        {
            map = value;

            if (IsLoaded)
                setTexture();
        }
    }

    public bool Loaded { get; private set; }

    private bool cropped { get; }

    public MapBackground([CanBeNull] RealmMap map, bool cropped = false)
    {
        this.map = map;
        this.cropped = cropped;

        FillMode = FillMode.Fill;
    }

    [BackgroundDependencyLoader]
    private void load() => setTexture();

    private void setTexture()
    {
        var custom = cropped ? Map?.GetPanelBackground() : Map?.GetBackground();
        Texture = custom ?? skinManager.GetDefaultBackground();
    }

    public override void Show() => this.FadeInFromZero(400).OnComplete(_ => Loaded = true);
}
