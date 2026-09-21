using System;
using System.Collections.Generic;
using fluXis.Map.Structures.Bases;
using fluXis.Mode.Keys.Objects;
using fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects.Hits;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Default.HitObject;
using osu.Framework.Graphics;

namespace fluXis.Mode.Keys.Editor.Objects;

public partial class EditorLongNote : EditorDrawableHitObject<LongNote>
{
    public override bool Visible
    {
        get
        {
            var inbound = EditorClock.CurrentTime >= Data.Time && EditorClock.CurrentTime <= Data.GetEndTime();
            if (inbound) return true;

            var start = base.Visible;
            var end = Math.Abs(EditorClock.CurrentTime - Data.GetEndTime()) <= 2000;

            return start || end;
        }
    }

    private Drawable head = null!;
    private Drawable body = null!;
    public Drawable End { get; private set; } = null!;

    public EditorLongNote(LongNote hit)
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

        var endY = Playfield.HitObjectContainer.PositionAtTime(Data.GetEndTime());
        body.Height = Y - endY;
        body.Y = -(End.Height / 2f);
        End.Y = endY - Y;
    }
}
