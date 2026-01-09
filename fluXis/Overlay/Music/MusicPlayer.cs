using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Background;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using fluXis.Map;
using fluXis.Map.Drawables;
using fluXis.Screens;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osuTK;
using osuTK.Input;

namespace fluXis.Overlay.Music;

public partial class MusicPlayer : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private GlobalClock globalClock { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private Toolbar.Toolbar toolbar { get; set; }

    [Resolved]
    private GlobalBackground globalBackground { get; set; }

    [Resolved]
    private FluXisScreenStack screens { get; set; }

    private const float cover_small = 288;
    private const float cover_big = 144;

    private const int inner_padding = 8;
    private const int rounding = 16;

    private Container contentPad;
    private Container content;

    private Box dim;
    private BufferedContainer blur;
    private SpriteStack<MapBackground> backgrounds;
    private BackgroundVideo video;

    private Container coversContainer;
    private SpriteStack<MapCover> covers;

    private Container trackInfoContainer;
    private FillFlowContainer metadata;
    private FluXisSpriteText title;
    private FluXisSpriteText artist;

    private FillFlowContainer<MusicPlayerButton> buttons;
    private MusicPlayerButton pausePlay;
    private MusicPlayerButton fullscreenToggle;

    private Container gradient;
    private VerticalSectionedGradient colorGradient;
    private Progress progress;

    protected override bool StartHidden => true;

    [UsedImplicitly]
    private float animationProgress;

    private float lastProgress = -1;
    private bool fullscreen;

    private double metadataCountdown;
    private bool metadataVisible;

    [BackgroundDependencyLoader]
    private void load()
    {
        // to shut up msbuild
        animationProgress = 0;

        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new FullInputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                Allow = new List<Type> { typeof(MouseMoveEvent) },
                GenericHandle = e =>
                {
                    if (e is not KeyDownEvent kde) return false;
                    if (kde.Key != Key.F || kde.Repeat) return false;

                    toggleFullscreen();
                    return true;
                },
                Child = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Action = Hide,
                    Child = dim = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black.Opacity(0.5f)
                    }
                }
            },
            contentPad = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Right = 20, Top = 70 },
                Child = content = new HoverClickContainer
                {
                    Masking = true,
                    CornerRadius = rounding,
                    EdgeEffect = Styling.ShadowLarge,
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Theme.Background2
                        },
                        blur = new BufferedContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            RedrawOnScale = false,
                            Children = new Drawable[]
                            {
                                backgrounds = new SpriteStack<MapBackground>(),
                                video = new BackgroundVideo
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Clock = globalClock
                                },
                            }
                        },
                        gradient = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = .5f,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Children = new Drawable[]
                            {
                                colorGradient = new VerticalSectionedGradient
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    EndAlpha = 0,
                                    Colour = Theme.Highlight,
                                    Alpha = 0.5f
                                },
                                new VerticalSectionedGradient
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    EndAlpha = 0,
                                    Colour = Colour4.Black
                                },
                            }
                        },
                        new MusicVisualiser(),
                        trackInfoContainer = new Container
                        {
                            Padding = new MarginPadding(inner_padding),
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            AlwaysPresent = true,
                            Children = new Drawable[]
                            {
                                coversContainer = new Container
                                {
                                    Size = new Vector2(96),
                                    Masking = true,
                                    CornerRadius = 8,
                                    EdgeEffect = Styling.ShadowSmall,
                                    Child = covers = new SpriteStack<MapCover>()
                                },
                                metadata = new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                    Children = new Drawable[]
                                    {
                                        title = new TruncatingText
                                        {
                                            Text = "Song Title",
                                            WebFontSize = 20,
                                            Shadow = true
                                        },
                                        artist = new TruncatingText
                                        {
                                            Text = "by Artist",
                                            WebFontSize = 14,
                                            Shadow = true
                                        },
                                        buttons = new FillFlowContainer<MusicPlayerButton>
                                        {
                                            AutoSizeAxes = Axes.Both,
                                            Direction = FillDirection.Horizontal,
                                            Spacing = new Vector2(12),
                                            Margin = new MarginPadding { Top = 12 },
                                            Children = new[]
                                            {
                                                new()
                                                {
                                                    Icon = FontAwesome6.Solid.BackwardStep,
                                                    Action = () =>
                                                    {
                                                        if (screens.AllowMusicControl)
                                                            game.PreviousSong();
                                                    }
                                                },
                                                pausePlay = new MusicPlayerButton
                                                {
                                                    Icon = FontAwesome6.Solid.Play,
                                                    Action = () =>
                                                    {
                                                        if (!screens.AllowMusicPausing)
                                                            return;

                                                        if (globalClock.IsRunning)
                                                            globalClock.Stop();
                                                        else
                                                            globalClock.Start();
                                                    }
                                                },
                                                new()
                                                {
                                                    Icon = FontAwesome6.Solid.ForwardStep,
                                                    Action = () =>
                                                    {
                                                        if (screens.AllowMusicControl)
                                                            game.NextSong();
                                                    }
                                                },
                                                fullscreenToggle = new MusicPlayerButton
                                                {
                                                    Icon = FontAwesome6.Solid.UpRightAndDownLeftFromCenter,
                                                    Action = toggleFullscreen
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        progress = new Progress(globalClock)
                        {
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            RelativeSizeAxes = Axes.X,
                            Width = 1f
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        maps.MapBindable.BindValueChanged(mapChanged, true);
    }

    protected override void Update()
    {
        base.Update();

        globalBackground.Alpha = screens.Alpha = dim.Alpha = animationProgress >= 1f ? 0 : 1;
        backgrounds.Alpha = video.Alpha >= 1f ? 0 : 1;

        pausePlay.IconSprite.Icon = globalClock.IsRunning ? FontAwesome6.Solid.Pause : FontAwesome6.Solid.Play;
        fullscreenToggle.IconSprite.Icon = fullscreen ? FontAwesome6.Solid.DownLeftAndUpRightToCenter : FontAwesome6.Solid.UpRightAndDownLeftFromCenter;

        if (video.IsPlaying && fullscreen) metadataCountdown -= Time.Elapsed;
        if (metadataCountdown <= 0) hideMetadata();

        // needs to be outside to react to toolbar movement
        contentPad.Padding = new MarginPadding
        {
            Top = Interpolation.ValueAt(animationProgress, toolbar.DrawHeight + toolbar.Y + 20, 0, 0, 1),
            Right = Interpolation.ValueAt(animationProgress, 20, 0, 0, 1)
        };

        title.MaxWidth = artist.MaxWidth = Interpolation.ValueAt(animationProgress, cover_small, DrawWidth - 36 * 2 - 12 - cover_big, 0, 1);

        title.Margin = new MarginPadding { Left = Interpolation.ValueAt(animationProgress, cover_small / 2f - Math.Min(title.DrawWidth, title.MaxWidth) / 2f, 0, 0, 1) };
        artist.Margin = new MarginPadding { Left = Interpolation.ValueAt(animationProgress, cover_small / 2f - Math.Min(artist.DrawWidth, artist.MaxWidth) / 2f, 0, 0, 1) };

        if (lastProgress == animationProgress)
            return;

        lastProgress = animationProgress;

        var blurVal = Interpolation.ValueAt(animationProgress, .5f, 0f, 0, 1);
        blur.BlurSigma = new Vector2(blurVal * 12);
        blur.FrameBufferScale = new Vector2(1 - blurVal * .5f);

        gradient.Height = Interpolation.ValueAt(animationProgress, 2f, 1f, 0, 1);

        content.Width = Interpolation.ValueAt(animationProgress, cover_small + inner_padding * 2, DrawWidth, 0, 1);
        content.Height = Interpolation.ValueAt(animationProgress, 444, DrawHeight, 0, 1);
        content.CornerRadius = Interpolation.ValueAt(animationProgress, rounding, 0, 0, 1);

        trackInfoContainer.Width = Interpolation.ValueAt(animationProgress, cover_small + inner_padding * 2, DrawWidth, 0, 1);
        trackInfoContainer.Height = Interpolation.ValueAt(animationProgress, 444, cover_big + 36 * 2, 0, 1);
        trackInfoContainer.Padding = new MarginPadding(Interpolation.ValueAt(animationProgress, inner_padding, 36, 0, 1));

        metadata.X = Interpolation.ValueAt(animationProgress, 0, cover_big + 12, 0, 1);
        metadata.Y = Interpolation.ValueAt(animationProgress, cover_small + 20, cover_big / 2f - 58, 0, 1);

        coversContainer.Size = new Vector2(Interpolation.ValueAt(animationProgress, cover_small, cover_big, 0, 1));

        title.WebFontSize = Interpolation.ValueAt(animationProgress, 20, 24, 0, 1);
        artist.WebFontSize = Interpolation.ValueAt(animationProgress, 14, 16, 0, 1);

        buttons.Margin = new MarginPadding
        {
            Top = Interpolation.ValueAt(animationProgress, 12, 20, 0, 1),
            Left = Interpolation.ValueAt(animationProgress, cover_small / 2f - 196 / 2f, 0, 0, 1)
        };
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        maps.MapBindable.ValueChanged -= mapChanged;
    }

    private void mapChanged(ValueChangedEvent<RealmMap> e) => Scheduler.AddOnce(songChanged);

    private void songChanged()
    {
        var next = maps.CurrentMap;

        if (next == null) return;

        video.Stop();

        var color = next.Metadata.Color;
        var hue = color.ToHSL().X;
        var accent = Colour4.FromHSL(hue, .6f, .9f);

        title.FadeColour(accent, 400);
        artist.FadeColour(accent, 400);
        progress.FadeColour(accent, 400);
        buttons.ForEach(x => x.IconSprite.FadeColour(accent, 400));
        colorGradient.FadeColour(accent, 400);

        title.Text = next.Metadata.LocalizedTitle;
        artist.Text = next.Metadata.LocalizedArtist;

        showMetadata();

        LoadComponentAsync(new MapBackground(next) { RelativeSizeAxes = Axes.Both }, background =>
        {
            backgrounds.Add(background, 400);
            background.Show();
        });

        LoadComponentAsync(new MapCover(next.MapSet)
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }, cover =>
        {
            covers.Add(cover, 400);
            cover.Show();
        });

        Task.Run(() =>
        {
            video.LoadVideo(next.GetMapInfo());
            ScheduleAfterChildren(video.Start);
        });
    }

    private void showMetadata()
    {
        metadataCountdown = 4000;
        if (metadataVisible) return;

        metadataVisible = true;

        trackInfoContainer.ClearTransforms();
        trackInfoContainer.FadeIn(200);

        gradient.ClearTransforms();
        gradient.FadeTo(.5f, 200);

        progress.ClearTransforms();
        progress.FadeIn(200);
    }

    private void hideMetadata()
    {
        if (!metadataVisible) return;

        metadataVisible = false;

        trackInfoContainer.Delay(1000).FadeOut(800);
        gradient.Delay(1000).FadeOut(800);
        progress.Delay(1000).FadeOut(800);
    }

    private void toggleFullscreen()
    {
        fullscreen = !fullscreen;
        this.TransformTo(nameof(animationProgress), fullscreen ? 1f : 0f, 600, Easing.OutQuint);

        if (fullscreen)
            toolbar.Hide();
        else
            toolbar.Show();
    }

    protected override void PopIn()
    {
        this.FadeIn(200);
        content.ScaleTo(.9f).ScaleTo(1f, 800, Easing.OutElasticHalf);
        showMetadata();
    }

    protected override void PopOut()
    {
        this.FadeOut(200);
        content.ScaleTo(.9f, 400, Easing.OutQuint);
        if (fullscreen) toggleFullscreen();
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                if (fullscreen)
                    toggleFullscreen();
                else
                    Hide();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        showMetadata();
        return true;
    }

    private partial class Progress : Container
    {
        private const float collapsed_height = 6;
        private const float expanded_height = 10;

        private readonly GlobalClock clock;

        private Box fill;
        private Box background;
        private SeekContainer seekContainer;

        public Progress(GlobalClock clock)
        {
            this.clock = clock;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.BottomLeft;
            RelativeSizeAxes = Axes.X;
            Width = 1f; 
            Height = collapsed_height;

            Child = seekContainer = new SeekContainer
            {
                RelativeSizeAxes = Axes.Both,
                IsPlaying = () => clock.IsRunning,
                AlwaysDebounce = true,
                OnSeek = onSeek,
                Children = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.White,
                        Alpha = 0.1f 
                    },
                    fill = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = 0, 
                        Colour = Theme.Text,
                    }
                }
            };
        }

        protected override void Update()
        {
            base.Update();

            if (seekContainer.IsDragged)
            {
                fill.Width = seekContainer.Progress.Value;
            }
            else
            {
                var width = clock.CurrentTime / clock.CurrentTrack.Length;
                if (!double.IsFinite(width) || double.IsNaN(width)) width = 0;

                fill.Width = (float)width;
            }
        }

        protected override bool OnHover(HoverEvent e)
        {
            this.ResizeHeightTo(expanded_height, 200, Easing.OutQuad);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);
            this.ResizeHeightTo(collapsed_height, 300, Easing.OutQuad);
        }

        public void FadeColour(ColourInfo colour, double duration = 0, Easing easing = Easing.None)
        {
            fill.FadeColour(colour, duration, easing);
        }

        private void onSeek(float progress)
        {
            if (clock.CurrentTrack == null) return;
            double targetTime = progress * clock.CurrentTrack.Length;
            clock.Seek(targetTime);
        }
    }
}
