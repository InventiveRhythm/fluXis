using System.Collections.Generic;
using fluXis.Map.Structures;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Default.HitObject;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects;

public partial class EditorSingleNote : EditorHitObject
{
    private Drawable piece;

    public EditorSingleNote(HitObject hit)
        : base(hit)
    {
    }

    protected override IEnumerable<Drawable> CreateContent() => (piece = new DefaultHitObjectPiece(null, 0).With(h =>
    {
        h.RelativeSizeAxes = Axes.X;
        h.Anchor = Anchor.BottomCentre;
        h.Origin = Anchor.BottomCentre;
    })).Yield();

    protected override void LoadComplete()
    {
        (piece as ColorableSkinDrawable)?.UpdateColor(0, 0);

        base.LoadComplete();
    }
}
