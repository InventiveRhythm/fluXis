using fluXis.Game.Map.Structures;

namespace fluXis.Game.Screens.Edit.Actions.Notes;

public class NoteRemoveAction : EditorAction
{
    public override string Description => $"Remove {infos.Length} note(s)";
    private HitObject[] infos { get; }
    private EditorMap map { get; }

    public NoteRemoveAction(HitObject[] infos, EditorMap map)
    {
        this.infos = infos;
        this.map = map;
    }

    public override void Run()
    {
        foreach (var info in infos)
            map.Remove(info);
    }

    public override void Undo()
    {
        foreach (var info in infos)
            map.Add(info);
    }
}
