using fluXis.Game.Map.Structures;
using fluXis.Game.Utils;
using Humanizer;

namespace fluXis.Game.Screens.Edit.Actions.Events;

public class EventPlaceAction : EditorAction
{
    public override string Description => $"Add {name} at {TimeUtils.Format(obj.Time)}";

    private ITimedObject obj { get; }
    private string name { get; }

    public EventPlaceAction(ITimedObject obj)
    {
        this.obj = obj;
        name = obj.GetType().Name.Replace("Event", "").Humanize(LetterCasing.Title);
    }

    public override void Run(EditorMap map) => map.Add(obj);
    public override void Undo(EditorMap map) => map.Remove(obj);
}
