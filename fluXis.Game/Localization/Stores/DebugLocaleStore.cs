using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Localisation;

namespace fluXis.Game.Localization.Stores;

public class DebugLocaleStore : ILocalisationStore
{
    public CultureInfo EffectiveCulture { get; } = CultureInfo.CurrentCulture;

    public string Get(string name) => $"[[{name}]]";
    public Task<string> GetAsync(string name, CancellationToken cancellationToken = new()) => Task.FromResult(Get(name));
    public Stream GetStream(string name) => throw new System.NotImplementedException();
    public IEnumerable<string> GetAvailableResources() => throw new System.NotImplementedException();

    public void Dispose()
    {
    }
}
