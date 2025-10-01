using System.Collections.Generic;
using fluXis.Map.Structures;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Default.HitObject;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects;

public partial class EditorSingleNote : EditorHitObject
{
    private Drawable piece;

    public EditorSingleNote(HitObject hit)
        : base(hit)
    {
    }

    protected override IEnumerable<Drawable> CreateContent()
    {
        yield return piece = new DefaultHitObjectPiece(null, 0);
    }

    protected override void LoadComplete()
    {
        (piece as ColorableSkinDrawable)?.UpdateColor(0, 0);

        base.LoadComplete();
    }
}
