using System;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Placement;

public partial class LongNotePlacementBlueprint : PlacementBlueprint
{
    private readonly BlueprintLongNoteBody body;
    private readonly BlueprintNotePiece head;
    private readonly BlueprintNotePiece end;

    private float originalStartTime;

    public LongNotePlacementBlueprint()
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

        head.Width = EditorHitObjectContainer.NOTEWIDTH;
        body.Width = EditorHitObjectContainer.NOTEWIDTH;
        end.Width = EditorHitObjectContainer.NOTEWIDTH;

        head.Position = ToLocalSpace(Playfield.HitObjectContainer.ScreenSpacePositionAtTime(HitObject.Time, HitObject.Lane));
        end.Position = ToLocalSpace(Playfield.HitObjectContainer.ScreenSpacePositionAtTime(HitObject.HoldEndTime, HitObject.Lane));
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

        if (State == PlacementState.Working)
        {
            HitObject.Time = time < originalStartTime ? time : originalStartTime;
            HitObject.HoldTime = Math.Abs(time - originalStartTime);
        }
        else
        {
            /*if (result.Playfield != null)
            {
                head.Width = end.Width = result.Playfield.DrawWidth;
                head.X = end.X = ToLocalSpace(result.ScreenSpacePosition).X;
            }*/

            originalStartTime = HitObject.Time = time;
        }
    }
}
