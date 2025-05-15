using System.Collections.Generic;
using fluXis.Map.Structures;
using fluXis.Skinning.Default.HitObject;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects;

public partial class EditorTickNote : EditorHitObject
{
    private Drawable tickNotePiece;
    private Drawable tickNoteGhost;

    public EditorTickNote(HitObject hit)
        : base(hit)
    {
    }

    protected override IEnumerable<Drawable> CreateContent() => new[]
    {
        tickNoteGhost = new DefaultTickNote(false).With(d => d.Alpha = .2f),
        tickNotePiece = new DefaultTickNote(false)
    };

    protected override void Update()
    {
        base.Update();

        tickNotePiece.Width = Data.HoldTime > 0 ? 0.8f : 1f;

        var l = Data.VisualLane == 0 ? Data.Lane : Data.VisualLane;
        tickNoteGhost.X = Playfield.HitObjectContainer.PositionFromLane(l) - X;
    }
}
