using System;
using System.Linq;
using System.Reflection;
using osu.Framework.Logging;

// TODO: Maybe we should make a base class for this

namespace fluXis.Scripting;

/// <summary>
/// 2 - Make storyboard elements and animations use relative time
/// 2.1 -
/// </summary>
public static class StoryboardApiVersion
{
    public static readonly Version V1 = new(1, 0);
    public static readonly Version V2 = new(2, 0);
    public static readonly Version LATEST = new(2, 1);

    public static readonly Version[] VERSIONS =
    [
        .. typeof(StoryboardApiVersion)
           .GetFields(BindingFlags.Public | BindingFlags.Static)
           .Where(f => f.IsInitOnly && f.FieldType == typeof(Version))
           .Select(f => (Version)f.GetValue(null))
    ];

    private static bool tryParseInner(string input, out Version version, out string error)
    {
        version = null;
        error = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Storyboard API version is empty.";
            return false;
        }

        input = input.Trim();

        // Something like '2' wouldn't work with Version.TryParse so we have to add a period ourselves -> '2.0'
        if (!input.Contains('.'))
            input += ".0";

        if (!Version.TryParse(input, out var parsed))
        {
            error = $"Storyboard API version '{input}' is not a valid version string.";
            return false;
        }

        if (parsed > LATEST)
        {
            Logger.Log($"Storyboard API version '{input}' is too new and may be unsupported - Continuing as normal.",
                level: LogLevel.Important);
            version = parsed;
            return true;
        }

        if (!VERSIONS.Contains(parsed))
        {
            error = $"Unsupported Storyboard API version '{input}'. Supported: {string.Join(", ", VERSIONS.Select(v => v.ToString()))}.";
            return false;
        }

        version = parsed;
        return true;
    }

    public static bool TryParse(string input, out Version version)
        => tryParseInner(input, out version, out _);

    public static bool TryParse(string input, out Version version, out string error)
        => tryParseInner(input, out version, out error);

    public static Version Parse(string input)
        => tryParseInner(input, out var version, out string error)
            ? version
            : throw new ArgumentException(error, nameof(input));

    public static bool IsValid(string input) => tryParseInner(input, out _, out _);
    public static bool IsValid(string input, out string error) => tryParseInner(input, out _, out error);
}
