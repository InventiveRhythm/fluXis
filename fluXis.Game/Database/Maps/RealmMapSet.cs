using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Realms;

namespace fluXis.Game.Database.Maps;

public class RealmMapSet : RealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }

    public int OnlineID { get; set; } = -1;
    public IList<RealmMap> Maps { get; } = null!;
    public IList<RealmFile> Files { get; } = null!;

    public RealmMapMetadata Metadata => Maps.FirstOrDefault()?.Metadata ?? new RealmMapMetadata();

    public RealmMapSet([CanBeNull] List<RealmMap> maps = null, [CanBeNull] List<RealmFile> files = null)
    {
        ID = Guid.NewGuid();
        Maps = maps ?? new List<RealmMap>();
        Files = files ?? new List<RealmFile>();
    }

    [UsedImplicitly]
    private RealmMapSet()
    {
    }

    public string GetBackground()
    {
        string background = Metadata.Background;
        string hash = string.Empty;

        Files?.ToList().ForEach(file =>
        {
            if (file.Name == background)
                hash = file.Hash;
        });

        if (string.IsNullOrEmpty(hash)) return string.Empty;

        return hash.Substring(0, 1) + "/" + hash.Substring(1, 1) + "/" + hash;
    }

    public RealmFile GetFile(string name)
    {
        RealmFile file = null;

        foreach (var setFile in Files)
        {
            if (setFile.Name == name)
            {
                file = setFile;
                break;
            }
        }

        return file;
    }
}
