using System;
using System.Collections.Generic;
using fluXis.Import;
using fluXis.Overlay.Settings.UI;
using fluXis.Utils;
using JetBrains.Annotations;
using osu.Framework.Platform;

namespace fluXis.Plugins;

public abstract class Plugin : IFromAssembly
{
    public abstract string Name { get; }
    public abstract string Author { get; }
    public abstract Version Version { get; }

    string IFromAssembly.AssemblyName { get; set; } = string.Empty;
    string IFromAssembly.AssemblyHash { get; set; } = string.Empty;

    private MapImporter importer;

    [CanBeNull]
    public MapImporter Importer => importer ??= CreateImporter();

    [CanBeNull]
    protected virtual MapImporter CreateImporter() => null;

    public virtual void CreateConfig(Storage storage) { }
    public virtual List<SettingsItem> CreateSettings() => new();

    public override string ToString() => $"{Name} v{Version} by {Author}";
}
