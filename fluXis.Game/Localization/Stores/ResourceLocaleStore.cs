using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Game.Utils;
using osu.Framework.Bindables;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Stores;

public class ResourceLocaleStore : ILocalisationStore
{
    protected virtual bool ForceFallback => EffectiveCulture.Name == "en";

    public CultureInfo EffectiveCulture { get; }
    private ResourceStore<byte[]> store { get; }
    private Bindable<bool> showMissing { get; }

    private readonly Dictionary<string, Dictionary<string, string>> cache = new();

    public ResourceLocaleStore(string code, ResourceStore<byte[]> store, Bindable<bool> showMissing)
    {
        EffectiveCulture = new CultureInfo(code);
        this.store = store;
        this.showMissing = showMissing;
    }

    public virtual string Get(string name)
    {
        var result = get(name, showMissing.Value);

        if (showMissing.Value)
            result ??= $"[[{name}]]";

        return result;
    }

    private string get(string name, bool skipFallback)
    {
        // If the language is English, use the fallback values,
        // since they are always more up-to-date.
        if (ForceFallback && !skipFallback)
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
