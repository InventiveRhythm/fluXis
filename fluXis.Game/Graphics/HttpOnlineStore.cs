using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.IO.Network;
using osu.Framework.IO.Stores;

namespace fluXis.Game.Graphics;

/// <summary>
/// Basically a normal OnlineStore, but with http support.
/// </summary>
public class HttpOnlineStore : IResourceStore<byte[]>
{
    public byte[] Get(string url)
    {
        if (!url.StartsWith(@"http://", StringComparison.Ordinal))
            return null;

        try
        {
            using WebRequest req = new WebRequest($@"{url}");
            req.AllowInsecureRequests = true;
            req.Perform();
            return req.GetResponseData();
        }
        catch
        {
            return null;
        }
    }

    public Stream GetStream(string url)
    {
        byte[] ret = Get(url);
        return ret == null ? null : new MemoryStream(ret);
    }

    public Task<byte[]> GetAsync(string name, CancellationToken cancellationToken = new()) => throw new NotImplementedException();
    public IEnumerable<string> GetAvailableResources() => new List<string>();

    public void Dispose() => GC.SuppressFinalize(this);
}
