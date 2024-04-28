using fluXis.Game.Map.Structures;

namespace fluXis.Game.Screens.Edit.Actions.Notes;

public class NoteRemoveAction : EditorAction
{
    public override string Description => $"Remove {infos.Length} note(s)";
    private HitObject[] infos { get; }

    public NoteRemoveAction(HitObject[] infos)
    {
        this.infos = infos;
    }

    public override void Run(EditorMap map)
    {
        foreach (var info in infos)
            map.Remove(info);
    }

    public override void Undo(EditorMap map)
    {
        foreach (var info in infos)
            map.Add(info);
    }
}
