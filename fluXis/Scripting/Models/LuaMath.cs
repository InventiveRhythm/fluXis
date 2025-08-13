using System;
using fluXis.Scripting.Attributes;
using NLua;

namespace fluXis.Scripting.Models;

[LuaDefinition("math", Name = "mathf", Public = true)]
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

    /// <summary>
    /// Returns an integer that indicates the sign of a number.
    /// -1 value is less than zero | 
    /// 0 value is equal to zero | 
    /// 1 value is greater than zero
    /// </summary>
    [LuaMember(Name = "sign")]
    public int Sign(double d) => Math.Sign(d);

    [LuaMember(Name = "floor")]
    public double Floor(double d) => Math.Floor(d);

    /// <summary>
    /// Floors the input value only if it meets or greater than the threshold.
    /// Otherwise returns the original value unchanged.
    /// </summary>
    /// <param name="d">the input</param>
    /// <param name="threshold">
    /// The minimum value at which flooring will be applied. Values below this threshold remain unchanged.
    /// </param>
    [LuaMember(Name = "floort")]
    public double FloorThreshold(double d, double threshold) => d >= threshold ? Math.Floor(d) : d;

    [LuaMember(Name = "ceil")]
    public double Ceil(double d) => Math.Ceiling(d);

    /// <summary>
    /// Ceils the input value only if it is less than or equal to the specified threshold.
    /// Otherwise returns the original value unchanged.
    /// </summary>
    /// <param name="d">the input</param>
    /// <param name="threshold">
    /// The maximum value at which ceiling will be applied. Values above this threshold remain unchanged.
    /// </param>
    [LuaMember(Name = "ceilt")]
    public double CeilThreshold(double d, double threshold) => d <= threshold ? Math.Ceiling(d) : d;

    [LuaMember(Name = "round")]
    public double Round(double d) => Math.Round(d);

    /// <summary>
    /// Rounds the input value only if it is within the specified threshold distance from a whole number.
    /// Otherwise returns the original value unchanged.
    /// Useful for rounding values that are "close enough" to whole numbers while leaving others unchanged.
    /// </summary>
    /// <param name="d">the input</param>
    /// <param name="threshold">
    /// The maximum allowable distance from a whole number for rounding to be applied.
    /// </param>
    [LuaMember(Name = "roundt")]
    public double RoundThreshold(double d, double threshold) => Math.Abs(d - Math.Round(d)) <= threshold ? Math.Round(d) : d;

    [LuaMember(Name = "sqrt")]
    public double Sqrt(double d) => Math.Sqrt(d);

    [LuaMember(Name = "pow")]
    public double Pow(double x, double y) => Math.Pow(x, y);

    /// <summary>
    /// Returns e raised to the specified power.
    /// </summary>
    [LuaMember(Name = "exp")]
    public double Exp(double d) => Math.Exp(d);

    /// <summary>
    /// Returns the natural logarithm (base e).
    /// </summary>
    [LuaMember(Name = "log")]
    public double Log(double d) => Math.Log(d);

    [LuaMember(Name = "log10")]
    public double Log10(double d) => Math.Log10(d);

    /// <summary>
    /// Linearly interpolates between two values by a given factor.
    /// </summary>
    [LuaMember(Name = "lerp")]
    public double Lerp(double a, double b, double t) => a + (b - a) * t;

    // trigonometric functions

    /// <summary>Returns the sine of an angle.</summary>
    /// <param name="radians">The angle in radians.</param>
    [LuaMember(Name = "sin")]
    public double Sin(double radians) => Math.Sin(radians);

    /// <summary>Returns the cosine of an angle.</summary>
    /// <param name="radians">The angle in radians.</param>
    [LuaMember(Name = "cos")]
    public double Cos(double radians) => Math.Cos(radians);

    /// <summary>Returns the tangent of an angle.</summary>
    /// <param name="radians">The angle in radians.</param>
    [LuaMember(Name = "tan")]
    public double Tan(double radians) => Math.Tan(radians);

    /// <summary>Returns the angle whose sine is the specified number.</summary>
    /// <param name="value">A number between -1 and 1.</param>
    /// <returns>Angle in radians between -π/2 and π/2.</returns>
    [LuaMember(Name = "asin")]
    public double Asin(double value) => Math.Asin(value);

    /// <summary>Returns the angle whose cosine is the specified number.</summary>
    /// <param name="value">A number between -1 and 1.</param>
    /// <returns>Angle in radians between 0 and π.</returns>
    [LuaMember(Name = "acos")]
    public double Acos(double value) => Math.Acos(value);

    /// <summary>Returns the angle whose tangent is the specified number.</summary>
    /// <param name="value">A real number.</param>
    /// <returns>Angle in radians between -π/2 and π/2.</returns>
    [LuaMember(Name = "atan")]
    public double Atan(double value) => Math.Atan(value);

    /// <summary>Returns the angle from the y/x ratio, handling quadrants.</summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>Angle in radians between -π and π.</returns>
    [LuaMember(Name = "atan2")]
    public double Atan2(double x, double y) => Math.Atan2(x, y);

    /// <summary>Converts from radians to degrees.</summary>
    /// <param name="radians">Angle in radians.</param>
    /// <returns>Angle in degrees.</returns>
    [LuaMember(Name = "deg")]
    public double Degrees(double radians) => radians * (180.0 / Math.PI);

    /// <summary>Converts from degrees to radians.</summary>
    /// <param name="degrees">Angle in degrees.</param>
    /// <returns>Angle in radians.</returns>
    [LuaMember(Name = "rad")]
    public double Radians(double degrees) => degrees * (Math.PI / 180.0);

    // hyperbolic functions

    /// <summary>Returns the hyperbolic sine of the specified angle.</summary>
    /// <param name="radians">The angle in radians.</param>
    [LuaMember(Name = "sinh")]
    public double Sinh(double radians) => Math.Sinh(radians);

    /// <summary>Returns the hyperbolic cosine of the specified angle.</summary>
    /// <param name="radians">The angle in radians.</param>
    [LuaMember(Name = "cosh")]
    public double Cosh(double radians) => Math.Cosh(radians);

    /// <summary>Returns the hyperbolic tangent of the specified angle.</summary>
    /// <param name="radians">The angle in radians.</param>
    [LuaMember(Name = "tanh")]
    public double Tanh(double radians) => Math.Tanh(radians);

    // vector operations

    [LuaMember(Name = "vecsub")]
    public LuaVector2 VecSub(LuaVector2 a, LuaVector2 b) => new(a.TKVector - b.TKVector);

    [LuaMember(Name = "vecadd")]
    public LuaVector2 VecAdd(LuaVector2 a, LuaVector2 b) => new(a.TKVector + b.TKVector);

    [LuaMember(Name = "vecmul")]
    public LuaVector2 VecMul(LuaVector2 a, double scalar) => new(a.TKVector * (float)scalar);

    [LuaMember(Name = "vecdot")]
    public double VecDot(LuaVector2 a, LuaVector2 b) => a.TKVector.X * b.TKVector.X + a.TKVector.Y * b.TKVector.Y;

    [LuaMember(Name = "veclen")]
    public double VecLength(LuaVector2 v) => Math.Sqrt(v.TKVector.X * v.TKVector.X + v.TKVector.Y * v.TKVector.Y);

    [LuaMember(Name = "vecnorm")]
    public LuaVector2 VecNormalize(LuaVector2 v)
    {
        var len = VecLength(v);
        return len > 0 ? new LuaVector2(v.TKVector / (float)len) : v;
    }

    /// <summary>Rotates a vector by a given angle in radians.</summary>
    /// <param name="v">the vector to rotate.</param>
    /// <param name="angle">Angle in radians.</param>
    /// <returns>a rotated vector.</returns>
    [LuaMember(Name = "vecrotate")]
    public LuaVector2 VecRotate(LuaVector2 v, double angle)
    {
        var cos = Math.Cos(angle);
        var sin = Math.Sin(angle);
        var x = v.TKVector.X * cos - v.TKVector.Y * sin;
        var y = v.TKVector.X * sin + v.TKVector.Y * cos;
        return new LuaVector2(new osuTK.Vector2((float)x, (float)y));
    }

    /// <summary>Rotates vector/point a around vector b by a given angle in radians.</summary>
    /// <param name="a">the vector to apply the rotation to.</param>
    /// <param name="b">the vector to rotate around.</param>
    /// <param name="angle">Angle in radians.</param>
    /// <returns>a rotated vector.</returns>
    [LuaMember(Name = "vecrotatearound")]
    public LuaVector2 VecRotateAround(LuaVector2 a, LuaVector2 b, double angle)
    {
        var translated = VecSub(a, b);
        var rotated = VecRotate(translated, angle);
        return VecAdd(rotated, b);
    }

    [LuaMember(Name = "vecequals")]
    public bool VecEquals(LuaVector2 a, LuaVector2 b, double acceptableDifference = double.Epsilon)
    {
        var diff = VecSub(a, b);
        return VecLength(diff) <= acceptableDifference;
    }

    /// <summary>Returns the angle of a vector in radians.</summary>
    /// <returns>Angle in radians.</returns>
    [LuaMember(Name = "vecangle")]
    public double VecAngle(LuaVector2 v) => Math.Atan2(v.TKVector.Y, v.TKVector.X);

    /// <summary>Returns the angle of between two vectors/points in radians.</summary>
    /// <returns>Angle in radians.</returns>
    [LuaMember(Name = "vecanglebetween")]
    public double VecAngleBetween(LuaVector2 a, LuaVector2 b)
    {
        var dot = VecDot(a, b);
        var lengthProduct = VecLength(a) * VecLength(b);
        if (lengthProduct == 0) return 0;
        return Math.Acos(Math.Clamp(dot / lengthProduct, -1.0, 1.0));
    }

    [LuaMember(Name = "vecdist")]
    public double VecDistance(LuaVector2 a, LuaVector2 b) => VecLength(VecSub(a, b));

    /// <summary>Checks if the vector is normalized.</summary>
    [LuaMember(Name = "vecisnorm")]
    public bool VecIsNorm(LuaVector2 v, double acceptableDifference = double.Epsilon)
    {
        var length = VecLength(v);
        return Math.Abs(length - 1.0) <= acceptableDifference ;
    }

    /// <summary>Checks if the vector is a zero vector.</summary>
    [LuaMember(Name = "veciszero")]
    public bool VecIsZero(LuaVector2 v, double acceptableDifference = double.Epsilon) => VecLength(v) <= acceptableDifference;

    /// <summary>Returns a vector with all components set to its absolute values.</summary>
    [LuaMember(Name = "vecabs")]
    public LuaVector2 VecAbs(LuaVector2 v) => new(new osuTK.Vector2(Math.Abs(v.TKVector.X), Math.Abs(v.TKVector.Y)));
}