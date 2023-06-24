using System.Globalization;

namespace fluXis.Game.Utils;

public static class StringUtils
{
    public static string ToStringInvariant(this float value, string style = "") => value.ToString(style, CultureInfo.InvariantCulture);
    public static string ToStringInvariant(this double value, string style = "") => value.ToString(style, CultureInfo.InvariantCulture);
}
