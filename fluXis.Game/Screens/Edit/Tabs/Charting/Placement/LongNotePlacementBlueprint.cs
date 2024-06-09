using System;
using System.Linq;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Placement;

public partial class LongNotePlacementBlueprint : NotePlacementBlueprint
{
    private readonly BlueprintLongNoteBody body;
    private readonly BlueprintNotePiece head;
    private readonly BlueprintNotePiece end;

    private double originalStartTime;

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

        if (Object is not HitObject hit) return;

        head.Position = ToLocalSpace(Playfield.HitObjectContainer.ScreenSpacePositionAtTime(hit.Time, hit.Lane));
        end.Position = ToLocalSpace(Playfield.HitObjectContainer.ScreenSpacePositionAtTime(hit.EndTime, hit.Lane));
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
        if (Object is not HitObject hit) return;

        if (State == PlacementState.Working)
        {
            hit.Time = time < originalStartTime ? time : originalStartTime;
            hit.HoldTime = Math.Abs(time - originalStartTime);
        }
        else originalStartTime = hit.Time = time;
    }

    public override void OnPlacementFinished(bool commit)
    {
        var notesBetween = Map.MapInfo.HitObjects.Where(h => h.Time > Hit.Time && h.Time < Hit.EndTime && h.Lane == Hit.Lane).ToList();

        if (notesBetween.Count > 0)
        {
            foreach (var note in notesBetween)
                Map.Remove(note);
        }

        base.OnPlacementFinished(commit);
    }
}
