using System;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Skinning.Default.HitObject;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects.Events;

public partial class EditorDrawableEvent : EditorDrawableObject
{
    public override bool Visible
    {
        get
        {
            if (Data is not IHasDuration d)
                return base.Visible;

            var inbound = EditorClock.CurrentTime >= Data.Time && EditorClock.CurrentTime <= d.GetEndTime();
            if (inbound) return true;

            return base.Visible || Math.Abs(EditorClock.CurrentTime - d.GetEndTime()) <= 2000;
        }
    }

    private readonly DefaultHitObjectPiece head;
    private readonly DefaultHitObjectBody body;
    private readonly DefaultHitObjectEnd end;

    public EditorDrawableEvent(ITimedObject hit)
        : base(hit)
    {
        InternalChildren =
        [
            body = new DefaultHitObjectBody(null, 0).With(b =>
            {
                b.RelativeSizeAxes = Axes.X;
                b.Anchor = Anchor.BottomCentre;
                b.Origin = Anchor.BottomCentre;
            }),
            head = new DefaultHitObjectPiece(null, 0).With(h =>
            {
                h.RelativeSizeAxes = Axes.X;
                h.Anchor = Anchor.BottomCentre;
                h.Origin = Anchor.BottomCentre;
            }),
            end = new DefaultHitObjectEnd(null, 0).With(e =>
            {
                e.RelativeSizeAxes = Axes.X;
                e.Anchor = Anchor.BottomCentre;
                e.Origin = Anchor.BottomCentre;
            }),
        ];
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        var color = Theme.GetEventColor(Data);
        head.SetColor(color);
        body.SetColor(color);
        end.SetColor(color);
    }

    protected override void Update()
    {
        base.Update();

        if (Data is not IHasDuration d || d.Duration <= 0)
        {
            body.Alpha = end.Alpha = 0;
            return;
        }

        body.Alpha = end.Alpha = 1;

        var endY = Playfield.HitObjectContainer.PositionAtTime(d.GetEndTime());
        body.Height = Y - endY - end.Height + 4;
        body.Y = -end.Height + 2;
        end.Y = endY - Y;
    }
}
