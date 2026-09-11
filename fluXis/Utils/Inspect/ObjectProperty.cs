#nullable enable
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using fluXis.Utils.Attributes;

namespace fluXis.Utils.Inspect;

public class ObjectProperty
{
    public PropertyInfo Property { get; }
    public Type Type { get; }

    public object? Value { get; }
    public CustomCreateMethodAttribute? CustomCreateMethod { get; }

    public string Group { get; }
    public string Label { get; }
    public string Tooltip { get; }
    public string Placeholder { get; }
    public bool ReadOnly { get; }

    // text
    public int MaxLength { get; }
    public bool IsPassword { get; }

    // numbers
    public double? MinValue { get; }
    public double? MaxValue { get; }

    public TypeOverrideAttribute.Type? Override { get; }

    public ObjectProperty(PropertyInfo prop, object? value, Attribute[] attrs)
    {
        Property = prop;
        Type = prop.PropertyType;
        Value = value;

        CustomCreateMethod = attrs.OfType<CustomCreateMethodAttribute>().LastOrDefault();

        Group = attrs.OfType<GroupAttribute>().LastOrDefault()?.Group ?? string.Empty;
        Label = attrs.OfType<DescriptionAttribute>().LastOrDefault()?.Description ?? prop.Name;
        Tooltip = attrs.OfType<TooltipAttribute>().LastOrDefault()?.Tooltip ?? string.Empty;
        Placeholder = attrs.OfType<PlaceholderAttribute>().LastOrDefault()?.Placeholder ?? string.Empty;
        ReadOnly = attrs.OfType<ReadOnlyAttribute>().LastOrDefault()?.IsReadOnly ?? false;

        MaxLength = attrs.OfType<MaxLengthAttribute>().LastOrDefault()?.Length ?? 256;
        IsPassword = attrs.OfType<PasswordPropertyTextAttribute>().LastOrDefault()?.Password ?? false;

        var range = attrs.OfType<RangeAttribute>().LastOrDefault();
        MinValue = range?.Minimum as double?;
        MaxValue = range?.Maximum as double?;

        Override = attrs.OfType<TypeOverrideAttribute>().LastOrDefault()?.CustomType;
    }
}
