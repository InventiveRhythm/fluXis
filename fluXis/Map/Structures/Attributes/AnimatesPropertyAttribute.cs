using System;
using System.Linq;
using System.Reflection;

namespace fluXis.Map.Structures.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AnimatesPropertyAttribute : Attribute
{
    public string Property { get; }

    public AnimatesPropertyAttribute(string property)
    {
        Property = property;
    }
}

public static class AnimatesPropertyAttributeExtensions
{
    public static string[] GetAnimatedProperties(this Type type)
    {
        var attrs = type.GetCustomAttributes<AnimatesPropertyAttribute>();
        return attrs.Select(x => x.Property).ToArray();
    }
}
