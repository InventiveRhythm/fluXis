using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Gameplay.Ruleset.HitObjects;
using fluXis.Game.Screens.Gameplay.Ruleset.Playfields;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay;

#nullable enable

public partial class EventHandler<T> : CompositeComponent where T : ITimedObject
{
    [Resolved]
    private Playfield? playfield { get; set; }

    [Resolved]
    private HitObjectManager? manager { get; set; }

    private List<T> objects { get; }
    private List<T> previous { get; } = new();

    protected Action<T>? Trigger { get; init; }

    public EventHandler(IEnumerable<T> objects, Action<T>? trigger = null)
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
        switch (obj)
        {
            case IApplicableToPlayfield pf when playfield is not null:
                pf.Apply(playfield);
                return;

            case IApplicableToHitManager hm when manager is not null:
                hm.Apply(manager);
                return;

            default:
                Trigger?.Invoke(obj);
                break;
        }
    }
}
