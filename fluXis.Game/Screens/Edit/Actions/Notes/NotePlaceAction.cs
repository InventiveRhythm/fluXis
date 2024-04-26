using fluXis.Game.Map.Structures;

namespace fluXis.Game.Screens.Edit.Actions.Notes;

public class NotePlaceAction : EditorAction
{
    public override string Description => $"Place note at {(int)info.Time}ms on lane {info.Lane}";
    private HitObject info { get; }
    private EditorMap map { get; }

    public NotePlaceAction(HitObject info, EditorMap map)
    {
        this.info = info;
        this.map = map;
    }

    public override void Run() => map.Add(info);
    public override void Undo() => map.Remove(info);
}
