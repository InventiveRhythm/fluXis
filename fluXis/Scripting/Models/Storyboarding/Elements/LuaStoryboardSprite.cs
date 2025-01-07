using fluXis.Storyboards;
using NLua;

namespace fluXis.Scripting.Models.Storyboarding.Elements;

public class LuaStoryboardSprite : LuaStoryboardElement
{
    protected override StoryboardElementType Type => StoryboardElementType.Sprite;

    [LuaMember(Name = "texture")]
    public string TexturePath { get; set; } = string.Empty;

    public override StoryboardElement Build()
    {
        var el = base.Build();
        el.Parameters["file"] = TexturePath;
        return el;
    }
}
