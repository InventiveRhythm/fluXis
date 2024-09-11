using fluXis.Game.Storyboards;
using NLua;

namespace fluXis.Game.Scripting.Models.Storyboarding.Elements;

public class LuaStoryboardText : LuaStoryboardElement
{
    protected override StoryboardElementType Type => StoryboardElementType.Text;

    [LuaMember(Name = "size")]
    public float FontSize { get; set; } = 20;

    [LuaMember(Name = "text")]
    public string Text { get; set; } = "text";

    public override StoryboardElement Build()
    {
        var el = base.Build();
        el.Parameters["size"] = FontSize;
        el.Parameters["text"] = Text;
        return el;
    }
}
