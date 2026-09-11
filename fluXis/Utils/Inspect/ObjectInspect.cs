using System;
using System.Collections.Generic;
using System.Linq;
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

        /*if (cache.TryGetValue(objType, out var c))
            return c;*/

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

            var intfc = objType.GetInterfaces()
                               .Select(objType.GetInterfaceMap)
                               .SelectMany(m => m.TargetMethods.Select((t, i) => new { t, m = m.InterfaceMethods[i] }))
                               .Where(x => x.t == prop.GetMethod || x.t == prop.SetMethod)
                               .Select(x => x.m.DeclaringType!.GetProperty(x.m.Name[4..], BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                               .OfType<PropertyInfo>()
                               .ToArray();

            output.Add(new ObjectProperty(prop, prop.GetValue(obj), [.. intfc.SelectMany(i => i.GetCustomAttributes()), .. prop.GetCustomAttributes()]));
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
