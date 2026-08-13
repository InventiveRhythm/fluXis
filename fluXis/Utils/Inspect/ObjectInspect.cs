using System;
using System.Collections.Generic;
using System.Reflection;
using fluXis.Utils.Attributes;
using Newtonsoft.Json;

namespace fluXis.Utils.Inspect;

#nullable enable

public static class ObjectInspect
{
    private static Dictionary<Type, IReadOnlyList<ObjectProperty>> cache { get; } = new();

    public static IReadOnlyList<ObjectProperty> GetProperties(object obj, Options opt = 0)
    {
        var objType = obj.GetType();

        if (cache.TryGetValue(objType, out var c))
            return c;

        var props = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var output = new List<ObjectProperty>();

        foreach (var prop in props)
        {
            if (prop.GetCustomAttribute<HiddenAttribute>()?.Hide ?? false)
                continue;

            if (opt.HasFlag(Options.RequireJson) && prop.GetCustomAttribute<JsonPropertyAttribute>() == null)
                continue;

            if (prop.GetMethod == null || prop.SetMethod == null)
                continue;

            output.Add(new ObjectProperty(prop, prop.GetValue(obj)));
        }

        var ro = output.AsReadOnly();
        cache[objType] = ro;
        return ro;
    }

    [Flags]
    public enum Options
    {
        RequireJson = 1 << 0
    }
}
