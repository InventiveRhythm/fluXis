using System.IO;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace fluXis.Import.Quaver.Map;

public class QuaverRealmMap : RealmMap
{
    public string QuaverPath { get; init; } = string.Empty;
    public string FolderPath { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;

    public override MapInfo GetMapInfo()
    {
        var path = Path.Combine(QuaverPath, FolderPath, FileName);
        string yaml = System.IO.File.ReadAllText(path);
        return QuaverImport.ParseFromYaml(yaml).ToMapInfo();
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
