using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures;
using fluXis.Game.Skinning.Default.HitObject;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;

public partial class EditorHitObject : Container
{
    [Resolved]
    private EditorPlayfield playfield { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorValues values { get; set; }

    public HitObject Data { get; init; }

    public Drawable HitObjectPiece { get; private set; }
    private Drawable longNoteBody { get; set; }
    public Drawable LongNoteEnd { get; private set; }

    private FluXisSpriteText text { get; set; }

    private bool overZero = true;
    private const int max_distance = 100;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = EditorHitObjectContainer.NOTEWIDTH;
        AutoSizeAxes = Axes.Y;
        Origin = Anchor.BottomLeft;

        Children = new[]
        {
            HitObjectPiece = new DefaultHitObjectPiece(null),
            longNoteBody = new DefaultHitObjectBody(null).With(b =>
            {
                b.Anchor = Anchor.BottomCentre;
                b.Origin = Anchor.BottomCentre;
            }),
            LongNoteEnd = new DefaultHitObjectEnd(null).With(e =>
            {
                e.Anchor = Anchor.BottomCentre;
                e.Origin = Anchor.BottomCentre;
            }),
            new Container
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Height = 36,
                Child = text = new FluXisSpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = FluXisColors.Background2,
                    Alpha = 0
                }
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        text.Text = Data.HitSound?.Replace(".wav", "") ?? "";
        text.Alpha = values.ShowSamples.Value ? 1 : 0;

        X = playfield.HitObjectContainer.PositionFromLane(Data.Lane);
        Y = playfield.HitObjectContainer.PositionAtTime(Data.Time);

        if (Data.LongNote)
        {
            var endY = playfield.HitObjectContainer.PositionAtTime(Data.EndTime);
            longNoteBody.Height = Y - endY;
            longNoteBody.Y = -LongNoteEnd.Height / 2;
            LongNoteEnd.Y = endY - Y;
        }

        if (Data.Time <= clock.CurrentTime && clock.CurrentTime - Data.Time <= max_distance && overZero) playfield.PlayHitSound(Data);

        overZero = Data.Time > clock.CurrentTime;
    }

    protected override bool OnHover(HoverEvent e) => true;
}
