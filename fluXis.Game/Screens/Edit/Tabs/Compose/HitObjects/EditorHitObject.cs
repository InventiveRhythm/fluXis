using fluXis.Game.Audio;
using fluXis.Game.Map;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.HitObjects;

public partial class EditorHitObject : Container
{
    public EditorPlayfield Playfield { get; }

    public HitObjectInfo Info { get; set; }

    public bool IsOnScreen => Conductor.CurrentTime <= Info.HoldEndTime;

    private readonly Box note;
    private readonly Box holdBody;

    public EditorHitObject(EditorPlayfield playfield)
    {
        Playfield = playfield;

        Width = EditorPlayfield.COLUMN_WIDTH;
        Height = 20;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        Add(note = new Box
        {
            RelativeSizeAxes = Axes.Both
        });

        Add(holdBody = new Box
        {
            RelativeSizeAxes = Axes.X,
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            Width = .9f,
            Height = 0
        });
    }

    protected override void Update()
    {
        X = (Info.Lane - 1) * EditorPlayfield.COLUMN_WIDTH;
        note.Y = -EditorPlayfield.HITPOSITION_Y - .5f * ((Info.Time - Conductor.CurrentTime) * Playfield.Zoom);

        if (Info.IsLongNote())
        {
            if (Info.Time < Conductor.CurrentTime)
                note.Y = -EditorPlayfield.HITPOSITION_Y;

            holdBody.Y = -EditorPlayfield.HITPOSITION_Y - .5f * ((Info.HoldEndTime - Conductor.CurrentTime) * Playfield.Zoom);
            holdBody.Height = note.Y - holdBody.Y;
        }
        else holdBody.Height = 0;

        base.Update();
    }
}
