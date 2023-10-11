using fluXis.Game.Map;

namespace fluXis.Game.Screens.Edit.Actions.Notes;

public class NotePlaceAction : EditorAction
{
    private readonly HitObjectInfo info;
    private readonly EditorMapInfo mapInfo;

    public NotePlaceAction(HitObjectInfo info, EditorMapInfo mapInfo)
    {
        this.info = info;
        this.mapInfo = mapInfo;
    }

    public override void Run()
    {
        mapInfo.Add(info);
    }

    public override void Undo()
    {
        mapInfo.Remove(info);
    }
}
