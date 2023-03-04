using fluXis.Game.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.Lines;

public partial class EditorTimingLine : Box
{
    public EditorPlayfield Playfield { get; set; }

    public new float Time { get; set; }

    public EditorTimingLine(EditorPlayfield playfield)
    {
        Playfield = playfield;

        RelativeSizeAxes = Axes.X;
        Height = 1;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
    }

    protected override void Update()
    {
        Y = -EditorPlayfield.HITPOSITION_Y - .5f * ((Time - Conductor.CurrentTime) * Playfield.Zoom);

        base.Update();
    }
}
