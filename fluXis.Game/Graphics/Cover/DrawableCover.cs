using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Graphics.Cover;

public partial class DrawableCover : Sprite
{
    private RealmMapSet mapSet { get; }

    public DrawableCover(RealmMapSet mapSet)
    {
        this.mapSet = mapSet;
    }

    [BackgroundDependencyLoader]
    private void load(BackgroundTextureStore filesStore, TextureStore textures)
    {
        string path = mapSet.GetFile(mapSet.Cover)?.GetPath() ?? mapSet.GetBackground();

        Texture = filesStore.Get(path) ?? textures.Get(@"Covers/default.png");
        FillMode = FillMode.Fill;
    }
}
