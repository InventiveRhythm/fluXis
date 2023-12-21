using System;
using fluXis.Game.Import;
using JetBrains.Annotations;

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

    public override string ToString() => $"{Name} v{Version} by {Author}";
}
