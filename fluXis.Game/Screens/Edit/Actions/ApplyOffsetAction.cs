namespace fluXis.Game.Screens.Edit.Actions;

public class ApplyOffsetAction : EditorAction
{
    private EditorMap map { get; }
    private float offset { get; }

    public ApplyOffsetAction(EditorMap map, float offset)
    {
        this.map = map;
        this.offset = offset;
    }

    public override void Run() => map.ApplyOffsetToAll(offset);
    public override void Undo() => map.ApplyOffsetToAll(-offset);
}
