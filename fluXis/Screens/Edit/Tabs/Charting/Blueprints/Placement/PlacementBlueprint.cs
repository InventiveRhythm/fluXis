using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Actions;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;

public partial class PlacementBlueprint : Container
{
    [Resolved]
    protected ChartingContainer ChartingContainer { get; private set; }

    [Resolved]
    protected EditorActionStack Actions { get; private set; }

    [Resolved]
    protected EditorSettings Settings { get; private set; }

    [Resolved]
    protected EditorMap Map { get; private set; }

    protected virtual ITimePositionProvider PositionProvider => ChartingContainer.Playfields[0];

    public PlacementState State { get; set; }
    public ITimedObject Object { get; }

    protected PlacementBlueprint(ITimedObject obj)
    {
        Object = obj;
    }

    protected void StartPlacement()
    {
        OnPlacementStarted();
        State = PlacementState.Placing;
    }

    public virtual void UpdatePlacement(double time, int lane)
    {
        if (State == PlacementState.Waiting)
            Object.Time = time;
    }

    public void FinishPlacement(bool commit)
    {
        switch (State)
        {
            case PlacementState.Completed:
                return;

            case PlacementState.Waiting:
                StartPlacement();
                break;
        }

        OnPlacementFinished(commit);
        State = PlacementState.Completed;
    }

    protected virtual void OnPlacementStarted()
    {
    }

    protected virtual void OnPlacementFinished(bool commit)
    {
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        StartPlacement();
        return true;
    }
}

public enum PlacementState
{
    Waiting,
    Placing,
    Completed
}
