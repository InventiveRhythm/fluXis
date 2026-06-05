using fluXis.Storyboards;
using NLua;
using osu.Framework.Graphics;

namespace fluXis.Scripting.Models.Storyboarding;

public class LuaStoryboardAnimation : ILuaModel
{
    public LuaStoryboardAnimation(LuaStoryboardElement parentElement)
    {
        ParentElement = parentElement;
    }

    [LuaMember(Name = "time")]
    public double StartTime { get; set; }

    [LuaMember(Name = "duration")]
    public double Duration { get; set; }

    [LuaMember(Name = "ease")]
    public Easing Easing { get; set; }

    [LuaMember(Name = "type")]
    public StoryboardAnimationType Type { get; set; }

    [LuaMember(Name = "useStartValue")]
    public bool UseStartValue { get; set; } = true;

    [LuaMember(Name = "start")]
    public string Start { get; set; }

    [LuaMember(Name = "end")]
    public string End { get; set; }

    [LuaHide]
    public LuaStoryboardElement ParentElement;

    // TODO:
    // this is probably a bad idea for the future but also I don't think that building parent element for every animation is the right approach
    // It will be kept as null for now since we don't access lua animations' parent elements
    public StoryboardAnimation Build() => new(null)
    {
        StartTime = StartTime,
        Duration = Duration,
        Easing = Easing,
        Type = Type,
        UseStartValue = UseStartValue,
        StartValue = Start,
        ValueEnd = End
    };
}
