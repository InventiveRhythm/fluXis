using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Utils;

#nullable enable

/// <summary>
/// Loads types from accessible assemblies.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class AssemblyLoader<T> : Component
    where T : class, IFromAssembly
{
    protected abstract string StorageFolder { get; }
    protected abstract string AssemblyPrefix { get; }

    protected Storage Storage { get; private set; } = null!;

    public IReadOnlyList<T> Loaded => [.. items];
    private List<T> items { get; } = [];

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        Storage = storage.GetStorageForDirectory(StorageFolder);
        Lookup();

        Logger.Log($"Loaded {items.Count} items of type {typeof(T).Name}.");
    }

    protected virtual void Lookup()
    {
        lookupAppDomain();
        lookupStartupDirectory();
        lookupStorage();
    }

    protected virtual bool ShouldBeConsidered(Assembly assembly) => true;
    protected virtual bool ShouldBeConsidered(Type type) => true;
    protected virtual void SetupType(T type) { }

    protected void LoadSingular(Assembly assembly)
    {
        if (!ShouldBeConsidered(assembly))
            return;

        var name = assembly.GetName().Name ?? "unknown assembly name";

        try
        {
            var location = assembly.Location;

            var raw = File.OpenRead(location);
            var hash = MapUtils.GetHash(raw);
            raw.Dispose();

            var types = assembly.GetTypes()
                                .Where(t => t.IsSubclassOf(typeof(T)))
                                .Where(t => !t.IsAbstract)
                                .Where(ShouldBeConsidered);

            foreach (var t in types)
            {
                var type = Activator.CreateInstance(t) as T;
                if (type is null) return;

                type.AssemblyName = name;
                type.AssemblyHash = hash;
                SetupType(type);

                items.Add(type);
                Logger.Log($"Loaded assembly '{name}' ({t.Name}) as {typeof(T).Name}!");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to load assembly '{name}'!");
        }
    }

    private void lookupAppDomain()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var name = assembly.GetName().Name;
            if (name is null) return;

            if (!name.StartsWith(AssemblyPrefix, StringComparison.InvariantCultureIgnoreCase))
                return;

            LoadSingular(assembly);
        }
    }

    private void lookupStartupDirectory()
    {
        string[] files = Directory.GetFiles(RuntimeInfo.StartupDirectory, $"{AssemblyPrefix}.*.dll");

        foreach (var file in files)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                LoadSingular(assembly);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load plugin {file} from directory from AppDomain!");
            }
        }
    }

    private void lookupStorage()
    {
        var path = Storage.GetFullPath(".");

        if (!Directory.Exists(path))
        {
            Logger.Log($"Directory '{path}' does not exist. Creating...");
            Directory.CreateDirectory(path);
        }

        string[] files = Directory.GetFiles(path, $"{AssemblyPrefix}.*.dll");

        foreach (var file in files)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                LoadSingular(assembly);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load plugin {file} from plugins!");
            }
        }
    }
}

public interface IFromAssembly
{
    string AssemblyName { get; set; }
    string AssemblyHash { get; set; }
}
