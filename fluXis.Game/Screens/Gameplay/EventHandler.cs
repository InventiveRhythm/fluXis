using System;
using System.Collections.Generic;
using fluXis.Game.Map.Structures;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay;

public partial class EventHandler<T> : Component where T : TimedObject
{
    public T Current { get; private set; }
    public List<T> Objects { get; set; }
    public Action<T> Trigger { get; set; }

    public EventHandler(List<T> objects, Action<T> trigger = null)
    {
        Objects = objects;
        Trigger = trigger;
    }

    protected override void Update()
    {
        base.Update();

        var cur = Current;

        foreach (var obj in Objects)
        {
            if (obj.Time <= Clock.CurrentTime)
                Current = obj;
        }

        if (Current != cur)
            Trigger?.Invoke(Current);
    }
}
