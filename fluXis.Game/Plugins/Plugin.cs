using System;
using System.Collections.Generic;
using fluXis.Game.Import;
using fluXis.Game.Overlay.Settings.UI;
using JetBrains.Annotations;
using osu.Framework.Platform;

namespace fluXis.Game.Plugins;

public abstract class Plugin
{
    public abstract string Name { get; }
    public abstract string Author { get; }
    public abstract Version Version { get; }

    public string AssemblyName { get; internal set; }
    public string Hash { get; internal set; }

    private MapImporter importer;

    [CanBeNull]
    public MapImporter Importer => importer ??= CreateImporter();

    [CanBeNull]
    protected virtual MapImporter CreateImporter() => null;

    public virtual void CreateConfig(Storage storage) { }
    public virtual List<SettingsItem> CreateSettings() => new();

    public override string ToString() => $"{Name} v{Version} by {Author}";
}
