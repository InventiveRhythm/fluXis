using fluXis.Map.Structures;

namespace fluXis.Screens.Edit.Actions.Notes;

public class NotePasteAction : EditorAction
{
    public override string Description => $"Paste {infos.Length} note(s)";

    private HitObject[] infos { get; }

    public NotePasteAction(HitObject[] infos)
    {
        this.infos = infos;
    }

    public override void Run(EditorMap map)
    {
        foreach (var info in infos)
            map.Add(info);
    }

    public override void Undo(EditorMap map)
    {
        foreach (var info in infos)
            map.Remove(info);
    }
}
