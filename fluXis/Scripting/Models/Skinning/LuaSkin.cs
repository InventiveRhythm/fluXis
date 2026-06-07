using System;
using fluXis.Scripting.Attributes;
using fluXis.Skinning;
using fluXis.Storyboards;
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

    /// <summary>
    /// gets the aspect ratio of a skin sprite. returns 1 if not available
    /// </summary>
    [LuaMember(Name = "sprratio")]
    public float GetSpriteRatio([LuaCustomType(typeof(SkinSprite))] string str)
        => skin.GetSpriteAspectRatio(Enum.TryParse<SkinSprite>(str, out var s) ? s : SkinSprite.HitObject) ?? 1;

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
