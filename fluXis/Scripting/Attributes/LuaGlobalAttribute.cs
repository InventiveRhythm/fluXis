using System;

namespace fluXis.Scripting.Attributes;

#nullable enable

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class LuaGlobalAttribute : Attribute
{
    public string? Name { get; init; }
}
