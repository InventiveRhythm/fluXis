using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Scoring;
using fluXis.Game.Utils;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultHitPoints : Container
{
    private readonly Container hitPoints;

    public ResultHitPoints(MapInfo map, Performance performance)
    {
        Height = 300;
        RelativeSizeAxes = Axes.X;
        Margin = new MarginPadding { Top = 10 };
        CornerRadius = 10;
        Masking = true;

        AddRangeInternal(new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Surface
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(10),
                Children = new Drawable[]
                {
                    hitPoints = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                Height = 2,
                                Width = 3,
                                RelativeSizeAxes = Axes.X,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                            }
                        }
                    }
                }
            }
        });

        foreach (var hitPoint in performance.HitStats)
            hitPoints.Add(new ResultHitPoint(map, hitPoint));
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        return e.Button == MouseButton.Left;
    }

    protected override void OnDrag(DragEvent e)
    {
        if (e.Button == MouseButton.Left)
        {
            float maxX = hitPoints.DrawWidth / 2f * hitPoints.Scale.X;
            hitPoints.X += e.Delta.X;
            hitPoints.X = MathHelper.Clamp(hitPoints.X, -maxX, maxX);

            float maxY = hitPoints.DrawHeight / 2f * hitPoints.Scale.Y;
            hitPoints.Y += e.Delta.Y;
            hitPoints.Y = MathHelper.Clamp(hitPoints.Y, -maxY, maxY);
        }
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        hitPoints.ScaleTo(MathHelper.Clamp(hitPoints.Scale.X + e.ScrollDelta.Y / 2f, 1f, 5f), 100, Easing.OutQuint);
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button == MouseButton.Right)
        {
            hitPoints.ScaleTo(1f, 200, Easing.OutQuint);
            hitPoints.MoveTo(Vector2.Zero, 200, Easing.OutQuint);
            return true;
        }

        return false;
    }

    public partial class ResultHitPoint : CircularContainer, IHasTooltip
    {
        public string Tooltip => TimeUtils.Format(stat.Time) + " | " + stat.Difference + "ms";

        private readonly HitStat stat;

        public ResultHitPoint(MapInfo map, HitStat stat)
        {
            this.stat = stat;

            Size = new Vector2(3);
            Masking = true;
            RelativePositionAxes = Axes.X;
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.Centre;

            X = (stat.Time - map.StartTime) / (map.EndTime - map.StartTime);
            Y = stat.Difference;

            HitWindow hitWindow = HitWindow.FromKey(stat.Judgement);

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = hitWindow.Color
                }
            };
        }
    }
}
