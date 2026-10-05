using System.Collections.Generic;

namespace fluXis.Map.Format;

#nullable enable

public interface IMapFormat
{
    const string IMPORT_CHART = "chart";
    const string IMPORT_EVENT = "event";
    const string IMPORT_STORYBOARD = "storyboard";

    bool IsChart(string path);

    PlayableMap? Parse(string path);
    Dictionary<string, string> Save(PlayableMap map);
}
