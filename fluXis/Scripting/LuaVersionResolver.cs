using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace fluXis.Scripting;

public interface IVersionedCandidate
{
    [CanBeNull]
    LuaVersionAttribute Version { get; }
}

public static class LuaVersionResolver
{
    [CanBeNull]
    public static T SelectBestMatch<T>(IEnumerable<T> candidates, int version, string label = "Member")
        where T : class, IVersionedCandidate
    {
        var matches = candidates
                      .Where(c => c.Version?.Supports(version) ?? true)
                      .OrderByDescending(c => c.Version?.Since ?? 0)
                      .ToList();

        if (matches.Count > 1 && (matches[0].Version?.Since ?? 0) == (matches[1].Version?.Since ?? 0))
            throw new InvalidOperationException($"'{label}': Ambiguous candidate for version {version}");

        return matches.FirstOrDefault();
    }
}
