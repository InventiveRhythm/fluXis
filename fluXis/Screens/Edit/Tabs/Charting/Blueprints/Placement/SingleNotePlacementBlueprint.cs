using System.Linq;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;

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

    public override void UpdatePlacement(double time, int lane)
    {
        base.UpdatePlacement(time, lane);

        piece.Width = EditorHitObjectContainer.NOTEWIDTH;
        piece.Position = ToLocalSpace(PositionProvider.ScreenSpacePositionAtTime(time, lane));
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        base.OnMouseDown(e);
        FinishPlacement(true);
        return true;
    }

    protected override void OnPlacementFinished(bool commit)
    {
        if (Map.MapInfo.HitObjects.Any(h => (int)h.Time == (int)Hit.Time && h.Lane == Hit.Lane))
            return;

        base.OnPlacementFinished(commit);
    }
}
