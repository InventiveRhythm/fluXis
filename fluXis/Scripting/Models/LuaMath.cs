using System;
using NLua;

namespace fluXis.Scripting.Models;

public class LuaMath : ILuaModel
{
    [LuaMember(Name = "pi")]
    public double Pi => Math.PI;

    [LuaMember(Name = "cos")]
    public double Cos(double d) => Math.Cos(d);

    [LuaMember(Name = "atan2")]
    public double Atan2(double x, double y) => Math.Atan2(x, y);

    [LuaMember(Name = "vecsub")]
    public LuaVector VecSub(LuaVector a, LuaVector b) => new(a.TKVector - b.TKVector);
}
