using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay;

public partial class EventHandler<T> : CompositeComponent where T : ITimedObject
{
    [Resolved]
    private Playfield playfield { get; set; }

    private List<T> objects { get; }
    private List<T> previous { get; } = new();

    protected Action<T> Trigger { get; init; }

    public EventHandler(IEnumerable<T> objects, Action<T> trigger = null)
    {
        this.objects = objects.ToList(); // Copy the list to avoid modifying the original
        Trigger = trigger;
    }

    protected override void Update()
    {
        base.Update();

        while (objects.Count > 0 && objects.First().Time <= Clock.CurrentTime)
        {
            var first = objects.First();
            trigger(first);

            objects.Remove(first);
            previous.Add(first);
        }
    }

    private void trigger(T obj)
    {
        if (obj is IApplicableToPlayfield applicableToPlayfield)
        {
            applicableToPlayfield.Apply(playfield);
            return;
        }

        Trigger?.Invoke(obj);
    }
}
