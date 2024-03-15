using System;
using fluXis.Game.Map.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Placement.Effect;

public partial class LaneSwitchPlacementBlueprint : PlacementBlueprint
{
    private readonly BlueprintLongNoteBody body;
    private readonly BlueprintNotePiece head;
    private readonly BlueprintNotePiece end;

    private float originalStartTime;

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

        head.Position = ToLocalSpace(Playfield.HitObjectContainer.ScreenSpacePositionAtTime(ls.Time, 1));
        end.Position = ToLocalSpace(Playfield.HitObjectContainer.ScreenSpacePositionAtTime(ls.Time + ls.Speed, ls.Count + 1));
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

        EndPlacement(true);
    }

    public override void UpdatePlacement(float time, int lane)
    {
        base.UpdatePlacement(time, lane);
        if (Object is not LaneSwitchEvent ls) return;

        ls.Count = lane;

        if (State == PlacementState.Working)
        {
            ls.Time = time < originalStartTime ? time : originalStartTime;
            ls.Speed = Math.Abs(time - originalStartTime);
        }
        else originalStartTime = ls.Time = time;
    }

    public override void OnPlacementFinished(bool commit)
    {
        if (commit)
            Map.Add(Object);
    }
}
