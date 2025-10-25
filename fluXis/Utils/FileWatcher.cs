using System.IO;
using fluXis.Database;
using fluXis.Database.Maps;

namespace fluXis.Utils;

public class FileWatcher : FileSystemWatcher
{
    public FileWatcher(RealmMapSet map, string filter = "*.*", NotifyFilters filters = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName)
        : this(MapFiles.GetFullPath(map.GetPathForFile("")), filter, filters)
    {
    }

    public FileWatcher(string path, string filter = "*.*", NotifyFilters filters = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName)
    {
        Path = path;
        NotifyFilter = filters;
        Filter = filter;
        IncludeSubdirectories = true;
    }

    public void Disable() => EnableRaisingEvents = false;
    public void Enable() => EnableRaisingEvents = true;
}
