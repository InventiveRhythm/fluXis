using fluXis.Scripting.Attributes;
using NLua;
using osuTK;

namespace fluXis.Scripting.Models;

[LuaDefinition("struct")]
public class LuaVector2 : ILuaModel
{
    [LuaHide]
    public Vector2 TKVector => new(X, Y);

    [LuaMember(Name = "x")]
    public float X { get; set; }

    [LuaMember(Name = "y")]
    public float Y { get; set; }

    public LuaVector2(Vector2 vec)
        : this(vec.X, vec.Y)
    {
    }

    [LuaConstructor]
    public LuaVector2(float x, float y)
    {
        X = x;
        Y = y;
    }
}
