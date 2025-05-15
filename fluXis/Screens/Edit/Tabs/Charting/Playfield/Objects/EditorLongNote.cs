using System.Collections.Generic;
using fluXis.Map.Structures;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Default.HitObject;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects;

public partial class EditorLongNote : EditorHitObject
{
    private Drawable head;
    private Drawable body;
    public Drawable End { get; private set; }

    public EditorLongNote(HitObject hit)
        : base(hit)
    {
    }

    protected override IEnumerable<Drawable> CreateContent() => new[]
    {
        head = new DefaultHitObjectPiece(null),
        body = new DefaultHitObjectBody(null).With(b =>
        {
            b.Anchor = Anchor.BottomCentre;
            b.Origin = Anchor.BottomCentre;
        }),
        End = new DefaultHitObjectEnd(null).With(e =>
        {
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
