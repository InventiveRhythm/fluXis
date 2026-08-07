using System.Linq;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs;
using osuTK;

namespace fluXis.Screens.Edit.Actions.Generic;

public class ObjectMoveAction<T> : EditorAction
    where T : ITimedObject
{
    public override string Description => $"Move {infos.Length} {ChartingTab.FormatTypeName<T>(infos.Length > 1)}";

    private T[] infos { get; }
    private Vector2d[] originalPos { get; }
    private Vector2d[] newPos;

    public ObjectMoveAction(T[] infos)
    {
        this.infos = infos;
        originalPos = CreateFrom([.. infos]);
        newPos = [.. originalPos]; // copy, since it gets updated later
    }

    public override void Run(EditorMap map) => Apply(newPos, false);
    public override void Undo(EditorMap map) => Apply(originalPos, false);

    public void Apply(Vector2d[] vecs, bool update)
    {
        if (update)
            newPos = vecs;

        for (var i = 0; i < infos.Length; i++)
        {
            var info = infos[i];
            var vec = vecs[i];

            info.Time = vec[0];
            info.Lane = (int)vec[1];
        }
    }

    public static Vector2d[] CreateFrom(ITimedObject[] objs)
        => objs.Select(x => new Vector2d(x.Time, x.Lane)).ToArray();
}
