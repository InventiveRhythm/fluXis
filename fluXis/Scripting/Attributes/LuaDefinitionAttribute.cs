using System;

namespace fluXis.Scripting.Attributes;

#nullable enable

[AttributeUsage(AttributeTargets.Class)]
public class LuaDefinitionAttribute : Attribute
{
    public string FileName { get; }
    public string? Name { get; init; } = null;
    public bool Public { get; init; } = false;
    public bool Hide { get; init; } = false;

    public LuaDefinitionAttribute(string fileName)
    {
        FileName = fileName;
    }
}
