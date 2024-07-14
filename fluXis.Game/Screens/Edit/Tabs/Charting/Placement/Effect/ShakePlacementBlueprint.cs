using System;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Actions.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Placement.Effect;

public partial class ShakePlacementBlueprint : PlacementBlueprint
{
    private readonly BlueprintLongNoteBody body;
    private readonly BlueprintNotePiece head;
    private readonly BlueprintNotePiece end;

    private double originalStartTime;

    public ShakePlacementBlueprint()
        : base(new ShakeEvent())
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

        if (Object is not ShakeEvent shake) return;

        head.Position = ToLocalSpace(Playfield.HitObjectContainer.ScreenSpacePositionAtTime(shake.Time, 1));
        end.Position = ToLocalSpace(Playfield.HitObjectContainer.ScreenSpacePositionAtTime(shake.Time + shake.Duration, 1));
        body.Height = Math.Abs(head.Y - end.Y);
        body.Position = new Vector2(head.X, head.Y - head.DrawHeight / 2);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        EndPlacement(true);
    }

    public override void UpdatePlacement(double time, int lane)
    {
        base.UpdatePlacement(time, lane);
        if (Object is not ShakeEvent shake) return;

        if (State == PlacementState.Working)
        {
            shake.Time = time < originalStartTime ? time : originalStartTime;
            shake.Duration = Math.Abs(time - originalStartTime);
        }
        else originalStartTime = shake.Time = time;
    }

    public override void OnPlacementFinished(bool commit)
    {
        if (commit)
            Actions.Add(new EventPlaceAction(Object));
    }
}

