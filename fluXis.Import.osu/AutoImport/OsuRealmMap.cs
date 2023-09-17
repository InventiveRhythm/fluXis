using System.IO;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace fluXis.Import.osu.AutoImport;

public class OsuRealmMap : RealmMap
{
    public string OsuPath { get; init; } = string.Empty;
    public string FolderPath { get; init; } = string.Empty;

    public override MapInfo GetMapInfo()
    {
        var path = Path.Combine(OsuPath, FolderPath, FileName);
        string osu = File.ReadAllText(path);
        return OsuImport.ParseOsuMap(osu).ToMapInfo();
    }

    public override Texture GetBackground()
    {
        var backgrounds = MapSet.Resources?.BackgroundStore;
        return backgrounds?.Get(Path.Combine(FolderPath, Metadata.Background));
    }

    public override Texture GetPanelBackground()
    {
        var croppedBackgrounds = MapSet.Resources?.CroppedBackgroundStore;
        return croppedBackgrounds?.Get(Path.Combine(FolderPath, Metadata.Background));
    }

    public override Track GetTrack()
    {
        var tracks = MapSet.Resources?.TrackStore;
        return tracks?.Get(Path.Combine(FolderPath, Metadata.Audio));
    }
}
