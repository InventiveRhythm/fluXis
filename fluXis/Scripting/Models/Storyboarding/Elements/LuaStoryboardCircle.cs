using fluXis.Scripting.Attributes;
using fluXis.Storyboards;

namespace fluXis.Scripting.Models.Storyboarding.Elements;

[LuaDefinition("storyboard")]
public class LuaStoryboardCircle : LuaStoryboardElement
{
    protected override StoryboardElementType Type => StoryboardElementType.Circle;
}
