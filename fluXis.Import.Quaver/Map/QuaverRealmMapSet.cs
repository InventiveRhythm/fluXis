using System.Collections.Generic;
using System.IO;
using fluXis.Game.Database.Maps;
using osu.Framework.Graphics.Textures;

namespace fluXis.Import.Quaver.Map;

public class QuaverRealmMapSet : RealmMapSet
{
    public string FolderPath { get; init; } = string.Empty;

    public QuaverRealmMapSet(List<RealmMap> maps)
        : base(maps)
    {
    }

    public override Texture GetCover()
    {
        var backgrounds = Resources?.BackgroundStore;
        return backgrounds?.Get(Path.Combine(FolderPath, Metadata.Background));
    }
}
