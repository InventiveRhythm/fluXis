using System;
using System.Linq;
using fluXis.Map.Format;
using fluXis.Utils;
using osu.Framework.Platform;
using rhym;

namespace fluXis.Modes;

#nullable enable

public partial class GameModeManager : AssemblyLoader<GameMode>
{
    protected override string StorageFolder => "modes";
    protected override string AssemblyPrefix => "fluXis.Mode";

    public IMapFormat? GetFormat(Storage storage, string ext)
    {
        switch (ext)
        {
            case ".rhym":
                return new RhymMapFormat(storage, this);

            default:
                foreach (var mode in Loaded)
                {
                    var fmt = mode.GetFormat(storage, ext);
                    if (fmt is not null) return fmt;
                }

                return null;
        }
    }

    public GameMode? Find(ResourceLocation location)
    {
        var normalized = new ResourceLocation(location.Namespace, location.Path.Split('/').First());
        return Loaded.FirstOrDefault(x => x.Location == normalized);
    }

    public bool Exists(ResourceLocation location) => Loaded.Any(x => x.Location == location);

    public static Exception FailedToLoadException() => new("Tried to load an unknown game mode!");
}
