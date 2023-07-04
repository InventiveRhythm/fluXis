using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background.Cropped;
using fluXis.Game.Import;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;

namespace fluXis.Game.Graphics.Background;

public partial class MapBackground : Sprite
{
    [Resolved(CanBeNull = true)]
    private ImportManager importManager { get; set; }

    public readonly RealmMap Map;
    public bool Cropped { get; set; }

    public MapBackground(RealmMap map)
    {
        Map = map;
    }

    [BackgroundDependencyLoader]
    private void load(BackgroundTextureStore backgrounds, CroppedBackgroundStore croppedBackgrounds, TextureStore textures)
    {
        if (Map == null)
            return;

        Texture tex = null;

        if (Map.MapSet.Managed)
        {
            if (importManager == null)
            {
                Logger.Log("ImportManager is null!", LoggingTarget.Runtime, LogLevel.Error);
                return;
            }

            var store = importManager.GetTextureStore(Map.Status);

            if (store != null)
            {
                string path = importManager.GetAsset(Map, ImportedAssetType.Background);
                if (path != null) tex = store.Get(path);
            }
        }
        else
        {
            var path = Map.MapSet.GetFile(Map.Metadata.Background)?.GetPath();
            tex = Cropped ? croppedBackgrounds.Get(path) : backgrounds.Get(path);
        }

        Texture = tex ?? textures.Get("Backgrounds/default.png");
    }
}
