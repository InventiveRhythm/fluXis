using System;
using System.Threading.Tasks;
using fluXis.Game.Graphics;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Scoring;
using fluXis.Game.Skinning;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;
using osuTK.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultHitPoints : Container
{
    public MapInfo MapInfo { get; set; }
    public Performance Performance { get; set; }

    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private IRenderer renderer { get; set; }

    private const int max_zoom = 4;

    private Container hitPoints;
    private LoadingIcon loadingIcon;

    private Image<Rgba32> image;
    private TextureUpload upload;

    [BackgroundDependencyLoader]
    private void load()
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
                Colour = ColourInfo.GradientVertical(FluXisColors.Surface.Opacity(.4f), FluXisColors.Surface.Opacity(.6f))
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                // Padding = new MarginPadding(10),
                Children = new Drawable[]
                {
                    hitPoints = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
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
                                Origin = Anchor.Centre
                            }
                        }
                    },
                    loadingIcon = new LoadingIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(50),
                        Alpha = 0
                    }
                }
            }
        });

        Task.Run(() =>
        {
            try { drawSprite(); }
            catch (Exception e)
            {
                Logger.Log($"Failed to draw hitpoints: {e.Message}", LoggingTarget.Runtime, LogLevel.Error);
                Logger.Log(e.StackTrace, LoggingTarget.Runtime, LogLevel.Error);

                Schedule(() =>
                {
                    var text = new FluXisSpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = "Failed to draw hitstats..."
                    };

                    hitPoints.Clear();
                    hitPoints.Add(text);
                    hitPoints.FadeInFromZero(200);
                    loadingIcon.FadeOut(200);
                });
            }
        });
    }

    private void drawSprite()
    {
        Schedule(() => loadingIcon.FadeIn(200));

        image = new Image<Rgba32>(740 * max_zoom, 300 * max_zoom, new Rgba32(0, 0, 0, 0));

        foreach (var stat in Performance.HitStats)
        {
            var statTime = stat.Time - MapInfo.StartTime;
            var endTime = MapInfo.EndTime - MapInfo.StartTime;

            var x = (int)(image.Width * (statTime == endTime ? .5f : statTime / endTime)) + 10 * max_zoom;
            var y = (int)(image.Height / 2f - stat.Difference * max_zoom) + 10 * max_zoom;

            HitWindow hitWindow = HitWindow.FromKey(stat.Judgement);
            var colour4 = skinManager.CurrentSkin.GetColorForJudgement(hitWindow.Key);
            var rgb = new Rgba32(colour4.R, colour4.G, colour4.B);

            const int radius = 3 * max_zoom;

            var poly = new EllipsePolygon(x, y, radius);
            var brush = new SolidBrush(new Color(rgb));
            image.Mutate(ctx => ctx.Fill(brush, poly));

            if (stat.Judgement == Judgement.Miss)
            {
                // draw a semi-transparent line with the width of radius over every transparent pixel in the same x
                for (int i = 0; i < radius / 2; i++)
                {
                    for (var y2 = 0; y2 < image.Height; y2++)
                    {
                        var pixel = image[x + i, y2];

                        if (pixel.A == 0)
                            image[x + i, y2] = new Rgba32(colour4.R, colour4.G, colour4.B, .25f);
                        else
                        {
                            var curAlpha = pixel.A / 255f;
                            var newAlpha = curAlpha + .25f;
                            image[x + i, y2] = new Rgba32(colour4.R, colour4.G, colour4.B, newAlpha);
                        }

                        pixel = image[x - i, y2];

                        if (pixel.A == 0)
                            image[x - i, y2] = new Rgba32(colour4.R, colour4.G, colour4.B, .25f);
                        else
                        {
                            var curAlpha = pixel.A / 255f;
                            var newAlpha = curAlpha + .25f;
                            image[x + i, y2] = new Rgba32(colour4.R, colour4.G, colour4.B, newAlpha);
                        }
                    }
                }
            }
        }

        upload = new TextureUpload(image);

        var sprite = new Sprite
        {
            RelativeSizeAxes = Axes.Both,
            Texture = renderer.CreateTexture(image.Width, image.Height, true),
        };

        sprite.Texture.BypassTextureUploadQueueing = true;
        sprite.Texture.SetData(upload);

        Schedule(() =>
        {
            hitPoints.Add(sprite);
            hitPoints.FadeInFromZero(200, Easing.OutQuint);
            loadingIcon.FadeOut(200);
        });
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        image?.Dispose();
        upload?.Dispose();
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
        hitPoints.ScaleTo(MathHelper.Clamp(hitPoints.Scale.X + e.ScrollDelta.Y / 2f, 1f, max_zoom), 100, Easing.OutQuint);
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

    public partial class ResultHitPoint : CircularContainer, IHasTextTooltip
    {
        public string Tooltip => TimeUtils.Format(Stat.Time) + " | " + Stat.Difference + "ms";

        public HitStat Stat { get; init; }
        public MapInfo MapInfo { get; init; }

        [BackgroundDependencyLoader]
        private void load(SkinManager skinManager)
        {
            Size = new Vector2(3);
            Masking = true;
            RelativePositionAxes = Axes.X;
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.Centre;

            var statTime = Stat.Time - MapInfo.StartTime;
            var endTime = MapInfo.EndTime - MapInfo.StartTime;

            X = statTime == endTime ? .5f : statTime / endTime;
            Y = Stat.Difference;

            HitWindow hitWindow = HitWindow.FromKey(Stat.Judgement);

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = skinManager.CurrentSkin.GetColorForJudgement(hitWindow.Key)
                }
            };
        }

        // required for tooltip
        protected override bool OnHover(HoverEvent e) => true;
    }
}
