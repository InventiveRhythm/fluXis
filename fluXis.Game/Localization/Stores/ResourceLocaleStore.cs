using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Shared.Utils;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Stores;

public class ResourceLocaleStore : ILocalisationStore
{
    protected virtual bool ForceFallback => EffectiveCulture.Name == "en";

    public CultureInfo EffectiveCulture { get; }
    private ResourceStore<byte[]> store { get; }

    private readonly Dictionary<string, Dictionary<string, string>> cache = new();

    public ResourceLocaleStore(string code, ResourceStore<byte[]> store)
    {
        EffectiveCulture = new CultureInfo(code);
        this.store = store;
    }

    public virtual string Get(string name)
    {
        // If the language is English, use the fallback values,
        // since they are always more up-to-date.
        if (ForceFallback)
            return null;

        var split = name.Split(':');

        if (split.Length < 2)
            return null;

        var category = split[0];
        var key = split[1];

        var id = $"{EffectiveCulture.Name}/{category}";

        if (!cache.TryGetValue(id, out var dict))
        {
            using var stream = store.GetStream($"{id}.json");

            if (stream == null)
                return null;

            using var reader = new StreamReader(stream);
            cache[id] = reader.ReadToEnd().Deserialize<Dictionary<string, string>>();
            dict = cache[id];
        }

        if (dict == null)
            return null;

        try
        {
            return dict[key];
        }
        catch (Exception)
        {
            return null;
        }
    }

    public Task<string> GetAsync(string name, CancellationToken cancellationToken = new()) => Task.FromResult(Get(name));
    public Stream GetStream(string name) => throw new NotImplementedException();
    public IEnumerable<string> GetAvailableResources() => throw new NotImplementedException();

    public void Dispose()
    {
    }
}
