using System.Linq;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Placement;

public partial class SingleNotePlacementBlueprint : NotePlacementBlueprint
{
    private readonly BlueprintNotePiece piece;

    public SingleNotePlacementBlueprint()
    {
        RelativeSizeAxes = Axes.Both;
        InternalChild = piece = new BlueprintNotePiece
        {
            Anchor = Anchor.TopLeft,
            Origin = Anchor.BottomLeft
        };
    }

    public override void UpdatePlacement(float time, int lane)
    {
        base.UpdatePlacement(time, lane);

        piece.Width = EditorHitObjectContainer.NOTEWIDTH;
        piece.Position = ToLocalSpace(Playfield.HitObjectContainer.ScreenSpacePositionAtTime(time, lane));
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        base.OnMouseDown(e);
        EndPlacement(true);
        return true;
    }

    public override void OnPlacementFinished(bool commit)
    {
        if (Map.MapInfo.HitObjects.Any(h => (int)h.Time == (int)Hit.Time && h.Lane == Hit.Lane))
            return;

        base.OnPlacementFinished(commit);
    }
}
