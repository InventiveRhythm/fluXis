using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Structures;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay;

public partial class EventHandler<T> : Component where T : ITimedObject
{
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
            Trigger?.Invoke(first);

            objects.Remove(first);
            previous.Add(first);
        }
    }
}
