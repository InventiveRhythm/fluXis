using System;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Actions.Generic;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;

#nullable enable

public partial class ObjectPlacementBlueprint<T> : PlacementBlueprint
    where T : ITimedObject
{
    protected new T Object => (T)base.Object;
    protected bool HasDuration => Object is IHasDuration;

    private readonly BlueprintNotePiece piece;
    private readonly BlueprintLongNoteBody body;
    private readonly BlueprintNotePiece end;

    private double originalStartTime;

    public ObjectPlacementBlueprint(T obj)
        : base(obj)
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            body = new BlueprintLongNoteBody
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.BottomLeft,
                Alpha = HasDuration ? 0.5f : 0f
            },
            piece = new BlueprintNotePiece
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.BottomLeft
            },
            end = new BlueprintNotePiece
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.BottomLeft,
                Alpha = HasDuration ? 1f : 0f
            }
        };
    }

    public override void UpdatePlacement(double time, int lane)
    {
        base.UpdatePlacement(time, lane);

        piece.Position = ToLocalSpace(PositionProvider.ScreenSpacePositionAtTime(Object.Time, Object.Lane));

        if (Object is not IHasDuration d)
            return;

        if (State == PlacementState.Placing)
        {
            d.Time = time < originalStartTime ? time : originalStartTime;
            d.Duration = Math.Abs(time - originalStartTime);
        }
        else originalStartTime = d.Time = time;

        end.Position = ToLocalSpace(PositionProvider.ScreenSpacePositionAtTime(d.GetEndTime(), Object.Lane));
        body.Height = Math.Abs(piece.Y - end.Y);
        body.Position = new Vector2(piece.X, piece.Y - piece.DrawHeight / 2);
    }

    protected override void Update()
    {
        base.Update();

        piece.Width = EditorHitObjectContainer.NOTEWIDTH * Settings.ObjectZoom;
        body.Width = EditorHitObjectContainer.NOTEWIDTH * Settings.ObjectZoom;
        end.Width = EditorHitObjectContainer.NOTEWIDTH * Settings.ObjectZoom;
    }

    protected override void OnPlacementFinished(bool commit)
    {
        if (!commit)
            return;

        Actions.Add(new ObjectPlaceAction<T>(Object));
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (!base.OnMouseDown(e))
            return false;

        if (!HasDuration)
            FinishPlacement(true);

        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.ShiftPressed) return;
        if (e.Button != MouseButton.Left)
            return;

        if (HasDuration)
            FinishPlacement(true);
    }
}
