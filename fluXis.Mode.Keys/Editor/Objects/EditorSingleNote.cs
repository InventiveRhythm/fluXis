using System.Collections.Generic;
using fluXis.Mode.Keys.Objects;
using fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects.Hits;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Default.HitObject;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Mode.Keys.Editor.Objects;

public partial class EditorSingleNote : EditorDrawableHitObject<Note>
{
    private Drawable piece = null!;

    public EditorSingleNote(Note hit)
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
