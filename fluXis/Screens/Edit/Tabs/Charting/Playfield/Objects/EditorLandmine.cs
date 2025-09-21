using System.Collections.Generic;
using fluXis.Map.Structures;
using fluXis.Skinning.Default.HitObject;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects;

public partial class EditorLandmine : EditorHitObject
{
    private Drawable landminePiece;
    private Drawable landmineGhost;

    public EditorLandmine(HitObject hit)
        : base(hit)
    {
    }

    protected override IEnumerable<Drawable> CreateContent() => new[]
    {
        landmineGhost = new DefaultLandmine(false).With(d => d.Alpha = .2f),
        landminePiece = new DefaultLandmine(false)
    };

    protected override void Update()
    {
        base.Update();

        landminePiece.Width = Data.HoldTime > 0 ? 0.8f : 1f;

        var l = Data.VisualLane == 0 ? Data.Lane : Data.VisualLane;
        landmineGhost.X = Playfield.HitObjectContainer.PositionFromLane(l) - X;
    }
}
