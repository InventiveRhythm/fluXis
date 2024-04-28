using fluXis.Game.Map.Structures;

namespace fluXis.Game.Screens.Edit.Actions.Notes;

public class NotePlaceAction : EditorAction
{
    public override string Description => $"Place note at {(int)info.Time}ms on lane {info.Lane}";
    private HitObject info { get; }

    public NotePlaceAction(HitObject info)
    {
        this.info = info;
    }

    public override void Run(EditorMap map) => map.Add(info);
    public override void Undo(EditorMap map) => map.Remove(info);
}
