using System;
using System.Linq;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;

public partial class TickNotePlacementBlueprint : NotePlacementBlueprint
{
    public override bool AllowPainting => true;
    private readonly BlueprintNotePiece piece;

    public TickNotePlacementBlueprint()
    {
        RelativeSizeAxes = Axes.Both;
        InternalChild = piece = new BlueprintNotePiece
        {
            Anchor = Anchor.TopLeft,
            Origin = Anchor.BottomLeft
        };

        Hit.Type = 1;
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
        if (Map.MapInfo.HitObjects.Where(x => x.Lane == Hit.Lane).Any(x => Math.Abs(x.Time - Hit.Time) < 10))
            return;

        base.OnPlacementFinished(commit);
    }
}
