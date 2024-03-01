using System;
using System.Globalization;

namespace fluXis.Game.Utils;

public static class StringUtils
{
    public static string ToStringInvariant(this float value, string style = "") => value.ToString(style, CultureInfo.InvariantCulture);
    public static string ToStringInvariant(this double value, string style = "") => value.ToString(style, CultureInfo.InvariantCulture);

    public static int ToIntInvariant(this string value) => int.Parse(value, CultureInfo.InvariantCulture);
    public static float ToFloatInvariant(this string value) => float.Parse(value, CultureInfo.InvariantCulture);
    public static double ToDoubleInvariant(this string value) => double.Parse(value, CultureInfo.InvariantCulture);

    public static string CensorEmail(string mail)
    {
        // we want something like dummy@example.com
        // to be displayed as dum**@exam***.com

        if (string.IsNullOrEmpty(mail)) return string.Empty;

        const int visible_name_length = 3;
        const int visible_domain_length = 4;

        var atSplit = mail.Split('@');
        var domain = atSplit[1];
        var domainSplit = domain.Split('.');

        var name = atSplit[0];
        var domainName = domainSplit[0];
        var tld = domainSplit[^1];

        var nameCensored = name.Substring(0, Math.Min(visible_name_length, name.Length)) + new string('*', Math.Max(0, name.Length - visible_name_length));
        var domainNameCensored = domainName.Substring(0, Math.Min(visible_domain_length, domainName.Length)) + new string('*', Math.Max(0, domainName.Length - visible_domain_length));

        return $"{nameCensored}@{domainNameCensored}.{tld}";
    }

    public static string NumberWithOrderSuffix(int number)
    {
        var numStr = number.ToString();

        if (numStr.EndsWith("1") && !numStr.EndsWith("11"))
            return $"{number}st";

        if (numStr.EndsWith("2") && !numStr.EndsWith("12"))
            return $"{number}nd";

        if (numStr.EndsWith("3") && !numStr.EndsWith("13"))
            return $"{number}rd";

        return $"{number}th";
    }
}
