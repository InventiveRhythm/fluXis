using fluXis.Game.Map;
using fluXis.Game.Skinning.Default.HitObject;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;

public partial class EditorHitObject : Container
{
    [Resolved]
    private EditorPlayfield playfield { get; set; }

    public HitObjectInfo Data { get; init; }

    private Drawable lnBody;
    private Drawable lnEnd;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = EditorHitObjectContainer.NOTEWIDTH;
        AutoSizeAxes = Axes.Y;
        Origin = Anchor.BottomLeft;

        Children = new[]
        {
            new DefaultHitObjectPiece(),
            lnBody = new DefaultHitObjectBody().With(b =>
            {
                b.Anchor = Anchor.BottomCentre;
                b.Origin = Anchor.BottomCentre;
            }),
            lnEnd = new DefaultHitObjectEnd().With(e =>
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
            lnBody.Height = Y - endY;
            lnBody.Y = -lnEnd.Height / 2;
            lnEnd.Y = endY - Y;
        }
    }
}
