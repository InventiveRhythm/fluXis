using System;
using System.Reflection;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Utils;

[AttributeUsage(AttributeTargets.All)]
public class IconAttribute : Attribute
{
    public virtual int Code { get; init; }

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
        var code = type.GetField(value.ToString() ?? string.Empty)?.GetCustomAttribute<IconAttribute>()?.Code ?? 0x3f;
        return FontAwesome6.Solid.GetSolid(code);
    }
}
