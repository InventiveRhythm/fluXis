using System;
using JetBrains.Annotations;
using osu.Framework.Graphics.Containers;

namespace fluXis.Utils.Attributes;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Property)]
public class PlaceholderAttribute : Attribute
{
    public string Placeholder { get; }

    public PlaceholderAttribute(string placeholder)
    {
        Placeholder = placeholder;
    }
}

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Property)]
public class GroupAttribute : Attribute
{
    public string Group { get; }

    public GridSizeMode SizeMode { get; set; } = GridSizeMode.Distributed;
    public float Size { get; set; }
    public float MinSize { get; set; }
    public float MaxSize { get; set; } = float.MaxValue;

    public float Aspect { get; set; }
    public bool RelativeHeight { get; set; }

    public GroupAttribute(string group)
    {
        Group = group;
    }
}

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Property)]
public class TypeOverrideAttribute : Attribute
{
    public Type CustomType { get; }

    public TypeOverrideAttribute(Type customType)
    {
        CustomType = customType;
    }

    public enum Type
    {
        Image,
        Color
    }
}

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Property)]
public class HiddenAttribute : Attribute
{
    public bool Hide { get; }

    public HiddenAttribute(bool hide = true)
    {
        Hide = hide;
    }
}
