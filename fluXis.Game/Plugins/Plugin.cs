using System;
using fluXis.Game.Import;
using JetBrains.Annotations;
using osu.Framework.Platform;

namespace fluXis.Game.Plugins;

public abstract class Plugin
{
    public abstract string Name { get; }
    public abstract string Author { get; }
    public abstract Version Version { get; }

    private MapImporter importer;

    [CanBeNull]
    public MapImporter Importer => importer ??= CreateImporter();

    [CanBeNull]
    protected virtual MapImporter CreateImporter() => null;

    public virtual void CreateConfig(Storage storage) { }

    public override string ToString() => $"{Name} v{Version} by {Author}";
}
