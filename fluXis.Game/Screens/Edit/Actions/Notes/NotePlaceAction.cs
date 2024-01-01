using fluXis.Game.Map.Structures;

namespace fluXis.Game.Screens.Edit.Actions.Notes;

public class NotePlaceAction : EditorAction
{
    private readonly HitObject info;
    private readonly EditorMapInfo mapInfo;

    public NotePlaceAction(HitObject info, EditorMapInfo mapInfo)
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
