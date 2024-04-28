namespace fluXis.Game.Screens.Edit.Actions;

public class ApplyOffsetAction : EditorAction
{
    public override string Description => $"Add offset of {offset}ms to everything";
    private float offset { get; }

    public ApplyOffsetAction(float offset)
    {
        this.offset = offset;
    }

    public override void Run(EditorMap map) => map.ApplyOffsetToAll(offset);
    public override void Undo(EditorMap map) => map.ApplyOffsetToAll(-offset);
}
