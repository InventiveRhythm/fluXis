using System;
using NLua;

namespace fluXis.Scripting.Models;

public class LuaMath : ILuaModel
{
    // constants
    [LuaMember(Name = "pi")]
    public double Pi => Math.PI;

    [LuaMember(Name = "tau")]
    public double Tau => Math.Tau;

    [LuaMember(Name = "e")]
    public double E => Math.E;

    // helper functions
    [LuaMember(Name = "abs")]
    public double Abs(double d) => Math.Abs(d);

    [LuaMember(Name = "clamp")]
    public double Clamp(double value, double min, double max) => Math.Clamp(value, min, max);

    [LuaMember(Name = "min")]
    public double Min(double a, double b) => Math.Min(a, b);

    [LuaMember(Name = "max")]
    public double Max(double a, double b) => Math.Max(a, b);

    [LuaMember(Name = "sign")]
    public int Sign(double d) => Math.Sign(d);

    [LuaMember(Name = "floor")]
    public double Floor(double d) => Math.Floor(d);

    [LuaMember(Name = "floort")]
    public double FloorThreshold(double d, double threshold) => d >= threshold ? Math.Floor(d) : d;

    [LuaMember(Name = "ceil")]
    public double Ceil(double d) => Math.Ceiling(d);

    [LuaMember(Name = "ceilt")]
    public double CeilThreshold(double d, double threshold) => d <= threshold ? Math.Ceiling(d) : d;

    [LuaMember(Name = "round")]
    public double Round(double d) => Math.Round(d);

    [LuaMember(Name = "roundt")]
    public double RoundThreshold(double d, double threshold) => Math.Abs(d - Math.Round(d)) <= threshold ? Math.Round(d) : d;

    [LuaMember(Name = "sqrt")]
    public double Sqrt(double d) => Math.Sqrt(d);

    [LuaMember(Name = "pow")]
    public double Pow(double x, double y) => Math.Pow(x, y);

    [LuaMember(Name = "exp")]
    public double Exp(double d) => Math.Exp(d);

    [LuaMember(Name = "log")]
    public double Log(double d) => Math.Log(d);

    [LuaMember(Name = "log10")]
    public double Log10(double d) => Math.Log10(d);

    [LuaMember(Name = "lerp")]
    public double Lerp(double a, double b, double t) => a + (b - a) * t;

    // trigonometric functions
    [LuaMember(Name = "sin")]
    public double Sin(double d) => Math.Sin(d);

    [LuaMember(Name = "cos")]
    public double Cos(double d) => Math.Cos(d);

    [LuaMember(Name = "tan")]
    public double Tan(double d) => Math.Tan(d);

    [LuaMember(Name = "asin")]
    public double Asin(double d) => Math.Asin(d);

    [LuaMember(Name = "acos")]
    public double Acos(double d) => Math.Acos(d);

    [LuaMember(Name = "atan")]
    public double Atan(double d) => Math.Atan(d);

    [LuaMember(Name = "atan2")]
    public double Atan2(double x, double y) => Math.Atan2(x, y);

    [LuaMember(Name = "deg")]
    public double Degrees(double radians) => radians * (180.0 / Math.PI);

    [LuaMember(Name = "rad")]
    public double Radians(double degrees) => degrees * (Math.PI / 180.0);

    // hyperbolic functions
    [LuaMember(Name = "sinh")]
    public double Sinh(double d) => Math.Sinh(d);

    [LuaMember(Name = "cosh")]
    public double Cosh(double d) => Math.Cosh(d);

    [LuaMember(Name = "tanh")]
    public double Tanh(double d) => Math.Tanh(d);

    // vector operations
    [LuaMember(Name = "vecsub")]
    public LuaVector VecSub(LuaVector a, LuaVector b) => new(a.TKVector - b.TKVector);

    [LuaMember(Name = "vecadd")]
    public LuaVector VecAdd(LuaVector a, LuaVector b) => new(a.TKVector + b.TKVector);

    [LuaMember(Name = "vecmul")]
    public LuaVector VecMul(LuaVector a, double scalar) => new(a.TKVector * (float)scalar);

    [LuaMember(Name = "vecdot")]
    public double VecDot(LuaVector a, LuaVector b) => a.TKVector.X * b.TKVector.X + a.TKVector.Y * b.TKVector.Y;

    [LuaMember(Name = "veclen")]
    public double VecLength(LuaVector v) => Math.Sqrt(v.TKVector.X * v.TKVector.X + v.TKVector.Y * v.TKVector.Y);

    [LuaMember(Name = "vecnorm")]
    public LuaVector VecNormalize(LuaVector v)
    {
        var len = VecLength(v);
        return len > 0 ? new LuaVector(v.TKVector / (float)len) : v;
    }
}