using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset;

public partial class Stage : Container
{
    private const int lane_margin = 0;

    public Box Background;
    public Box BorderLeft;
    public Box BorderRight;

    private readonly Playfield playfield;

    private int currentKeyCount;

    public Stage(Playfield playfield)
    {
        this.playfield = playfield;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        MapInfo map = playfield.Map;
        currentKeyCount = map.InitialKeyCount;

        AddRangeInternal(new[]
        {
            Background = new Box
            {
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = Colour4.Black,
                Alpha = 0.5f,
                Width = Receptor.SIZE.X * map.InitialKeyCount + lane_margin * 2
            },
            BorderLeft = new Box
            {
                RelativeSizeAxes = Axes.Y,
                Height = 1f,
                Width = 5,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopRight,
                Colour = Colour4.White,
                X = -(Receptor.SIZE.X * (map.InitialKeyCount / 2f) + lane_margin)
            },
            BorderRight = new Box
            {
                RelativeSizeAxes = Axes.Y,
                Height = 1f,
                Width = 5,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomLeft,
                Colour = Colour4.White,
                X = Receptor.SIZE.X * (map.InitialKeyCount / 2f) + lane_margin
            }
        });
    }

    protected override void Update()
    {
        if (currentKeyCount != playfield.Manager.CurrentKeyCount)
        {
            var currentEvent = playfield.Manager.CurrentLaneSwitchEvent;
            currentKeyCount = currentEvent.Count;
            Background.ResizeWidthTo(Receptor.SIZE.X * currentKeyCount + lane_margin * 2, currentEvent.Speed, Easing.OutQuint);
            BorderLeft.MoveToX(-(Receptor.SIZE.X * (currentKeyCount / 2f) + lane_margin), currentEvent.Speed, Easing.OutQuint);
            BorderRight.MoveToX(Receptor.SIZE.X * (currentKeyCount / 2f) + lane_margin, currentEvent.Speed, Easing.OutQuint);
        }

        base.Update();
    }
}
