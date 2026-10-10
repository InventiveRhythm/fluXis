using System.Collections.Generic;
using fluXis.Map.Structures.Bases;

namespace fluXis.Map.Format;

#nullable enable

public interface IMapFormat
{
    const string IMPORT_CHART = "chart";
    const string IMPORT_EVENT = "event";
    const string IMPORT_STORYBOARD = "storyboard";

    bool IsChart(string path);
    bool Supports(ITimedObject obj);

    PlayableMap? Parse(string path);
    Dictionary<string, string> Save(PlayableMap map);
}
