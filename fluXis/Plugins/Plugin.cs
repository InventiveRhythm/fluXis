using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Overlay.Settings.UI;
using fluXis.Plugins.Capabilities;
using osu.Framework.Platform;

namespace fluXis.Plugins;

/// <summary>
/// Base class for all fluXis plugins. A plugin provides metadata and inherits a set of
/// <see cref="IPluginCapability"/>.
/// </summary>
/// <remarks>
/// To create a plugin, inherit this and override the required metadata properties.
/// Capabilities are declared by extending using <see cref="IPluginCapability"/>s (such as <see cref="IMapImporterCapability"/>) or by adding modules in <see cref="CreateModules"/>
/// that extend using <see cref="IPluginCapability"/>s.
/// <code>
/// public class OsuPlugin : Plugin
/// {
///     public override string Name => "MyPlugin";
///     public override string Author => "Developer";
///     public override Version Version => new("1.0.0");
///
///     protected override IReadOnlyList&lt;IPluginModule&gt; CreateModules() => new IPluginModule[]
///     {
///         new ImporterModule(),
///         new EditorModule()
///     };
/// }
/// </code>
/// </remarks>
public abstract class Plugin
{
    public abstract string Name { get; }
    public abstract string Author { get; }
    public abstract Version Version { get; }

    public virtual PluginType Type => PluginType.Misc; /* some older 3rd-porty plugins don't implement this so
    we need to make it virtual instead of abstract to have a default value */

    public string AssemblyName { get; internal set; }
    public string Hash { get; internal set; }

    private IReadOnlyList<IPluginModule> modules;
    public IReadOnlyList<IPluginModule> Modules => modules ??= CreateModules();

    /// <summary>
    /// Gets the first capability of <typeparamref name="T"/> with <see cref="Plugin"/> getting higher priority than <see cref="IPluginModule"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetCapability<T>() where T : class, IPluginCapability
        => this is T self ? self : Modules.OfType<T>().FirstOrDefault();

    /// <summary>
    /// Gets the first capability of <typeparamref name="T"/> that satisfies a condition with <see cref="Plugin"/> getting higher priority than <see cref="IPluginModule"/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetCapability<T>(Func<T, bool> predicate) where T : class, IPluginCapability
        => this is T self && predicate(self) ? self : Modules.OfType<T>().FirstOrDefault(predicate);

    /// <summary>
    /// Gets all capabilities of <typeparamref name="T"/> including the ones in <see cref="IPluginModule"/>s
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IEnumerable<T> GetCapabilities<T>() where T : class, IPluginCapability
        => this is T self ? Modules.OfType<T>().Prepend(self) : Modules.OfType<T>();

    /// <summary>
    /// Checks if <typeparamref name="T"/> exists including the ones in <see cref="IPluginModule"/>s
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool HasCapability<T>() where T : class, IPluginCapability
        => this is T || Modules.OfType<T>().Any();

    protected virtual IReadOnlyList<IPluginModule> CreateModules() => [];

    public virtual void CreateConfig(Storage storage) { }
    public virtual List<SettingsItem> CreateSettings() => new();

    public override string ToString() => $"{Name} v{Version} by {Author}";
}
