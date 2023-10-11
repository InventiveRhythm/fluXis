using fluXis.Game.Map;

namespace fluXis.Game.Screens.Edit.Actions.Notes;

public class NoteRemoveAction : EditorAction
{
    private readonly HitObjectInfo[] infos;
    private readonly EditorMapInfo mapInfo;

    public NoteRemoveAction(HitObjectInfo[] infos, EditorMapInfo mapInfo)
    {
        this.infos = infos;
        this.mapInfo = mapInfo;
    }

    public override void Run()
    {
        foreach (var info in infos)
        {
            mapInfo.Remove(info);
        }
    }

    public override void Undo()
    {
        foreach (var info in infos)
        {
            mapInfo.Add(info);
        }
    }
}
