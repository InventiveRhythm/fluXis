using System;
using fluXis.Online.API.Models.Maps.Modding;
using fluXis.Screens.Edit.Modding;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Modding;

public partial class EditorChangeRequestBlueprint : PlacementBlueprint
{
    private readonly BlueprintLongNoteBody body;
    private readonly BlueprintNotePiece head;
    private readonly BlueprintNotePiece end;

    private double originalStartTime;

    public EditorChangeRequestBlueprint(EditorModding modding)
        : base(new EditorChangeRequest(modding.CurrentAction, new APIModdingChangeRequest()))
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
        if (Object is not EditorChangeRequest req) return;

        head.Position = ToLocalSpace(PositionProvider.ScreenSpacePositionAtTime(req.Time, 1));
        end.Position = ToLocalSpace(PositionProvider.ScreenSpacePositionAtTime(req.Time + req.Duration, Map.RealmMap.KeyCount + 1));
        body.Height = Math.Abs(head.Y - end.Y);
        body.Position = new Vector2(head.X, head.Y - head.DrawHeight / 2);

        var width = Math.Abs(head.X - end.X);
        head.Width = body.Width = end.Width = width;
        end.X = head.X;
    }

    public override void UpdatePlacement(double time, int lane)
    {
        base.UpdatePlacement(time, lane);
        if (Object is not EditorChangeRequest req) return;

        if (State == PlacementState.Placing)
        {
            req.Time = time < originalStartTime ? time : originalStartTime;
            req.Duration = Math.Abs(time - originalStartTime);
        }
        else originalStartTime = req.Time = time;
    }

    protected override void OnPlacementFinished(bool commit)
    {
        if (!commit)
            return;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.Button != MouseButton.Left)
            return;

        FinishPlacement(true);
    }
}
