using System;
using System.IO;
using fluXis.Game.Replays;
using fluXis.Game.Utils;
using JetBrains.Annotations;
using osu.Framework.Platform;

namespace fluXis.Game.IO;

public class ReplayStorage
{
    private Storage storage { get; }

    public ReplayStorage(Storage storage)
    {
        this.storage = storage;
    }

    public bool Exists(Guid id)
    {
        var path = getPath(id);
        return storage.Exists(path);
    }

    [CanBeNull]
    public Replay Get(Guid scoreId)
    {
        var path = getPath(scoreId);

        if (!Exists(scoreId))
            return null;

        var json = File.ReadAllText(storage.GetFullPath(path));
        return json.Deserialize<Replay>();
    }

    public void Save(Replay replay)
    {
    }

    private string getPath(Guid scoreId) => $"{scoreId}.frp";
}
