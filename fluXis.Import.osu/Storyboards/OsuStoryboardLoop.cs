using System;
using System.Collections.Generic;
using fluXis.Storyboards;

namespace fluXis.Import.osu.Storyboards;

public class OsuStoryboardLoop
{
    public double StartTime { get; init; }
    public int LoopCount { get; init; }
    public List<StoryboardAnimation> Animations { get; } = new();

    public void PushTo(List<StoryboardAnimation> dest)
    {
        var time = StartTime;
        var duration = 0d;

        for (int i = 0; i < Math.Max(LoopCount, 1); i++)
        {
            foreach (var animation in Animations)
            {
                var clone = animation.DeepClone();
                clone.StartTime = time + animation.StartTime;
                duration = Math.Max(duration, animation.Duration);
                dest.Add(clone);
            }

            time += duration;
        }
    }
}
