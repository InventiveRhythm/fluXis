using System;
using System.Reflection;
using fluXis.Graphics.Sprites.Icons;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Utils.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class IconAttribute : Attribute
{
    public int Code { get; init; }
    public bool Regular { get; init; }

    public IconAttribute(int code)
    {
        Code = code;
    }
}

public static class IconAttrExtensions
{
    public static IconUsage GetIcon(this object value)
    {
        if (value is IconUsage icon)
            return icon;

        Type type = value as Type ?? value.GetType();
        var attr = type.GetField(value.ToString() ?? string.Empty)?.GetCustomAttribute<IconAttribute>();
        var code = attr?.Code ?? 0x3f;
        return attr is { Regular: true } ? FontAwesome6.Regular.GetRegular(code) : FontAwesome6.Solid.GetSolid(code);
    }
}
