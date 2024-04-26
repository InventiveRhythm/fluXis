namespace fluXis.Game.Screens.Edit.Actions;

public class ApplyOffsetAction : EditorAction
{
    public override string Description => $"Add offset of {offset}ms to everything";

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
