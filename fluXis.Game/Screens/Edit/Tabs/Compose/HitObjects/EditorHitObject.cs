using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.HitObjects;

public partial class EditorHitObject : Container
{
    public EditorPlayfield Playfield { get; }

    public HitObjectInfo Info { get; set; }

    public bool IsOnScreen => Conductor.CurrentTime <= Info.HoldEndTime && Conductor.CurrentTime >= Info.Time - 3000 / Playfield.Zoom;

    private readonly Box note;
    private readonly Box holdBody;
    private readonly Container outline;

    public EditorHitObject(EditorPlayfield playfield)
    {
        Playfield = playfield;

        Width = EditorPlayfield.COLUMN_WIDTH;
        AutoSizeAxes = Axes.Y;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        InternalChildren = new Drawable[]
        {
            note = new Box
            {
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Height = 20
            },
            holdBody = new Box
            {
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Width = .9f,
                Height = 0,
                Y = -20
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    outline = new Container
                    {
                        Masking = true,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        CornerRadius = 5,
                        BorderThickness = 6,
                        BorderColour = FluXisColors.Accent3,
                        Alpha = 0,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0,
                            AlwaysPresent = true
                        }
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        X = (Info.Lane - 1) * EditorPlayfield.COLUMN_WIDTH;
        Y = -EditorPlayfield.HITPOSITION_Y - .5f * ((Info.Time - Conductor.CurrentTime) * Playfield.Zoom);

        if (Info.IsLongNote())
        {
            if (Info.Time < Conductor.CurrentTime)
            {
                Y = -EditorPlayfield.HITPOSITION_Y;
                holdBody.Height = .5f * ((Info.HoldEndTime - Conductor.CurrentTime) * Playfield.Zoom);
            }
            else
                holdBody.Height = .5f * ((Info.HoldEndTime - Info.Time) * Playfield.Zoom);
        }
        else holdBody.Height = 0;

        outline.Width = DrawWidth + 20;
        outline.Height = DrawHeight + 20;

        base.Update();
    }

    public void UpdateSelection(bool selected)
    {
        outline.FadeTo(selected ? 1 : 0, 200);
    }
}
