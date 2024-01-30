namespace fluXis.Game.Screens.Edit.Actions;

public class ApplyOffsetAction : EditorAction
{
    private EditorMapInfo mapInfo { get; }
    private float offset { get; }

    public ApplyOffsetAction(EditorMapInfo mapInfo, float offset)
    {
        this.mapInfo = mapInfo;
        this.offset = offset;
    }

    public override void Run() => mapInfo.ApplyOffsetToAll(offset);
    public override void Undo() => mapInfo.ApplyOffsetToAll(-offset);
}
