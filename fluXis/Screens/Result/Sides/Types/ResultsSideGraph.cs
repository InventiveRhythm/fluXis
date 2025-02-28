using System;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Graphics.Containers;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
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

    private ScoreInfo score { get; }
    private RealmMap map { get; }

    public ResultsSideGraph(ScoreInfo score, RealmMap map)
    {
        this.score = score;
        this.map = map;
    }

    protected override Drawable CreateContent() => new LoadWrapper<Graph>
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        AutoSizeDuration = 400,
        AutoSizeEasing = Easing.Out,
        LoadContent = () => new Graph(score, map),
        OnComplete = g => g.FadeInFromZero(400)
    };

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

            var image = new Image<Rgba32>(800, (int)Math.Ceiling(miss) * 2, new Rgba32(0, 0, 0, 0));

            image.Mutate(ctx => ctx.DrawLine(new Color(new Rgba32(1f, 1f, 1f)), 2, new PointF(0, miss - 1), new PointF(image.Width, miss - 1)));

            var start = score.HitResults.MinBy(x => x.Time).Time;
            var end = score.HitResults.MaxBy(x => x.Time).Time - start;

            var misses = score.HitResults.Where(x => x.Judgement == Judgement.Miss).ToList();

            foreach (var result in misses)
            {
                var color = skins.SkinJson.GetColorForJudgement(result.Judgement);
                var x = (float)((image.Width - 8) * ((result.Time - start) / end)) + 4;

                image.Mutate(ctx => ctx.Fill(new Color(new Rgba32(color.Opacity(.4f).Vector)), new RectangleF(x - 2, 0, 4, image.Height)));
            }

            foreach (var result in score.HitResults)
            {
                var color = skins.SkinJson.GetColorForJudgement(result.Judgement);

                var x = (float)((image.Width - 8) * ((result.Time - start) / end)) + 4;
                var y = (float)(image.Height / 2f - result.Difference / rate);

                if (!float.IsFinite(x))
                    x = image.Width / 2f;
                if (result.Judgement == Judgement.Miss)
                    y = image.Height / 2f;

                var poly = new EllipsePolygon(x, y, 4);
                var brush = new SolidBrush(new Color(color.Vector));

                image.Mutate(ctx => ctx.Fill(brush, poly));
            }

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
