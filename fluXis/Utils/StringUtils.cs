using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace fluXis.Utils;

public static class StringUtils
{
    private static readonly string[] size_suffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

#pragma warning disable RS0030
    public static string ToStringInvariant(this float value, string style = "") => value.ToString(style, CultureInfo.InvariantCulture);
    public static string ToStringInvariant(this double value, string style = "") => value.ToString(style, CultureInfo.InvariantCulture);
#pragma warning restore RS0030

    public static int ToIntInvariant(this string value) => int.Parse(value, CultureInfo.InvariantCulture);
    public static bool TryParseIntInvariant(this string value, out int result) => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);

    public static float ToFloatInvariant(this string value) => float.Parse(value, CultureInfo.InvariantCulture);
    public static bool TryParseFloatInvariant(this string value, out float result) => float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);

    public static double ToDoubleInvariant(this string value) => double.Parse(value, CultureInfo.InvariantCulture);
    public static bool TryParseDoubleInvariant(this string value, out double result) => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);

    public static bool EqualsLower(this string first, string second) => first.Equals(second, StringComparison.InvariantCultureIgnoreCase);

    public static string FormatSteamProfile(ulong id) => $"https://steamcommunity.com/profiles/{id}";

    public static string FormatBytes(long bytes)
    {
        if (bytes < 0) return $"-{FormatBytes(-bytes)}";
        if (bytes == 0) return "0 bytes";

        var m = (int)Math.Log(bytes, 1024);
        var adjust = (decimal)bytes / (1L << (m * 10));

        if (Math.Round(adjust, 2) >= 1000)
        {
            m += 1;
            adjust /= 1024;
        }

        return $"{adjust:0.0}{size_suffixes[m]}";
    }

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

    public static bool IsUpperCase(this string str) => str.All(char.IsUpper);

    public static string NumberWithOrderSuffix(this int number) => NumberWithOrderSuffix((long)number);

    public static string NumberWithOrderSuffix(this long number)
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

    public static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        string[] split = str.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length == 0)
            return string.Empty;

        StringBuilder result = new StringBuilder();

        result.Append(char.ToLower(split[0][0]));
        result.Append(split[0].AsSpan(1));

        for (int i = 1; i < split.Length; i++)
        {
            if (split[i].Length > 0)
            {
                result.Append(char.ToUpper(split[i][0]));
                result.Append(split[i].AsSpan(1));
            }
        }

        return result.ToString();
    }
}
