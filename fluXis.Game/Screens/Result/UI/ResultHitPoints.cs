using fluXis.Game.Map;
using fluXis.Game.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Result.UI
{
    public partial class ResultHitPoints : Container
    {
        public ResultHitPoints(MapInfo map, Performance performance)
        {
            Height = 300;
            RelativeSizeAxes = Axes.X;
            Margin = new MarginPadding { Top = 10 };
            CornerRadius = 10;
            Masking = true;

            Container hitPoints;

            AddRangeInternal(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.FromHex("#2a2a30")
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(10),
                    Child = hitPoints = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                    }
                },
            });

            foreach (var hitPoint in performance.HitPoints)
                hitPoints.Add(new Dot(map, hitPoint));
        }

        private partial class Dot : CircularContainer
        {
            public Dot(MapInfo map, HitPoint point)
            {
                Size = new Vector2(3);
                Masking = true;
                RelativePositionAxes = Axes.X;
                Anchor = Anchor.CentreLeft;
                Origin = Anchor.Centre;

                X = (point.Time - map.StartTime) / (map.EndTime - map.StartTime);
                Y = point.Difference;

                HitWindow hitWindow = HitWindow.FromKey(point.Judgement);

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
}
