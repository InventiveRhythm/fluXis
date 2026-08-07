using System;
using System.ComponentModel;
using System.Reflection;

namespace fluXis.Utils.Extensions;

public static class GeneralExtensions
{
    public static string GetTypeDescription(this Type type) => type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? type.Name;
}
