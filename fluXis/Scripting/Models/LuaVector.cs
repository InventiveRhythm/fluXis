using NLua;
using osuTK;

namespace fluXis.Scripting.Models;

public class LuaVector : ILuaModel
{
    [LuaHide]
    public Vector2 TKVector => new(X, Y);

    [LuaMember(Name = "x")]
    public float X { get; set; }

    [LuaMember(Name = "y")]
    public float Y { get; set; }

    public LuaVector(Vector2 vec)
        : this(vec.X, vec.Y)
    {
    }

    public LuaVector(float x, float y)
    {
        X = x;
        Y = y;
    }
}
