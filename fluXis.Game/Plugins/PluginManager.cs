using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Plugins;

public partial class PluginManager : Component
{
    [Resolved]
    private Storage storage { get; set; }

    private List<Plugin> plugins { get; } = new();
    public IEnumerable<Plugin> Plugins => plugins.ToImmutableArray();

    [BackgroundDependencyLoader]
    private void load()
    {
        loadFromAppDomain();
        loadFromRunFolder();
        loadFromPlugins();

        Logger.Log($"Loaded {plugins.Count} plugins");
    }

    private void loadSingle(Assembly assembly)
    {
        string name = assembly.GetName().Name;

        try
        {
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Plugin)));

            foreach (var type in types)
            {
                var plugin = (Plugin)Activator.CreateInstance(type);

                if (plugin == null)
                {
                    Logger.Log($"Failed to load plugin {name}");
                    continue;
                }

                plugins.Add(plugin);
                Logger.Log($"Loaded plugin {plugin}");
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to load plugin {name}");
        }
    }

    private void loadFromAppDomain()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var name = assembly.GetName().Name;

            if (name == null)
                continue;

            if (!name.StartsWith("fluXis", StringComparison.InvariantCultureIgnoreCase))
                continue;

            loadSingle(assembly);
        }
    }

    private void loadFromRunFolder()
    {
        string[] files = Directory.GetFiles(RuntimeInfo.StartupDirectory, "fluXis.*.dll");

        foreach (var file in files)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                loadSingle(assembly);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load plugin {file} from directory from AppDomain!");
            }
        }
    }

    private void loadFromPlugins()
    {
        var path = storage.GetFullPath("plugins");

        if (!Directory.Exists(path))
        {
            Logger.Log($"Plugins directory {path} does not exist. Creating...");
            Directory.CreateDirectory(path);
        }

        string[] files = Directory.GetFiles(path, "fluXis.*.dll");

        foreach (var file in files)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                loadSingle(assembly);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load plugin {file} from plugins!");
            }
        }
    }
}
