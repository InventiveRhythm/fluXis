using fluXis.Scripting.Attributes;
using fluXis.Storyboards;

namespace fluXis.Scripting.Models.Storyboarding.Elements;

[LuaDefinition("storyboard")]
public class LuaStoryboardBox : LuaStoryboardElement
{
    protected override StoryboardElementType Type => StoryboardElementType.Box;
}
