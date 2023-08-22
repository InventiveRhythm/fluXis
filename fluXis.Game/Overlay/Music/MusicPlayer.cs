using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Cover;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Music;

public partial class MusicPlayer : VisibilityContainer, IKeyBindingHandler<FluXisKeybind>
{
    [Resolved]
    private AudioClock audioClock { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    public FluXisScreenStack ScreenStack { get; set; }

    private const int inner_padding = 40;
    private const int rounding = 20;

    private Container content;
    private Container backgrounds;
    private BackgroundVideo video;
    private Container covers;
    private FluXisSpriteText title;
    private FluXisSpriteText artist;
    private MusicPlayerButton pausePlay;

    protected override bool StartHidden => true;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new FullInputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Action = Hide,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = .5f
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 150, Bottom = 200, Top = 20 },
                Child = content = new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = rounding,
                    Y = -50,
                    EdgeEffect = new EdgeEffectParameters
                    {
                        Type = EdgeEffectType.Shadow,
                        Colour = Colour4.Black.Opacity(.25f),
                        Radius = 20,
                        Offset = new Vector2(0, 1)
                    },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2
                        },
                        backgrounds = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
                        video = new BackgroundVideo
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = .5f,
                            Children = new Box[]
                            {
                                new()
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Height = .8f,
                                    Colour = ColourInfo.GradientVertical(Colour4.Black.Opacity(0), Colour4.Black)
                                },
                                new()
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Height = .2f,
                                    Colour = Colour4.Black,
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft
                                }
                            }
                        },
                        new MusicVisualiser(),
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Horizontal = inner_padding, Bottom = inner_padding, Top = rounding + inner_padding },
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(20, 0),
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.BottomLeft,
                                    Children = new Drawable[]
                                    {
                                        covers = new Container
                                        {
                                            Size = new Vector2(150),
                                            Masking = true,
                                            CornerRadius = 20,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            EdgeEffect = new EdgeEffectParameters
                                            {
                                                Type = EdgeEffectType.Shadow,
                                                Colour = Colour4.Black.Opacity(.25f),
                                                Radius = 10,
                                                Offset = new Vector2(0, 1)
                                            },
                                            Children = new Drawable[]
                                            {
                                                new Box
                                                {
                                                    RelativeSizeAxes = Axes.Both,
                                                    Colour = Colour4.Black
                                                }
                                            }
                                        },
                                        new FillFlowContainer
                                        {
                                            AutoSizeAxes = Axes.Both,
                                            Direction = FillDirection.Vertical,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Children = new Drawable[]
                                            {
                                                title = new FluXisSpriteText
                                                {
                                                    Text = "Song Title",
                                                    FontSize = 36,
                                                    Shadow = true
                                                },
                                                artist = new FluXisSpriteText
                                                {
                                                    Text = "by Artist",
                                                    Colour = FluXisColors.Text2,
                                                    FontSize = 32,
                                                    Shadow = true
                                                },
                                                new FillFlowContainer
                                                {
                                                    AutoSizeAxes = Axes.Both,
                                                    Direction = FillDirection.Horizontal,
                                                    Spacing = new Vector2(10, 0),
                                                    Margin = new MarginPadding { Top = 20 },
                                                    Children = new Drawable[]
                                                    {
                                                        new MusicPlayerButton
                                                        {
                                                            Icon = FontAwesome.Solid.StepBackward,
                                                            Action = () =>
                                                            {
                                                                if (ScreenStack.AllowMusicControl)
                                                                    game.PreviousSong();
                                                            }
                                                        },
                                                        pausePlay = new MusicPlayerButton
                                                        {
                                                            Icon = FontAwesome.Solid.Play,
                                                            Action = () =>
                                                            {
                                                                if (!ScreenStack.AllowMusicControl)
                                                                    return;

                                                                if (audioClock.IsRunning)
                                                                    audioClock.Stop();
                                                                else
                                                                    audioClock.Start();
                                                            }
                                                        },
                                                        new MusicPlayerButton
                                                        {
                                                            Icon = FontAwesome.Solid.StepForward,
                                                            Action = () =>
                                                            {
                                                                if (ScreenStack.AllowMusicControl)
                                                                    game.NextSong();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        game.OnSongChanged += songChanged;
    }

    private void songChanged()
    {
        Schedule(() =>
        {
            if (maps.CurrentMapSet == null) return;

            video.Stop();

            title.Text = maps.CurrentMapSet.Metadata.Title;
            artist.Text = maps.CurrentMapSet.Metadata.Artist;

            LoadComponentAsync(new MapBackground
            {
                Map = maps.CurrentMapSet.Maps.First(),
                RelativeSizeAxes = Axes.Both,
                FillMode = FillMode.Fill
            }, background =>
            {
                backgrounds.Add(background);
                background.FadeInFromZero(400);
            });

            LoadComponentAsync(new DrawableCover(maps.CurrentMapSet)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill
            }, cover =>
            {
                covers.Add(cover);
                cover.FadeInFromZero(400);
            });

            Task.Run(() =>
            {
                video.Map = maps.CurrentMapSet.Maps.First();
                video.Info = maps.CurrentMapSet.Maps.First().GetMapInfo();

                video.LoadVideo();
                ScheduleAfterChildren(video.Start);
            });
        });
    }

    protected override void Update()
    {
        base.Update();

        while (backgrounds.Count > 1 && backgrounds.Last().Alpha == 1)
            backgrounds.Remove(backgrounds[0], true);

        while (covers.Count > 1 && covers.Last().Alpha == 1)
            covers.Remove(covers[0], true);

        pausePlay.IconSprite.Icon = audioClock.IsRunning ? FontAwesome.Solid.Pause : FontAwesome.Solid.Play;
    }

    protected override void PopIn()
    {
        this.FadeIn(200);
        content.MoveToY(0, 400, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.FadeOut(200);
        content.MoveToY(-50, 400, Easing.OutQuint);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.Back:
                Hide();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }
}
