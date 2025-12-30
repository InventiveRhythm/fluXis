using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace fluXis.Utils.Attributes;

#nullable enable

[AttributeUsage(AttributeTargets.Field)]
public class WidthHeightAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ShaderStrengthAttribute : Attribute
{
    public int Index { get; }
    public float Min { get; set; }
    public float Max { get; set; } = 1f;
    public float Step { get; set; } = 0.01f;

    public string? ParamName { get; set; }
    public string? Tooltip { get; set; }

    /// <summary>
    /// Makes values apply to both start and end at the same time.
    /// </summary>
    public bool Single { get; set; } = false;

    public ShaderStrengthAttribute(int index = 1)
    {
        Index = index;
    }
}

public static class EnumFieldAttributeExtensions
{
    public static bool HasAttribute<T, A>(this T val)
        where T : Enum
        where A : Attribute
    {
        var type = typeof(T);
        var field = type.GetField(val.ToString());
        var attr = field?.GetCustomAttribute<A>();
        return attr != null;
    }

    public static bool TryGetAttribute<T, A>(this T val, [NotNullWhen(true)] out A? attr)
        where T : Enum
        where A : Attribute
    {
        var type = typeof(T);
        var field = type.GetField(val.ToString()!);

        if (field is null)
            throw new InvalidOperationException($"Field {val} is not part of {nameof(T)}.");

        attr = field.GetCustomAttribute<A>();
        return attr != null;
    }

    public static bool TryGetAllAttributes<T, A>(this T val, out A[] attr)
        where T : Enum
        where A : Attribute
    {
        var type = typeof(T);
        var field = type.GetField(val.ToString()!);

        if (field is null)
            throw new InvalidOperationException($"Field {val} is not part of {nameof(T)}.");

        attr = field.GetCustomAttributes<A>().ToArray();
        return attr.Length > 0;
    }
}
