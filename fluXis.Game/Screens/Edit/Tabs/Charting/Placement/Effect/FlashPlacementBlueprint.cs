using System;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Placement.Effect;

public partial class FlashPlacementBlueprint : PlacementBlueprint
{
    private readonly BlueprintLongNoteBody body;
    private readonly BlueprintNotePiece head;
    private readonly BlueprintNotePiece end;

    private float originalStartTime;

    public FlashPlacementBlueprint()
        : base(new FlashEvent())
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            body = new BlueprintLongNoteBody
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.BottomLeft
            },
            head = new BlueprintNotePiece
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.BottomLeft
            },
            end = new BlueprintNotePiece
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.BottomLeft
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        if (Parent == null) return;

        head.Width = Playfield.DrawWidth;
        body.Width = Playfield.DrawWidth;
        end.Width = Playfield.DrawWidth;

        if (Object is not FlashEvent flash) return;

        head.Position = ToLocalSpace(Playfield.HitObjectContainer.ScreenSpacePositionAtTime(flash.Time, 1));
        end.Position = ToLocalSpace(Playfield.HitObjectContainer.ScreenSpacePositionAtTime(flash.Time + flash.Duration, 1));
        body.Height = Math.Abs(head.Y - end.Y);
        body.Position = new Vector2(head.X, head.Y - head.DrawHeight / 2);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        EndPlacement(true);
    }

    public override void UpdatePlacement(float time, int lane)
    {
        base.UpdatePlacement(time, lane);
        if (Object is not FlashEvent flash) return;

        if (State == PlacementState.Working)
        {
            flash.Time = time < originalStartTime ? time : originalStartTime;
            flash.Duration = Math.Abs(time - originalStartTime);
        }
        else originalStartTime = flash.Time = time;
    }

    public override void OnPlacementFinished(bool commit)
    {
        if (commit) EditorValues.MapEvents.Add(Object as FlashEvent);
    }
}
