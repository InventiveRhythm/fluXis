using System;
using System.Reflection;
using fluXis.Graphics.Sprites.Icons;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Utils.Attributes;

#nullable enable

[AttributeUsage(AttributeTargets.All)]
public class IconAttribute : Attribute
{
    public int Code { get; init; }
    public bool Fill { get; init; }
    public bool FluXis { get; }

    public IconAttribute(int code)
    {
        Code = code;
    }

    public IconAttribute(FluXisIconType flx)
    {
        Code = (int)flx;
        FluXis = true;
    }
}

public static class IconAttrExtensions
{
    public static IconUsage GetIcon(this object value)
    {
        if (value is IconUsage icon)
            return icon;

        IconAttribute? attr;

        if (value is Type t)
            attr = t.GetCustomAttribute<IconAttribute>();
        else
        {
            Type type = value.GetType();
            attr = type.GetField(value.ToString() ?? string.Empty)?.GetCustomAttribute<IconAttribute>();
        }

        var code = attr?.Code ?? 0x3f;

        if (attr?.FluXis ?? false)
            return FluXisIcon.Get((FluXisIconType)code);

        return attr is { Fill: true } ? Phosphor.Fill.Get(code) : Phosphor.Bold.Get(code);
    }
}
