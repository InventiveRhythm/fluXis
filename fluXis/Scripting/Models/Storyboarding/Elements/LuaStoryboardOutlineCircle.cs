using fluXis.Scripting.Attributes;
using fluXis.Storyboards;
using NLua;

namespace fluXis.Scripting.Models.Storyboarding.Elements;

[LuaDefinition("storyboard")]
public class LuaStoryboardOutlineCircle : LuaStoryboardElement
{
    protected override StoryboardElementType Type => StoryboardElementType.OutlineCircle;

    [LuaMember(Name = "border")]
    public float Border { get; set; }

    public override StoryboardElement Build()
    {
        var el = base.Build();
        el.Parameters["border"] = Border;
        return el;
    }
}
