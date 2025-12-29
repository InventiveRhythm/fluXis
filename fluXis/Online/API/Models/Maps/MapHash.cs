using System;

namespace fluXis.Online.API.Models.Maps;

public struct MapHash : IEquatable<MapHash>
{
    public string Chart;
    public string Effect;
    public string Storyboard;

    public readonly bool Equals(MapHash other)
    {
        return (Chart ?? "") == (other.Chart ?? "") &&
               (Effect ?? "") == (other.Effect ?? "") &&
               (Storyboard ?? "") == (other.Storyboard ?? "");
    }

    public readonly bool Equals(string chart, string effect, string storyboard)
    {
        return (Chart ?? "") == (chart ?? "") &&
               (Effect ?? "") == (effect ?? "") &&
               (Storyboard ?? "") == (storyboard ?? "");
    }

    public readonly bool Equals(string chart)
    {
        return (Chart ?? "") == (chart ?? "");
    }

    public override readonly bool Equals(object obj)
    {
        return obj is MapHash other && Equals(other);
    }

    public readonly bool IsEmpty()
    {
        return (Chart ?? "") == string.Empty &&
               (Effect ?? "") == string.Empty &&
               (Storyboard ?? "") == string.Empty;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Chart ?? "", Effect ?? "", Storyboard ?? "");
    }

    public static bool operator ==(MapHash left, MapHash right) => left.Equals(right);

    public static bool operator !=(MapHash left, MapHash right) => !left.Equals(right);
}