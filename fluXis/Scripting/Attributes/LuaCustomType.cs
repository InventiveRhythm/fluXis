using System;

namespace fluXis.Scripting.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class LuaCustomType : Attribute
{
    public Type Target { get; }

    public LuaCustomType(Type target)
    {
        Target = target;
    }
}
