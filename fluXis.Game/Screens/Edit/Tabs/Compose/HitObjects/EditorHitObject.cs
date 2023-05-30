using fluXis.Game.Map;
using fluXis.Game.Skinning.Default.HitObject;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Compose.HitObjects;

public partial class EditorHitObject : Container
{
    public override bool HandlePositionalInput => true;

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    public EditorPlayfield Playfield { get; }
    public HitObjectInfo Info { get; set; }

    public bool IsOnScreen
    {
        get
        {
            if (clock == null) return true;

            return clock.CurrentTime - Info.HoldEndTime <= 2000 && clock.CurrentTime >= Info.Time - 3000 / values.Zoom;
        }
    }

    public bool PlayedHitSound { get; set; }

    private DefaultHitObjectPiece notePiece;
    private DefaultHitObjectBody holdBody;
    private DefaultHitObjectEnd holdEnd;
    private Container outline;

    public EditorHitObject(EditorPlayfield playfield)
    {
        Playfield = playfield;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = EditorPlayfield.COLUMN_WIDTH;
        AutoSizeAxes = Axes.Y;
        Anchor = Anchor.BottomLeft;
        Origin = Anchor.BottomLeft;

        InternalChildren = new Drawable[]
        {
            notePiece = new DefaultHitObjectPiece(),
            holdBody = new DefaultHitObjectBody(),
            holdEnd = new DefaultHitObjectEnd(),
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
                        BorderColour = Colour4.Yellow,
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

        UpdateColors();
    }

    protected override void LoadComplete()
    {
        notePiece.Height /= 2;
        notePiece.CornerRadius = 5;
        holdEnd.Height /= 2;
        holdEnd.CornerRadius = 5;

        base.LoadComplete();
    }

    protected override void Update()
    {
        X = (Info.Lane - 1) * EditorPlayfield.COLUMN_WIDTH;
        Y = -EditorPlayfield.HITPOSITION_Y - .5f * ((Info.Time - (float)clock.CurrentTime) * values.Zoom);

        if (Info.Time >= clock.CurrentTime) PlayedHitSound = false;

        if (Info.IsLongNote())
        {
            holdBody.Height = .5f * (Info.HoldTime * values.Zoom);
            holdBody.Y = -notePiece.Height / 2;
            holdEnd.Y = -.5f * (Info.HoldTime * values.Zoom);
            holdBody.Alpha = 1;
            holdEnd.Alpha = 1;
        }
        else
        {
            holdBody.Height = 0;
            holdEnd.Y = notePiece.Y;
            holdBody.Alpha = 0;
            holdEnd.Alpha = 0;
        }

        outline.Width = DrawWidth + 20;
        outline.Height = DrawHeight + 20;

        base.Update();
    }

    public void UpdateSelection(bool selected)
    {
        outline.FadeTo(selected ? 1 : 0, 200);
    }

    public void UpdateColors()
    {
        notePiece.UpdateColor(Info.Lane, Playfield.Map.KeyCount);
        holdBody.UpdateColor(Info.Lane, Playfield.Map.KeyCount);
        holdEnd.UpdateColor(Info.Lane, Playfield.Map.KeyCount);
    }
}
