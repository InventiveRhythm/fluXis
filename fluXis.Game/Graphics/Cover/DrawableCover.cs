using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Import;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;

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
    private void load(BackgroundTextureStore filesStore, TextureStore textures)
    {
        FillMode = FillMode.Fill;

        Texture tex = null;

        if (mapSet.Managed)
        {
            if (importManager == null)
            {
                Logger.Log("ImportManager is null!", LoggingTarget.Runtime, LogLevel.Error);
                return;
            }

            var map = mapSet.Maps.First();
            var store = importManager.GetTextureStore(map.Status);

            if (store != null)
            {
                string path = importManager.GetAsset(map, ImportedAssetType.Cover);
                if (path != null) tex = store.Get(path);
            }
        }
        else tex = filesStore.Get(mapSet.GetFile(mapSet.Cover)?.GetPath() ?? mapSet.GetBackground());

        Texture = tex ?? textures.Get("Backgrounds/default.png");
    }
}
