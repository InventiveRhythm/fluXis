using fluXis.Game.Storyboards;
using NLua;

namespace fluXis.Game.Scripting.Models.Storyboarding.Elements;

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
