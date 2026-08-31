using System;
using System.Linq;
using fluXis.Modes.Keys;
using fluXis.Utils;
using JetBrains.Annotations;

namespace fluXis.Modes;

public partial class GameModeManager : AssemblyLoader<GameMode>
{
    protected override string StorageFolder => "modes";
    protected override string AssemblyPrefix => "fluXis.Mode";

    protected override void Lookup()
    {
        Items.Add(new KeysGameMode());
        base.Lookup();
    }

    [CanBeNull]
    public GameMode Find(ResourceLocation location) => Loaded.FirstOrDefault(x => x.Location == location);

    public bool Exists(ResourceLocation location) => Loaded.Any(x => x.Location == location);

    public static Exception FailedToLoadException() => new("Tried to load an unknown game mode!");
}
