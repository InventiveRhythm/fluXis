using System;
using fluXis.Game.Storyboards;
using NLua;
using osu.Framework.Graphics;

namespace fluXis.Game.Scripting.Models.Storyboarding;

public class LuaStoryboardElement : ILuaModel
{
    [LuaHide]
    protected virtual StoryboardElementType Type => StoryboardElementType.Script;

    [LuaMember(Name = "z")]
    public int ZIndex { get; set; }

    [LuaMember(Name = "layer")]
    public int Layer { get; set; }

    [LuaMember(Name = "time")]
    public float StartTime { get; set; }

    [LuaMember(Name = "endtime")]
    public float EndTime { get; set; }

    [LuaMember(Name = "anchor")]
    public int Anchor { get; set; } = (int)osu.Framework.Graphics.Anchor.TopLeft;

    [LuaMember(Name = "origin")]
    public int Origin { get; set; } = (int)osu.Framework.Graphics.Anchor.TopLeft;

    [LuaMember(Name = "x")]
    public float StartX { get; set; }

    [LuaMember(Name = "y")]
    public float StartY { get; set; }

    [LuaMember(Name = "width")]
    public float Width { get; set; }

    [LuaMember(Name = "height")]
    public float Height { get; set; }

    [LuaMember(Name = "color")]
    public uint Color { get; set; } = 0xffffffff;

    public virtual StoryboardElement Build() => new()
    {
        Type = Type,
        ZIndex = ZIndex,
        Layer = (StoryboardLayer)Math.Clamp(Layer, 0, 2),
        StartTime = StartTime,
        EndTime = EndTime,
        Anchor = (Anchor)Anchor,
        Origin = (Anchor)Origin,
        StartX = StartX,
        StartY = StartY,
        Width = Width,
        Height = Height,
        Color = Color
    };

    public static LuaStoryboardElement FromElement(StoryboardElement element) => new()
    {
        ZIndex = element.ZIndex,
        Layer = (int)element.Layer,
        StartTime = (float)element.StartTime,
        EndTime = (float)element.EndTime,
        Anchor = (int)element.Anchor,
        Origin = (int)element.Origin,
        StartX = element.StartX,
        StartY = element.StartY,
        Width = element.Width,
        Height = element.Height,
        Color = element.Color
    };
}
