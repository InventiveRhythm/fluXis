using System.Linq;
using fluXis.Map.Structures;
using osuTK;

namespace fluXis.Screens.Edit.Actions.Notes;

public class NoteMoveAction : EditorAction
{
    public override string Description => $"Move {infos.Length} note(s)";

    private HitObject[] infos { get; }
    private Vector2d[] originalPos { get; }
    private Vector2d[] newPos;

    public NoteMoveAction(HitObject[] infos)
    {
        this.infos = infos;
        originalPos = CreateFrom(infos);
        newPos = originalPos.ToArray(); // copy, since it gets updated later
    }

    public override void Run(EditorMap map)
    {
        Apply(newPos, false);
    }

    public override void Undo(EditorMap map)
    {
        Apply(originalPos, false);
    }

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

    public static Vector2d[] CreateFrom(HitObject[] objs)
        => objs.Select(x => new Vector2d(x.Time, x.Lane)).ToArray();
}
