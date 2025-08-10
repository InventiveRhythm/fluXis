using System;
using fluXis.Scripting.Attributes;
using NLua;

namespace fluXis.Scripting.Models;

[LuaDefinition("math", Name = "mathf", Public = true)]
public class LuaMath : ILuaModel
{
    [LuaMember(Name = "pi")]
    public double Pi => Math.PI;

    [LuaMember(Name = "cos")]
    public double Cos(double d) => Math.Cos(d);

    [LuaMember(Name = "atan2")]
    public double Atan2(double x, double y) => Math.Atan2(x, y);

    [LuaMember(Name = "vecsub")]
    public LuaVector2 VecSub(LuaVector2 a, LuaVector2 b) => new(a.TKVector - b.TKVector);
}
