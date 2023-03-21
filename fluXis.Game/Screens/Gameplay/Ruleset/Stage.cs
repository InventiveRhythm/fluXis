using fluXis.Game.Graphics;
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
    public Container BorderLeft;
    public Container BorderRight;

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

        AddRangeInternal(new Drawable[]
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
            BorderLeft = new Container
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                X = -(Receptor.SIZE.X * (map.InitialKeyCount / 2f) + lane_margin),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopRight,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 5,
                        Margin = new MarginPadding { Left = 2 },
                        Colour = FluXisColors.Surface
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 2,
                        Alpha = .5f,
                        Colour = FluXisColors.Accent3
                    }
                }
            },
            BorderRight = new Container
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                X = Receptor.SIZE.X * (map.InitialKeyCount / 2f) + lane_margin,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomLeft,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 5,
                        Colour = FluXisColors.Surface
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 2,
                        Margin = new MarginPadding { Left = 5 },
                        Alpha = .5f,
                        Colour = FluXisColors.Accent
                    }
                }
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
