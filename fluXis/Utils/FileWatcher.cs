using System.IO;
using fluXis.Database;
using fluXis.Database.Maps;

namespace fluXis.Utils;

public class FileWatcher : FileSystemWatcher
{
    public NotifyFilters DefaultFilters = NotifyFilters.LastWrite
                                        | NotifyFilters.FileName
                                        | NotifyFilters.DirectoryName;
    
    private void init()
    {
        NotifyFilter = DefaultFilters;
        Filter = "*.*";
    }

    public FileWatcher() : base()
    {
        init();
    }

    public FileWatcher(string path) : base(path)
    {
        init();
    }

    public FileWatcher(string path, string filter) : base(path, filter)
    {
        init();
        Filter = filter;
    }

    public FileWatcher(RealmMapSet map, string filter) : base()
    {
        init();
        Filter = filter;
        ChangePath(map);
    }

    public void ChangePath(string newPath) => Path = newPath;

    public void ChangePath(RealmMapSet map)
    {
        var mapRoot = map.GetPathForFile("");
        ChangePath(MapFiles.GetFullPath(mapRoot));
    }

    public void Disable() => EnableRaisingEvents = false;
    public void Enable() => EnableRaisingEvents = true;
}