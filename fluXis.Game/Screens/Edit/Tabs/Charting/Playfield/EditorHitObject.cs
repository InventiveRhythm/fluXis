using fluXis.Game.Map;
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

    public HitObjectInfo Data { get; init; }

    public Drawable HitObjectPiece { get; private set; }
    private Drawable longNoteBody { get; set; }
    public Drawable LongNoteEnd { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = EditorHitObjectContainer.NOTEWIDTH;
        AutoSizeAxes = Axes.Y;
        Origin = Anchor.BottomLeft;

        Children = new[]
        {
            HitObjectPiece = new DefaultHitObjectPiece(),
            longNoteBody = new DefaultHitObjectBody().With(b =>
            {
                b.Anchor = Anchor.BottomCentre;
                b.Origin = Anchor.BottomCentre;
            }),
            LongNoteEnd = new DefaultHitObjectEnd().With(e =>
            {
                e.Anchor = Anchor.BottomCentre;
                e.Origin = Anchor.BottomCentre;
            })
        };
    }

    protected override void Update()
    {
        base.Update();

        X = playfield.HitObjectContainer.PositionFromLane(Data.Lane);
        Y = playfield.HitObjectContainer.PositionAtTime(Data.Time);

        if (Data.IsLongNote())
        {
            var endY = playfield.HitObjectContainer.PositionAtTime(Data.HoldEndTime);
            longNoteBody.Height = Y - endY;
            longNoteBody.Y = -LongNoteEnd.Height / 2;
            LongNoteEnd.Y = endY - Y;
        }
    }

    protected override bool OnHover(HoverEvent e) => true;
}
