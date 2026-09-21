using System;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Mode.Keys.Objects;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Mode.Keys.Editor.Blueprint;

public partial class TickNotePlacementBlueprint : NotePlacementBlueprint<Tick>
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
        if (Map.Playable.ObjectsOfType<HitObject>().Where(x => x.Lane == Hit.Lane).Any(x => Math.Abs(x.Time - Hit.Time) < 10))
            return;

        base.OnPlacementFinished(commit);
    }
}
