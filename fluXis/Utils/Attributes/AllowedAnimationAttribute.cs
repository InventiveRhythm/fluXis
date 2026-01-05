using System;
using fluXis.Storyboards;

namespace fluXis.Utils.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class AllowedAnimationAttribute : Attribute
{
    public StoryboardAnimationType Type { get; }
    public bool IsDeny { get; }

    public AllowedAnimationAttribute(StoryboardAnimationType type, bool deny = false)
    {
        Type = type;
        IsDeny = deny;
    }
}
