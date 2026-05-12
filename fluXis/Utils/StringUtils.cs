using System;
using System.Text;

namespace fluXis.Utils;

public static class StringUtils
{
    public static string FormatSteamProfile(ulong id) => $"https://steamcommunity.com/profiles/{id}";

    public static string TruncateBytes(string value, int maxBytes)
    {
        if (string.IsNullOrEmpty(value))
            return value ?? "";

        var encoding = Encoding.UTF8;
        if (encoding.GetByteCount(value) <= maxBytes)
            return value;

        var bytes = encoding.GetBytes(value);
        var truncated = encoding.GetString(bytes, 0, Math.Min(bytes.Length, maxBytes));

        while (encoding.GetByteCount(truncated) > maxBytes && truncated.Length > 0)
            truncated = truncated[..^1];

        return truncated;
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
