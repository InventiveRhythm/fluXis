using System.Collections.Generic;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Mode.Keys.Objects;
using fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects.Hits;
using fluXis.Skinning.Default.HitObject;
using osu.Framework.Graphics;

namespace fluXis.Mode.Keys.Editor.Objects;

public partial class EditorLandmine : EditorDrawableHitObject<Landmine>
{
    public override Colour4 TextColor => Theme.Text;
    private Drawable landminePiece = null!;

    public EditorLandmine(Landmine hit)
        : base(hit)
    {
    }

    protected override IEnumerable<Drawable> CreateContent()
        => [landminePiece = new DefaultLandmine().With(d => d.RelativeSizeAxes = Axes.X)];

    protected override void Update()
    {
        base.Update();

        landminePiece.Alpha = Data.Hidden ? 0.3f : 1f;
    }
}
