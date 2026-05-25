using fluXis.Scripting.Attributes;
using NLua;
using osuTK.Graphics;

namespace fluXis.Scripting.Models;

[LuaDefinition("struct")]
public class LuaColor4 : ILuaModel
{
    [LuaHide]
    public Color4 TKColor4 => new(R, G, B, A);

    [LuaMember(Name = "r")]
    public float R { get; set; }

    [LuaMember(Name = "g")]
    public float G { get; set; }

    [LuaMember(Name = "b")]
    public float B { get; set; }

    [LuaMember(Name = "a")]
    public float A { get; set; }

    public LuaColor4(Color4 col)
        : this(col.R, col.G, col.B, col.A)
    {
    }

    /// <summary>
    /// Create a new Color4 from RGBA ranging from 0-1
    /// </summary>
    [LuaConstructor]
    public LuaColor4(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
}
