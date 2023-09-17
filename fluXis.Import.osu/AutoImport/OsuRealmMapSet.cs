using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Database.Maps;
using osu.Framework.Graphics.Textures;

namespace fluXis.Import.osu.AutoImport;

public class OsuRealmMapSet : RealmMapSet
{
    public OsuRealmMapSet(List<RealmMap> maps)
        : base(maps)
    {
    }

    public override Texture GetCover()
    {
        if (Maps.FirstOrDefault() is not OsuRealmMap first) return null;

        var backgrounds = Resources?.BackgroundStore;
        return backgrounds?.Get(Path.Combine(first.FolderPath, first.Metadata.Background));
    }
}
