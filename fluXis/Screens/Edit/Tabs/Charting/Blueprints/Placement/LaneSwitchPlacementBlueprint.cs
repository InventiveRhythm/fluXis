using System;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Actions.Events;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;

public partial class LaneSwitchPlacementBlueprint : PlacementBlueprint
{
    private readonly BlueprintLongNoteBody body;
    private readonly BlueprintNotePiece head;
    private readonly BlueprintNotePiece end;

    private double originalStartTime;

    public LaneSwitchPlacementBlueprint()
        : base(new LaneSwitchEvent())
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

        if (Object is not LaneSwitchEvent ls) return;

        ls.Count = Math.Clamp(ls.Count, 1, Map.RealmMap.KeyCount);

        head.Position = ToLocalSpace(PositionProvider.ScreenSpacePositionAtTime(ls.Time, 1));
        end.Position = ToLocalSpace(PositionProvider.ScreenSpacePositionAtTime(ls.Time + ls.Duration, ls.Count + 1));
        body.Height = Math.Abs(head.Y - end.Y);
        body.Position = new Vector2(head.X, head.Y - head.DrawHeight / 2);

        var endPos = end.Position;
        var width = endPos.X - head.X;

        head.Width = width;
        body.Width = width;
        end.Width = width;
        end.X = head.X;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        FinishPlacement(true);
    }

    public override void UpdatePlacement(double time, int lane)
    {
        base.UpdatePlacement(time, lane);
        if (Object is not LaneSwitchEvent ls) return;

        ls.Count = lane;

        if (State == PlacementState.Placing)
        {
            ls.Time = time < originalStartTime ? time : originalStartTime;
            ls.Duration = Math.Abs(time - originalStartTime);
        }
        else originalStartTime = ls.Time = time;
    }

    protected override void OnPlacementFinished(bool commit)
    {
        if (commit)
            Actions.Add(new EventPlaceAction(Object));
    }
}
