using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map;
using JetBrains.Annotations;
using osu.Framework.Graphics.Textures;
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

    [Ignored]
    public RealmMapMetadata Metadata => Maps.FirstOrDefault()?.Metadata ?? new RealmMapMetadata();

    [Ignored]
    public bool Managed { get; set; }

    [Ignored]
    [CanBeNull]
    public MapResourceProvider Resources { get; set; }

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

    public virtual Texture GetCover()
    {
        var backgrounds = Resources?.BackgroundStore;
        if (backgrounds == null) return null;

        var coverFile = GetFile(Cover);

        if (coverFile != null)
        {
            var texture = backgrounds.Get(coverFile.Path);
            if (texture != null) return texture;
        }

        var backgroundFile = GetFile(Metadata.Background);
        return backgroundFile == null ? null : backgrounds.Get(backgroundFile.Path);
    }

    public RealmFile GetFile(string name) => Files.FirstOrDefault(setFile => setFile.Name == name);
    public RealmFile GetFileFromHash(string hash) => Files.FirstOrDefault(setFile => setFile.Hash == hash);
    public override string ToString() => ID.ToString();

    public void SetStatus(int status)
    {
        foreach (RealmMap map in Maps) map.Status = status;
    }
}
