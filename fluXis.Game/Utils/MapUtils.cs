using System;
using System.IO;
using fluXis.Game.Map;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Logging;

namespace fluXis.Game.Utils;

public class MapUtils
{
    [CanBeNull]
    public static MapInfo LoadFromPath(string path)
    {
        try
        {
            MapInfo map = JsonConvert.DeserializeObject<MapInfo>(File.ReadAllText(path));
            return map;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load map from path: " + path);
            return null;
        }
    }
}
