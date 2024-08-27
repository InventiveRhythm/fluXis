using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Map.Drawables;
using fluXis.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Music;

public partial class MusicPlayer : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private GlobalClock globalClock { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private FluXisScreenStack screens { get; set; }

    private const int inner_padding = 40;
    private const int rounding = 20;

    private Container content;
    private SpriteStack<MapBackground> backgrounds;
    private BackgroundVideo video;
    private SpriteStack<MapCover> covers;
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
                    EdgeEffect = FluXisStyles.ShadowLarge,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2
                        },
                        backgrounds = new SpriteStack<MapBackground>(),
                        video = new BackgroundVideo
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            ShowDim = false,
                            Clock = globalClock
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
                                        new Container
                                        {
                                            Size = new Vector2(150),
                                            Masking = true,
                                            CornerRadius = 20,
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            EdgeEffect = FluXisStyles.ShadowMedium,
                                            Child = covers = new SpriteStack<MapCover>()
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
                                                        new MusicPlayerButton
                                                        {
                                                            Icon = FontAwesome6.Solid.ForwardStep,
                                                            Action = () =>
                                                            {
                                                                if (screens.AllowMusicControl)
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

        maps.MapBindable.BindValueChanged(mapChanged, true);
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

        title.Text = next.Metadata.Title;
        artist.Text = next.Metadata.Artist;

        LoadComponentAsync(new MapBackground(next) { RelativeSizeAxes = Axes.Both }, background =>
        {
            background.FadeInFromZero(400);
            backgrounds.Add(background, 400);
        });

        LoadComponentAsync(new MapCover(next.MapSet)
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }, cover =>
        {
            cover.FadeInFromZero(400);
            covers.Add(cover, 400);
        });

        Task.Run(() =>
        {
            video.Map = next;
            video.Info = next.GetMapInfo();

            video.LoadVideo();
            ScheduleAfterChildren(video.Start);
        });
    }

    protected override void Update()
    {
        base.Update();

        pausePlay.IconSprite.Icon = globalClock.IsRunning ? FontAwesome6.Solid.Pause : FontAwesome6.Solid.Play;
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

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                Hide();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
