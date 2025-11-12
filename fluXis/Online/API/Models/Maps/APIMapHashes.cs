using System.Collections.Generic;
using System.Linq;

namespace fluXis.Online.API.Models.Maps;

public class APIMapHashes : List<string>
{
    public List<MapHash> SplitHashes() =>
        this.Select(hash =>
        {
            var parts = hash.Split('|');
            return new MapHash
            {
                Chart = parts.Length > 0 ? parts[0] : string.Empty,
                Effect = parts.Length > 1 ? parts[1] : string.Empty,
                Storyboard = parts.Length > 2 ? parts[2] : string.Empty
            };
        }).ToList();
}