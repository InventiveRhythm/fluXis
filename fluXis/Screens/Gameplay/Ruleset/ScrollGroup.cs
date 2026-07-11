using System.Collections.Generic;
using fluXis.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.Ruleset;

public partial class ScrollGroup : Component
{
    private readonly List<ScrollVelocity> velocities = new();
    private readonly List<double> marks = new();

    public double CurrentTime { get; private set; }
    public float ScrollMultiplier { get; set; } = 1;

    public override bool RemoveCompletedTransforms => false;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Origin = Anchor.Centre;
    }

    protected override void Update()
    {
        base.Update();

        var current = Clock.CurrentTime;
        var idx = 0;

        while (idx < velocities.Count && velocities[idx].Time <= current)
            idx++;

        CurrentTime = PositionFromTime(current, idx);
    }

    public double PositionFromTime(double time, int index = -1)
    {
        if (velocities.Count == 0)
            return time;

        if (index == -1)
        {
            index = velocities.BinarySearch(new ScrollVelocity { Time = time }, Comparer<ScrollVelocity>.Create((a, b) => a.Time.CompareTo(b.Time)));

            if (index < 0)
                index = ~index;
            else
            {
                // if there are multiple SVs stacked together at the same time then we want to use the last one
                while (index < velocities.Count && velocities[index].Time == time)
                    index++;
            }
        }

        if (index == 0)
            return time;

        var prev = velocities[index - 1];

        var position = marks[index - 1];
        position += (time - prev.Time) * prev.Multiplier;
        return position;
    }

    public void InitMarkers()
    {
        if (velocities.Count == 0 || marks.Count > 0)
            return;

        velocities.Sort((a, b) => a.Time.CompareTo(b.Time));

        var first = velocities[0];

        var time = first.Time;
        marks.Add(time);

        for (var i = 1; i < velocities.Count; i++)
        {
            var prev = velocities[i - 1];
            var current = velocities[i];

            time += (current.Time - prev.Time) * prev.Multiplier;
            marks.Add(time);
        }
    }

    public void AddVelocity(ScrollVelocity sv) => velocities.Add(sv);
}
