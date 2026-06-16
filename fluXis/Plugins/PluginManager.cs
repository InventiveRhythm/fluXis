using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Plugins.Capabilities;
using fluXis.Plugins.Providers;
using fluXis.Utils;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Plugins;

public partial class PluginManager : Component
{
    [Resolved]
    private FluXisGame game { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    private Storage pluginStorage;

    private readonly List<Plugin> plugins = new();
    private readonly HashSet<string> loadedHashes = new();

    public IReadOnlyList<Plugin> Plugins => plugins;

    public IEnumerable<T> GetCapabilities<T>() where T : class, IPluginCapability
        => plugins
           .Select(p => p.GetCapability<T>())
           .Where(c => c != null);

    [BackgroundDependencyLoader]
    private void load()
    {
        pluginStorage = storage.GetStorageForDirectory("plugins");

        registerCapabilityProviders();

        loadFromAppDomain();
        loadFromRunFolder();
        loadFromPlugins();

        Logger.Log($"Loaded {plugins.Count} plugins.");
    }

    private void registerCapabilityProviders()
    {
        CapabilityRegistry.Register<ISchedulerProvider>(new GameSchedulerProvider(game));
        CapabilityRegistry.Register<IPanelProvider>(new PanelProvider(panels));

        Logger.Log("Plugin capabilities registered.");
    }

    private void loadSingle(Assembly assembly)
    {
        string name = assembly.GetName().Name ?? assembly.FullName;

        try
        {
            var location = assembly.Location;

            if (string.IsNullOrEmpty(location) || !File.Exists(location))
            {
                Logger.Log($"Couldn't find {name}, Skipping.");
                return;
            }

            string hash;
            using (var fs = File.OpenRead(location))
                hash = MapUtils.GetHash(fs);

            if (!loadedHashes.Add(hash))
            {
                Logger.Log($"Skipping duplicate {name} [{hash[..8]}]");
                return;
            }

            var types = assembly.GetTypes()
                                .Where(t => t.IsSubclassOf(typeof(Plugin)) && !t.IsAbstract);

            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is not Plugin plugin)
                {
                    Logger.Log($"Failed to instantiate plugin {type.Name}!");
                    continue;
                }

                PluginLogger.RegisterPlugin(assembly, plugin.Name);

                var capabilities = type.GetInterfaces()
                                       .Where(i => typeof(IPluginCapability).IsAssignableFrom(i) && i != typeof(IPluginCapability));

                plugin.AssemblyName = name;
                plugin.Hash = hash;

                plugin.CreateConfig(pluginStorage);

                plugin.Modules.ForEach(m =>
                {
                    m.Plugin = plugin;
                });

                plugins.Add(plugin);
                var capabilitiesList = capabilities.ToList();
                string capabilitiesStr = string.Join(", ", capabilitiesList.Select(c => c.Name));
                Logger.Log($"Loaded {plugin} [{name}] capabilities: {(capabilitiesList.Count != 0 ? capabilitiesStr : "None")}");
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to load plugin {name}!");
        }
    }

    private void loadFromAppDomain()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var name = assembly.GetName().Name;

            if (name is null || !name.StartsWith("fluXis", StringComparison.OrdinalIgnoreCase))
                continue;

            loadSingle(assembly);
        }
    }

    private void loadFromRunFolder()
    {
        foreach (var file in Directory.GetFiles(RuntimeInfo.StartupDirectory, "fluXis.*.dll"))
        {
            try
            {
                loadSingle(Assembly.LoadFrom(file));
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load plugin {file} from run folder!");
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
            return;
        }

        foreach (var file in Directory.GetFiles(path, "fluXis.*.dll"))
        {
            try
            {
                loadSingle(Assembly.LoadFrom(file));
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load plugin {file} from plugins!");
            }
        }
    }
}
