using fluXis.Scripting.Attributes;
using fluXis.Storyboards;
using NLua;

namespace fluXis.Scripting.Models.Storyboarding.Elements;

[LuaDefinition("storyboard")]
public class LuaStoryboardSkinSprite : LuaStoryboardElement
{
    protected override StoryboardElementType Type => StoryboardElementType.SkinSprite;

    [LuaMember(Name = "sprite")]
    public SkinSprite Sprite { get; set; }

    [LuaMember(Name = "lane")]
    public int Lane { get; set; }

    [LuaMember(Name = "keycount")]
    public int Keycount { get; set; }

    public LuaStoryboardSkinSprite(SkinSprite spr)
    {
        Sprite = spr;
    }

    public override StoryboardElement Build()
    {
        var el = base.Build();
        el.Parameters["sprite"] = (int)Sprite;
        el.Parameters["lane"] = Lane;
        el.Parameters["keycount"] = Keycount;
        return el;
    }
}
