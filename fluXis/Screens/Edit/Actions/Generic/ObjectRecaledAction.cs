using System.Linq;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Screens.Edit.Actions.Generic;

public class ObjectRescaledAction<T> : EditorAction
    where T : ITimedObject, IHasDuration
{
    public override string Description => $"Rescale {infos.Length} {ChartingTab.FormatTypeName<T>(infos.Length > 1)}";

    private T[] infos { get; }
    private Vector2d[] originalState { get; }
    private Vector2d[] newState;

    public ObjectRescaledAction(T[] infos)
    {
        this.infos = infos;
        originalState = CreateFrom([.. infos]);
        newState = [.. originalState]; // copy, since it gets updated later
    }

    public override void Run(EditorMap map) => Apply(map, newState, false);
    public override void Undo(EditorMap map) => Apply(map, originalState, false);

    public void Apply(EditorMap map, Vector2d[] vecs, bool update)
    {
        if (update)
            newState = vecs;

        for (var i = 0; i < infos.Length; i++)
        {
            var info = infos[i];
            var vec = vecs[i];

            bool timeChanged = !Precision.AlmostEquals(info.Time, vec[0]);
            bool durationChanged = !Precision.AlmostEquals(info.Duration, vec[1]);

            info.Time = vec[0];
            info.Duration = vec[1];

            if ((timeChanged || durationChanged) && update)
                map.Update(info);
        }
    }

    public static Vector2d[] CreateFrom(T[] objs)
        => objs.Select(x => new Vector2d(x.Time, x.Duration)).ToArray();
}
