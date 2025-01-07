using System.Collections.Generic;

namespace fluXis.Map.Structures.Bases;

public interface IHasLaneMask
{
    List<bool> LaneMask { get; set; }
}

public static class HasLaneMaskExtensions
{
    public static bool ValidFor(this IHasLaneMask mask, int lane)
        => mask.LaneMask.Count < lane || mask.LaneMask[lane - 1];
}
