using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Placement;

public partial class PlacementBlueprint : Container
{
    [Resolved]
    protected EditorPlayfield Playfield { get; set; }

    public PlacementState State { get; set; }
    public HitObjectInfo HitObject { get; set; } = new();

    public virtual void UpdatePlacement(float time, int lane)
    {
        if (State == PlacementState.Waiting)
        {
            HitObject.Time = time;
            HitObject.Lane = lane;
        }
    }

    protected void BeginPlacement()
    {
        Playfield.StartPlacement(HitObject);
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

        Playfield.FinishPlacement(HitObject, commit);
        State = PlacementState.Completed;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        BeginPlacement();
        return true;
    }

    public enum PlacementState
    {
        Waiting,
        Working,
        Completed
    }
}
