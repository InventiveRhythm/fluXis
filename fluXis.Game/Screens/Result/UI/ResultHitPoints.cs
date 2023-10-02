using System;
using System.Threading.Tasks;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning;
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
    public ScoreInfo Score { get; set; }

    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private IRenderer renderer { get; set; }

    private const int max_zoom = 4;

    private Container hitResults;
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
                Colour = ColourInfo.GradientVertical(FluXisColors.Background3.Opacity(.4f), FluXisColors.Background3.Opacity(.6f))
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                // Padding = new MarginPadding(10),
                Children = new Drawable[]
                {
                    hitResults = new Container
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

                    hitResults.Clear();
                    hitResults.Add(text);
                    hitResults.FadeInFromZero(200);
                    loadingIcon.FadeOut(200);
                });
            }
        });
    }

    private void drawSprite()
    {
        Schedule(() => loadingIcon.FadeIn(200));

        image = new Image<Rgba32>(740 * max_zoom, 300 * max_zoom, new Rgba32(0, 0, 0, 0));
        const int padding = 10 * max_zoom;

        foreach (var result in Score.HitResults)
        {
            var startTime = result.Time - MapInfo.StartTime;
            var endTime = MapInfo.EndTime - MapInfo.StartTime;

            var x = (int)((image.Width - padding * 2) * (startTime == endTime ? .5f : startTime / endTime)) + padding;
            var y = (int)(image.Height / 2f - result.Difference * max_zoom);

            var colour4 = skinManager.CurrentSkin.GetColorForJudgement(result.Judgement);
            var rgb = new Rgba32(colour4.R, colour4.G, colour4.B);

            const int radius = 3 * max_zoom;

            var poly = new EllipsePolygon(x, y, radius);
            var brush = new SolidBrush(new Color(rgb));
            image.Mutate(ctx => ctx.Fill(brush, poly));

            if (result.Judgement == Judgement.Miss)
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
            hitResults.Add(sprite);
            hitResults.FadeInFromZero(200, Easing.OutQuint);
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
            float maxX = hitResults.DrawWidth / 2f * hitResults.Scale.X;
            hitResults.X += e.Delta.X;
            hitResults.X = MathHelper.Clamp(hitResults.X, -maxX, maxX);

            float maxY = hitResults.DrawHeight / 2f * hitResults.Scale.Y;
            hitResults.Y += e.Delta.Y;
            hitResults.Y = MathHelper.Clamp(hitResults.Y, -maxY, maxY);
        }
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        var scale = MathHelper.Clamp(hitResults.Scale.X + e.ScrollDelta.Y / 2f, 1f, max_zoom);
        hitResults.ScaleTo(scale, 100, Easing.OutQuint);
        float maxX = hitResults.DrawWidth / 2f * scale;
        float maxY = hitResults.DrawHeight / 2f * scale;

        if (Math.Abs(hitResults.X) >= maxX)
        {
            hitResults.MoveToX(MathHelper.Clamp(hitResults.X, -maxX, maxX), 100, Easing.OutQuint);
        }

        if (Math.Abs(hitResults.Y) >= maxY)
        {
            hitResults.MoveToY(MathHelper.Clamp(hitResults.Y, -maxY, maxY), 100, Easing.OutQuint);
        }

        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button == MouseButton.Right)
        {
            hitResults.ScaleTo(1f, 200, Easing.OutQuint);
            hitResults.MoveTo(Vector2.Zero, 200, Easing.OutQuint);
            return true;
        }

        return false;
    }
}
