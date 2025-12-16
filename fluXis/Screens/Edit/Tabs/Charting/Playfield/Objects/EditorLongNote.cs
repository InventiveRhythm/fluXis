using System;
using System.Collections.Generic;
using fluXis.Map.Structures;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Default.HitObject;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects;

public partial class EditorLongNote : EditorHitObject
{
    public override bool Visible
    {
        get
        {
            var inbound = EditorClock.CurrentTime >= Data.Time && EditorClock.CurrentTime <= Data.EndTime;
            if (inbound) return true;

            var start = base.Visible;
            var end = Math.Abs(EditorClock.CurrentTime - Data.EndTime) <= 2000;

            return start || end;
        }
    }

    private Drawable head;
    private Drawable body;
    public Drawable End { get; private set; }

    public EditorLongNote(HitObject hit)
        : base(hit)
    {
    }

    protected override IEnumerable<Drawable> CreateContent() => new[]
    {
        head = new DefaultHitObjectPiece(null, 0).With(h =>
        {
            h.RelativeSizeAxes = Axes.X;
            h.Anchor = Anchor.BottomCentre;
            h.Origin = Anchor.BottomCentre;
        }),
        body = new DefaultHitObjectBody(null, 0).With(b =>
        {
            b.RelativeSizeAxes = Axes.X;
            b.Anchor = Anchor.BottomCentre;
            b.Origin = Anchor.BottomCentre;
        }),
        End = new DefaultHitObjectEnd(null, 0).With(e =>
        {
            e.RelativeSizeAxes = Axes.X;
            e.Anchor = Anchor.BottomCentre;
            e.Origin = Anchor.BottomCentre;
        }),
    };

    protected override void LoadComplete()
    {
        base.LoadComplete();

        (head as ColorableSkinDrawable)?.UpdateColor(0, 0);
        (body as ColorableSkinDrawable)?.UpdateColor(0, 0);
        (End as ColorableSkinDrawable)?.UpdateColor(0, 0);
    }

    protected override void Update()
    {
        base.Update();

        var endY = Playfield.HitObjectContainer.PositionAtTime(Data.EndTime);
        body.Height = Y - endY - End.Height + 4;
        body.Y = -End.Height + 2;
        End.Y = endY - Y;
    }
}
