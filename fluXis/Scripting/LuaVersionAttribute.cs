using System;

namespace fluXis.Scripting;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class LuaVersionAttribute : Attribute
{
    public int Since { get; init; } = 1;
    public int Until { get; init; } = int.MaxValue;

    public bool Supports(int v) => v >= Since && v <= Until;
}
