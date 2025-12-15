using System;
using System.Reflection;

namespace fluXis.Utils.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class WidthHeightAttribute : Attribute
{
}

public static class EnumFieldAttributeExtensions
{
    public static bool HasAttribute<T, A>(this T val)
        where T : Enum
        where A : Attribute
    {
        var type = typeof(T);
        var field = type.GetField(val.ToString()!);
        var attr = field.GetCustomAttribute<A>();
        return attr != null;
    }
}
