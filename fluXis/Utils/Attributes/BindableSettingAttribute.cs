using System;
using System.Collections.Generic;
using System.Reflection;
using fluXis.Screens.Edit.UI.Variable;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace fluXis.Utils.Attributes;

#nullable enable

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Property)]
public class BindableSettingAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }
    public string? Key { get; }

    public BindableSettingAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public BindableSettingAttribute(string name, string description, string? key)
    {
        Name = name;
        Description = description;
        Key = key;
    }
}

public class BindableSettingInfo
{
    public BindableSettingAttribute Attribute { get; }
    public IBindable Bindable { get; }

    public BindableSettingInfo(BindableSettingAttribute attribute, IBindable bindable)
    {
        Attribute = attribute;
        Bindable = bindable;
    }
}

public static class BindableSettingExtensions
{
    public static IEnumerable<BindableSettingInfo> GetSettingInfos(this object target)
    {
        var type = target.GetType();
        var props = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            var attr = prop.GetCustomAttribute<BindableSettingAttribute>();

            if (attr is null)
                continue;

            if (!typeof(IBindable).IsAssignableFrom(prop.PropertyType))
                throw new InvalidOperationException($"Property {prop.Name} must be of type {nameof(IBindable)}.");

            var bindable = prop.GetValue(target);

            if (bindable is null)
                throw new InvalidOperationException($"Property {prop.Name} cannot be null.");

            yield return new BindableSettingInfo(attr, (IBindable)bindable);
        }
    }

    public static IEnumerable<Drawable> CreatePointSettings(this object target)
    {
        var infos = GetSettingInfos(target);

        foreach (var info in infos)
        {
            var name = info.Attribute.Name;
            var description = info.Attribute.Description;

            switch (info.Bindable.GetBoundCopy())
            {
                case Bindable<bool> toggle:
                    yield return new EditorVariableToggle
                    {
                        Text = name,
                        TooltipText = description,
                        CurrentValue = toggle.Value,
                        Bindable = toggle
                    };

                    break;

                case Bindable<string> text:
                    yield return new EditorVariableTextBox
                    {
                        Text = name,
                        TooltipText = description,
                        CurrentValue = text.Value,
                        OnValueChanged = t => text.Value = t.Text
                    };

                    break;

                case Bindable<double> num:
                {
                    if (num is BindableNumber<double> { HasDefinedRange: true } range)
                    {
                        yield return new EditorVariableSlider<double>
                        {
                            Text = name,
                            TooltipText = description,
                            Min = range.MinValue,
                            Max = range.MaxValue,
                            CurrentValue = range.Value,
                            Step = range.Precision,
                            Bindable = range
                        };

                        continue;
                    }

                    yield return new EditorVariableTextBox
                    {
                        Text = name,
                        TooltipText = description
                    };

                    break;
                }

                case Bindable<float> num:
                {
                    if (num is BindableNumber<float> { HasDefinedRange: true } range)
                    {
                        yield return new EditorVariableSlider<float>
                        {
                            Text = name,
                            TooltipText = description,
                            Min = range.MinValue,
                            Max = range.MaxValue,
                            CurrentValue = range.Value,
                            Step = range.Precision,
                            Bindable = range
                        };

                        continue;
                    }

                    yield return new EditorVariableTextBox
                    {
                        Text = name,
                        TooltipText = description
                    };

                    break;
                }

                case Bindable<long> num:
                {
                    if (num is BindableNumber<long> { HasDefinedRange: true } range)
                    {
                        yield return new EditorVariableSlider<long>
                        {
                            Text = name,
                            TooltipText = description,
                            Min = range.MinValue,
                            Max = range.MaxValue,
                            CurrentValue = range.Value,
                            Step = range.Precision,
                            Bindable = range
                        };

                        continue;
                    }

                    yield return new EditorVariableTextBox
                    {
                        Text = name,
                        TooltipText = description
                    };

                    break;
                }

                case Bindable<int> num:
                {
                    if (num is BindableNumber<int> { HasDefinedRange: true } range)
                    {
                        yield return new EditorVariableSlider<int>
                        {
                            Text = name,
                            TooltipText = description,
                            Min = range.MinValue,
                            Max = range.MaxValue,
                            CurrentValue = range.Value,
                            Step = range.Precision,
                            Bindable = range
                        };

                        continue;
                    }

                    yield return new EditorVariableTextBox
                    {
                        Text = name,
                        TooltipText = description
                    };

                    break;
                }

                default:
                {
                    var valueType = info.Bindable.GetType();
                    var prop = valueType.GetProperty(nameof(Bindable<object>.Value), BindingFlags.Instance | BindingFlags.Public);

                    if (prop is null)
                        throw new InvalidOperationException($"Property {nameof(Bindable<object>.Value)} not found.");

                    var bindableType = prop.PropertyType;

                    if (bindableType.IsEnum)
                    {
                        var listType = typeof(List<>);
                        var constructedListType = listType.MakeGenericType(bindableType);
                        var items = Activator.CreateInstance(constructedListType);
                        var addMethod = constructedListType.GetMethod("Add");

                        var enumValues = Enum.GetValues(bindableType);

                        foreach (var enumValue in enumValues)
                            addMethod?.Invoke(items, new[] { enumValue });

                        var dropdownType = typeof(EditorVariableDropdown<>).MakeGenericType(bindableType);
                        var dropdown = (Drawable)Activator.CreateInstance(dropdownType)!;

                        dropdownType.GetProperty(nameof(EditorVariableDropdown<object>.Text))?.SetValue(dropdown, name);
                        dropdownType.GetProperty(nameof(EditorVariableDropdown<object>.TooltipText))?.SetValue(dropdown, (LocalisableString)description);
                        dropdownType.GetProperty(nameof(EditorVariableDropdown<object>.Bindable))?.SetValue(dropdown, info.Bindable.GetBoundCopy());
                        dropdownType.GetProperty(nameof(EditorVariableDropdown<object>.Items))?.SetValue(dropdown, items);

                        yield return dropdown;
                    }

                    break;
                }
            }
        }
    }
}
