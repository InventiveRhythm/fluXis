using fluXis.Game.Storyboards;
using NLua;
using osu.Framework.Graphics;

namespace fluXis.Game.Scripting.Models.Storyboarding;

public class LuaStoryboardAnimation : ILuaModel
{
    [LuaMember(Name = "time")]
    public double StartTime { get; set; }

    [LuaMember(Name = "duration")]
    public double Duration { get; set; }

    [LuaMember(Name = "ease")]
    public Easing Easing { get; set; }

    [LuaMember(Name = "type")]
    public StoryboardAnimationType Type { get; set; }

    [LuaMember(Name = "start")]
    public string Start { get; set; }

    [LuaMember(Name = "end")]
    public string End { get; set; }

    public StoryboardAnimation Build() => new()
    {
        StartTime = StartTime,
        Duration = Duration,
        Easing = Easing,
        Type = Type,
        ValueStart = Start,
        ValueEnd = End
    };
}
