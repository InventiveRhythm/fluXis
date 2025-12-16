using fluXis.Scripting.Attributes;
using fluXis.Skinning;
using NLua;

namespace fluXis.Scripting.Models.Skinning;

[LuaDefinition("skin", Name = "skin", Public = true)]
public class LuaSkin : ILuaModel
{
    private ISkin skin { get; }

    public LuaSkin(ISkin skin)
    {
        this.skin = skin;
    }

    [LuaMember(Name = "colwidth")]
    public int GetColumnWidth(int mode)
    {
        try
        {
            return skin.SkinJson.GetKeymode(mode).ColumnWidth;
        }
        catch
        {
            return 0;
        }
    }

    [LuaMember(Name = "hitpos")]
    public int GetHitPosition(int mode)
    {
        try
        {
            return skin.SkinJson.GetKeymode(mode).HitPosition;
        }
        catch
        {
            return 0;
        }
    }

    [LuaMember(Name = "recoffset")]
    public int GetReceptorOffset(int mode)
    {
        try
        {
            return skin.SkinJson.GetKeymode(mode).ReceptorOffset;
        }
        catch
        {
            return 0;
        }
    }

    [LuaMember(Name = "recfirst")]
    public bool GetReceptorsFirst(int mode)
    {
        try
        {
            return skin.SkinJson.GetKeymode(mode).ReceptorsFirst;
        }
        catch
        {
            return false;
        }
    }
}
