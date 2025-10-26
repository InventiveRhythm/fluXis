using System.Collections.Generic;
using fluXis.Map.Structures;
using fluXis.Skinning.Default.HitObject;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects;

public partial class EditorLandmine : EditorHitObject
{
    private Drawable landminePiece;

    public EditorLandmine(HitObject hit)
        : base(hit)
    {
    }

    protected override IEnumerable<Drawable> CreateContent() => new[]
    {
        landminePiece = new DefaultLandmine()
    };

    protected override void Update()
    {
        base.Update();

        landminePiece.Alpha = Data.Hidden ? 0.3f : 1f;
    }
}
