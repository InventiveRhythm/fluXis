using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Selection;

public class MoveSelectionEvent
{
    public SelectionBlueprint Blueprint { get; }
    public Vector2 Delta { get; }

    public MoveSelectionEvent(SelectionBlueprint blueprint, Vector2 delta)
    {
        Blueprint = blueprint;
        Delta = delta;
    }
}
