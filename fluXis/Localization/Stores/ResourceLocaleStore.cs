using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Utils;
using osu.Framework.Bindables;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;
using osu.Framework.Logging;

namespace fluXis.Localization.Stores;

public class ResourceLocaleStore : ILocalisationStore
{
    protected virtual bool ForceFallback => EffectiveCulture.Name == "en";

    public CultureInfo EffectiveCulture { get; }
    private Bindable<bool> showMissing { get; }

    private readonly Dictionary<string, Dictionary<string, string>> cache = new();

    public ResourceLocaleStore(string code, ResourceStore<byte[]> store, Bindable<bool> showMissing)
    {
        EffectiveCulture = new CultureInfo(code);
        this.showMissing = showMissing;

        foreach (var resource in store.GetAvailableResources())
        {
            try
            {
                if (!resource.StartsWith(code, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var name = Path.GetFileNameWithoutExtension(resource);

                using var stream = store.GetStream(resource);
                if (stream == null) continue;

                using var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();
                cache[name] = content.Deserialize<Dictionary<string, string>>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Failed to load '{resource}' for locale {code}!");
            }
        }

        Logger.Log($"Locale '{code}': loaded {cache.Count} files with {cache.Sum(x => x.Value.Count)} keys.", LoggingTarget.Runtime, LogLevel.Debug);
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

        if (!cache.TryGetValue(category, out var dict))
            return null;

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

    public float CompareTo(ResourceLocaleStore other)
    {
        var total = 0;
        var existing = 0;

        foreach (var (catKey, catValue) in cache)
        {
            total += catValue.Count;

            if (!other.cache.TryGetValue(catKey, out var otherCat))
                continue;

            foreach (var (stringKey, _) in catValue)
            {
                if (!otherCat.TryGetValue(stringKey, out var otherVal))
                    continue;

                if (!string.IsNullOrWhiteSpace(otherVal))
                    existing++;
            }
        }

        return existing / (float)total;
    }

    public Task<string> GetAsync(string name, CancellationToken cancellationToken = new()) => Task.FromResult(Get(name));
    public Stream GetStream(string name) => throw new NotImplementedException();
    public IEnumerable<string> GetAvailableResources() => throw new NotImplementedException();

    public void Dispose()
    {
    }
}
