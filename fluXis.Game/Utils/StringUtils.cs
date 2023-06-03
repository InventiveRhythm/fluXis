using System.Globalization;

namespace fluXis.Game.Utils;

public static class StringUtils
{
    public static string ToStringInvariant(this float value) => value.ToString(CultureInfo.InvariantCulture);
    public static string ToStringInvariant(this double value) => value.ToString(CultureInfo.InvariantCulture);
}
