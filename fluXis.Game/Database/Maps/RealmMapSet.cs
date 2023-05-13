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
    public string Cover { get; set; } = "cover.png";
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
    public RealmMapSet()
    {
    }

    public string GetBackground()
    {
        RealmFile file = GetFile(Metadata.Background);
        return file == null ? string.Empty : file.GetPath();
    }

    public RealmFile GetFile(string name)
    {
        return Files.FirstOrDefault(setFile => setFile.Name == name);
    }

    public RealmFile GetFileFromHash(string hash)
    {
        return Files.FirstOrDefault(setFile => setFile.Hash == hash);
    }

    public override string ToString()
    {
        return ID.ToString();
    }

    public void SetStatus(int status)
    {
        foreach (RealmMap map in Maps) map.Status = status;
    }
}
