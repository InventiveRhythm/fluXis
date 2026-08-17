using System;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Localisation;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace fluXis.Screens.Result.Sides.Types;

public partial class ResultsSideGraph : ResultsSideContainer
{
    protected override LocalisableString Title => "Graph";

    private Bindable<ScoreInfo> score { get; }
    private RealmMap map { get; }

    public ResultsSideGraph(Bindable<ScoreInfo> score, RealmMap map)
    {
        this.score = score;
        this.map = map;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        score.BindValueChanged(_ => RebuildContent());
    }

    protected override Drawable CreateContent() => new LoadWrapper<GraphContainer>
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        AutoSizeDuration = 400,
        AutoSizeEasing = Easing.Out,
        LoadContent = () => new GraphContainer(score.Value, map),
        OnComplete = g => g.FadeInFromZero(400)
    };

    private partial class GraphContainer : Container
    {
        private RealmMap map { get; }
        private ScoreInfo score { get; }

        public GraphContainer(ScoreInfo score, RealmMap map)
        {
            this.score = score;
            this.map = map;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            var rate = score.Rate;
            var judge = new HitWindows(map.AccuracyDifficulty, 1f);
            var timings = judge.GetTimings().ToList();

            int labelSize = 16;
            int subLabelSize = 14;

            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding { Left = 15 },
                    Margin = new MarginPadding { Left = 10 },
                    Child = new Graph(score, map)
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = 10 },
                    Margin = new MarginPadding { Left = 10 },
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = $"Early",
                            FontSize = subLabelSize,
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopRight,
                            Margin = new MarginPadding { Top = -10 }
                        },
                        new FluXisSpriteText
                        {
                            Text = $"Late",
                            FontSize = subLabelSize,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomRight,
                            Margin = new MarginPadding { Bottom = -10 }
                        },
                        new FluXisSpriteText
                        {
                            Text = $"-{timings[5].Milliseconds / rate:0}",
                            FontSize = labelSize,
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopRight,
                            Margin = new MarginPadding { Top = 10 }
                        },
                        new FluXisSpriteText
                        {
                            Text = $"-{timings[2].Milliseconds / rate:0}",
                            FontSize = labelSize,
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopRight,
                            RelativePositionAxes = Axes.Y,
                            Y = (float)((timings[5].Milliseconds - timings[3].Milliseconds) / (timings[5].Milliseconds * 2)),
                            Margin = new MarginPadding { Top = 5 }
                        },
                        new FluXisSpriteText
                        {
                            Text = $"0",
                            FontSize = labelSize,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreRight,
                            RelativePositionAxes = Axes.Y,
                        },
                        new FluXisSpriteText
                        {
                            Text = $"+{timings[2].Milliseconds / rate:0}",
                            FontSize = labelSize,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomRight,
                            RelativePositionAxes = Axes.Y,
                            Y = -(float)((timings[5].Milliseconds - timings[3].Milliseconds) / (timings[5].Milliseconds * 2)),
                            Margin = new MarginPadding { Bottom = 5 }
                        },
                        new FluXisSpriteText
                        {
                            Text = $"+{timings[5].Milliseconds / rate:0}",
                            FontSize = labelSize,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomRight,
                            Margin = new MarginPadding { Bottom = 10 }
                        }
                    }
                }
            };
        }
    }

    private partial class Graph : Sprite
    {
        [Resolved]
        private IRenderer renderer { get; set; }

        [Resolved]
        private SkinManager skins { get; set; }

        private RealmMap map { get; }
        private ScoreInfo score { get; }

        public Graph(ScoreInfo score, RealmMap map)
        {
            this.score = score;
            this.map = map;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            FillMode = FillMode.Fit;

            var rate = score.Rate;
            var judge = new HitWindows(map.AccuracyDifficulty, 1f);
            var miss = judge.TimingFor(Judgement.Miss);
            var timings = judge.GetTimings().ToList();

            var image = new Image<Rgba32>(800, (int)Math.Ceiling(miss) * 2, new Rgba32(0, 0, 0, 0));

            image.Mutate(ctx =>
            {
                var penWhite = Pens.Solid(Color.White, 2);
                var judgeColors = timings.Select(x => x.Judgement)
                                         .ToDictionary(x => x, x => Color.FromPixel(new Rgba32(skins.SkinJson.GetColorForJudgement(x).Vector)));

                // center line
                ctx.DrawLine(penWhite, new PointF(0, miss - 1), new PointF(image.Width, miss - 1));

                // judgement lines
                for (int i = 0; i < timings.Count - 1; i++)
                {
                    var timing = timings[i];
                    var pen = Pens.Solid(judgeColors[timing.Judgement].WithAlpha(.4f), 2);

                    var yEarly = miss - 1 - timing.Milliseconds;
                    var yLate = miss - 1 + timing.Milliseconds;

                    if (yEarly >= 0) ctx.DrawLine(pen, new PointF(0, yEarly), new PointF(image.Width, yEarly));
                    if (yLate < image.Height) ctx.DrawLine(pen, new PointF(0, yLate), new PointF(image.Width, yLate));
                }

                var start = score.HitResults.MinBy(x => x.Time).Time;
                var end = score.HitResults.MaxBy(x => x.Time).Time - start;

                var misses = score.HitResults.Where(x => x.Judgement == Judgement.Miss).ToList();
                var missBrush = Brushes.Solid(judgeColors[Judgement.Miss].WithAlpha(0.4f));

                foreach (var result in misses)
                {
                    var x = (float)((image.Width - 8) * ((result.Time - start) / end)) + 4;
                    ctx.Fill(missBrush, new Rectangle((int)x - 2, 0, 4, image.Height));
                }

                var judgeBrushes = judgeColors.ToDictionary(x => x.Key, x => Brushes.Solid(x.Value));

                foreach (var result in score.HitResults)
                {
                    var brush = judgeBrushes[result.Judgement];

                    var x = (float)((image.Width - 8) * ((result.Time - start) / end)) + 4;
                    var y = (float)(image.Height / 2f - result.Difference / rate);

                    if (!float.IsFinite(x))
                        x = image.Width / 2f;
                    if (result.Judgement == Judgement.Miss || result.Type == ResultType.Landmine)
                        y = image.Height / 2f;

                    ctx.Fill(brush, new EllipsePolygon(new PointF(x, y), 4));
                }
            });

            var upload = new TextureUpload(image);
            var texture = renderer.CreateTexture(image.Width, image.Height, true);
            texture.BypassTextureUploadQueueing = true;
            texture.SetData(upload);
            Texture = texture;
        }

        protected override void Update()
        {
            base.Update();

            Height = DrawWidth * ((float)Texture.Height / Texture.Width);
        }
    }
}
