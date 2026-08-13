#nullable enable
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

    public ObjectProperty(PropertyInfo prop, object? value)
    {
        Property = prop;
        Type = prop.PropertyType;
        Value = value;

        CustomCreateMethod = prop.GetCustomAttribute<CustomCreateMethodAttribute>();

        Group = prop.GetCustomAttribute<GroupAttribute>()?.Group ?? string.Empty;
        Label = prop.GetCustomAttribute<DescriptionAttribute>()?.Description ?? prop.Name;
        Tooltip = prop.GetCustomAttribute<TooltipAttribute>()?.Tooltip ?? string.Empty;
        Placeholder = prop.GetCustomAttribute<PlaceholderAttribute>()?.Placeholder ?? string.Empty;
        ReadOnly = prop.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly ?? false;

        MaxLength = prop.GetCustomAttribute<MaxLengthAttribute>()?.Length ?? 256;
        IsPassword = prop.GetCustomAttribute<PasswordPropertyTextAttribute>()?.Password ?? false;

        var range = prop.GetCustomAttribute<RangeAttribute>();
        MinValue = range?.Minimum as double?;
        MaxValue = range?.Maximum as double?;

        Override = prop.GetCustomAttribute<TypeOverrideAttribute>()?.CustomType;
    }
}
