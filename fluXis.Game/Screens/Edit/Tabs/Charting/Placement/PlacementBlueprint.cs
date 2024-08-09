using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Edit.Actions;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Placement;

public partial class PlacementBlueprint : Container
{
    [Resolved]
    protected EditorPlayfield Playfield { get; private set; }

    [Resolved]
    protected EditorActionStack Actions { get; private set; }

    [Resolved]
    protected EditorSettings Settings { get; private set; }

    [Resolved]
    protected EditorMap Map { get; private set; }

    public PlacementState State { get; set; }
    public ITimedObject Object { get; set; }

    protected PlacementBlueprint(ITimedObject obj)
    {
        Object = obj;
    }

    public virtual void UpdatePlacement(double time, int lane)
    {
        if (State == PlacementState.Waiting)
        {
            Object.Time = time;
        }
    }

    protected void BeginPlacement()
    {
        OnPlacementStarted();
        State = PlacementState.Working;
    }

    public void EndPlacement(bool commit)
    {
        switch (State)
        {
            case PlacementState.Completed:
                return;

            case PlacementState.Waiting:
                BeginPlacement();
                break;
        }

        OnPlacementFinished(commit);
        State = PlacementState.Completed;
    }

    public virtual void OnPlacementStarted()
    {
    }

    public virtual void OnPlacementFinished(bool commit)
    {
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        BeginPlacement();
        return true;
    }
}

public enum PlacementState
{
    Waiting,
    Working,
    Completed
}
