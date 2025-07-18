﻿using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Storyboards;
using Newtonsoft.Json.Linq;
using NLua;
using osu.Framework.Graphics;

namespace fluXis.Scripting.Models.Storyboarding;

public class LuaStoryboardElement : ILuaModel
{
    [LuaHide]
    protected virtual StoryboardElementType Type => StoryboardElementType.Script;

    [LuaMember(Name = "z")]
    public int ZIndex { get; set; }

    [LuaMember(Name = "layer")]
    public StoryboardLayer Layer { get; set; }

    [LuaMember(Name = "time")]
    public float StartTime { get; set; }

    [LuaMember(Name = "endtime")]
    public float EndTime { get; set; }

    [LuaMember(Name = "anchor")]
    public Anchor Anchor { get; set; } = Anchor.TopLeft;

    [LuaMember(Name = "origin")]
    public Anchor Origin { get; set; } = Anchor.TopLeft;

    [LuaMember(Name = "x")]
    public float StartX { get; set; }

    [LuaMember(Name = "y")]
    public float StartY { get; set; }

    [LuaMember(Name = "blend")]
    public bool Blending { get; set; }

    [LuaMember(Name = "width")]
    public float Width { get; set; }

    [LuaMember(Name = "height")]
    public float Height { get; set; }

    [LuaMember(Name = "color")]
    public uint Color { get; set; } = 0xffffffff;

    [LuaHide]
    public List<LuaStoryboardAnimation> Animations { get; set; } = new();

    [LuaHide]
    public Dictionary<string, JToken> ExtraParameters { get; set; } = new();

    [LuaMember(Name = "animate")]
    public void AddAnimation(string type, float time, float len, string start, string end, string ease) => Animations.Add(new LuaStoryboardAnimation
    {
        StartTime = time,
        Duration = len,
        Easing = Enum.TryParse<Easing>(ease, out var easing) ? easing : Easing.None,
        Type = Enum.Parse<StoryboardAnimationType>(type),
        Start = start,
        End = end
    });

    [LuaMember(Name = "param")]
    public object GetParameter(string key, object fallback)
    {
        if (!ExtraParameters.TryGetValue(key, out var value))
            return fallback;

        return value.ToObject<object>();
    }

    [LuaHide]
    public virtual StoryboardElement Build() => new()
    {
        Type = Type,
        ZIndex = ZIndex,
        Layer = Layer,
        StartTime = StartTime,
        EndTime = EndTime,
        Anchor = Anchor,
        Origin = Origin,
        StartX = StartX,
        StartY = StartY,
        Blending = Blending,
        Width = Width,
        Height = Height,
        Color = Color,
        Animations = Animations.Select(a => a.Build()).ToList()
    };

    public static LuaStoryboardElement FromElement(StoryboardElement element) => new()
    {
        ZIndex = element.ZIndex,
        Layer = element.Layer,
        StartTime = (float)element.StartTime,
        EndTime = (float)element.EndTime,
        Anchor = element.Anchor,
        Origin = element.Origin,
        StartX = element.StartX,
        StartY = element.StartY,
        Blending = element.Blending,
        Width = element.Width,
        Height = element.Height,
        Color = element.Color,
        ExtraParameters = element.Parameters
    };
}
