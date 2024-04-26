using fluXis.Game.Map.Structures;

namespace fluXis.Game.Screens.Edit.Actions.Notes;

public class NotePasteAction : EditorAction
{
    public override string Description => $"Paste {infos.Length} note(s)";

    private HitObject[] infos { get; }
    private EditorMap map { get; }

    public NotePasteAction(HitObject[] infos, EditorMap map)
    {
        this.infos = infos;
        this.map = map;
    }

    public override void Run()
    {
        foreach (var info in infos)
            map.Add(info);
    }

    public override void Undo()
    {
        foreach (var info in infos)
            map.Remove(info);
    }
}
