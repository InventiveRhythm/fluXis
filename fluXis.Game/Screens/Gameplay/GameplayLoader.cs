using System;
using System.Collections.Generic;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Map.Drawables;
using fluXis.Game.Mods;
using fluXis.Game.Mods.Drawables;
using fluXis.Game.Online.Activity;
using fluXis.Game.UI.Tips;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Gameplay;

public partial class GameplayLoader : FluXisScreen
{
    public override float Zoom => 1.3f;
    public override float ParallaxStrength => .1f;
    public override bool ShowToolbar => false;
    public override float BackgroundDim => 0.5f;
    public override float BackgroundBlur => 0.5f;
    public override bool AllowMusicControl => false;
    public override UserActivity InitialActivity => new UserActivity.LoadingGameplay();

    [Resolved]
    private GlobalClock clock { get; set; }

    public GameplayScreen GameplayScreen { get; set; }
    private readonly Func<GameplayScreen> createFunc;
    private bool fadeBackToGlobalClock;

    private RealmMap map { get; }
    private List<IMod> mods { get; }

    private LowPassFilter lowPass;

    private Container loadingContainer;
    private FillFlowContainer content;
    private FluXisSpriteText tip;

    public GameplayLoader(RealmMap map, List<IMod> mods, Func<GameplayScreen> create)
    {
        this.map = map;
        this.mods = mods;
        createFunc = create;
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Depth = -1;

        InternalChildren = new Drawable[]
        {
            lowPass = new LowPassFilter(audio.TrackMixer),
            content = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new Container
                    {
                        Size = new Vector2(300),
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        CornerRadius = 20,
                        Masking = true,
                        EdgeEffect = FluXisStyles.ShadowMedium,
                        Margin = new MarginPadding { Bottom = 20 },
                        Children = new Drawable[]
                        {
                            new MapCover(map.MapSet)
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                FillMode = FillMode.Fill
                            },
                            loadingContainer = new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = Colour4.Black,
                                        Alpha = 0.5f
                                    },
                                    new LoadingIcon
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Size = new Vector2(100)
                                    }
                                }
                            }
                        }
                    },
                    new FluXisSpriteText
                    {
                        Text = map.Metadata.Title,
                        Margin = new MarginPadding { Bottom = -5 },
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Shadow = true,
                        WebFontSize = 36
                    },
                    new FluXisSpriteText
                    {
                        Text = map.Metadata.Artist,
                        Margin = new MarginPadding { Bottom = 10 },
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Shadow = true,
                        Alpha = .8f,
                        FontSize = 32
                    },
                    new FluXisSpriteText
                    {
                        Text = map.Difficulty,
                        Margin = new MarginPadding { Bottom = 5 },
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Shadow = true,
                        WebFontSize = 24
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Spacing = new Vector2(10),
                        Margin = new MarginPadding { Bottom = 20 },
                        Children = new Drawable[]
                        {
                            new KeyCountChip
                            {
                                Size = new Vector2(40, 20),
                                FontSize = 14 * 1.4f,
                                KeyCount = map.KeyCount
                            },
                            new DifficultyChip
                            {
                                Size = new Vector2(80, 20),
                                FontSize = 14 * 1.4f,
                                Rating = map.Filters.NotesPerSecond
                            }
                        }
                    },
                    new Container
                    {
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = "Mapper",
                                Anchor = Anchor.Centre,
                                Origin = Anchor.CentreRight,
                                WebFontSize = 16,
                                Alpha = .8f,
                                X = -5,
                                Shadow = true
                            },
                            new TruncatingText
                            {
                                Text = map.Metadata.Mapper,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.CentreLeft,
                                WebFontSize = 16,
                                Width = 200,
                                X = 5,
                                Shadow = true
                            }
                        }
                    },
                    new Container
                    {
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Margin = new MarginPadding { Bottom = 20 },
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = "Source",
                                Anchor = Anchor.Centre,
                                Origin = Anchor.CentreRight,
                                WebFontSize = 16,
                                Alpha = .8f,
                                X = -5,
                                Shadow = true
                            },
                            new TruncatingText
                            {
                                Text = string.IsNullOrEmpty(map.Metadata.Source) ? "-" : map.Metadata.Source,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.CentreLeft,
                                WebFontSize = 16,
                                Width = 200,
                                X = 5,
                                Shadow = true
                            }
                        }
                    },
                    new ModList
                    {
                        Mods = mods,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    }
                }
            },
            tip = new FluXisSpriteText
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                FontSize = 36,
                Margin = new MarginPadding { Left = 40, Bottom = 35 }
            }
        };
    }

    private void loadGameplay()
    {
        GameplayScreen = createFunc();

        if (GameplayScreen == null)
        {
            this.Exit();
            return;
        }

        GameplayScreen.Loader = this;
        GameplayScreen.OnRestart += requestRestart;
        fadeBackToGlobalClock = GameplayScreen.FadeBackToGlobalClock;

        LoadComponentAsync(GameplayScreen, _ =>
        {
            if (!this.IsCurrentScreen())
                GameplayScreen.Dispose();
            else
            {
                ValidForResume = false;
                loadingContainer.FadeOut(FADE_DURATION);
                clock.Delay(MOVE_DURATION).Schedule(() => clock.VolumeOut(MOVE_DURATION));
                this.Delay(MOVE_DURATION * 2).Schedule(() =>
                {
                    clock.Stop();
                    this.Push(GameplayScreen);
                });
            }
        });
    }

    private void requestRestart()
    {
        ValidForResume = true;
        this.MakeCurrent();
    }

    public override void OnEntering(ScreenTransitionEvent e) => contentIn();
    public override void OnSuspending(ScreenTransitionEvent e) => contentOut();
    public override void OnResuming(ScreenTransitionEvent e) => contentIn();

    public override bool OnExiting(ScreenExitEvent e)
    {
        clock.VolumeIn(400, Easing.Out);

        if (fadeBackToGlobalClock)
            clock.Start();

        contentOut(false);
        return base.OnExiting(e);
    }

    private void contentIn()
    {
        this.FadeOut();

        using (BeginDelayedSequence(ENTER_DELAY))
        {
            this.ScaleTo(.9f).ScaleTo(1, MOVE_DURATION, Easing.OutQuint)
                .FadeIn(FADE_DURATION).Then().Schedule(loadGameplay);

            tip.FadeIn();
            content.MoveToY(0);
            loadingContainer.FadeIn(FADE_DURATION);
            lowPass.CutoffTo(LowPassFilter.MIN, MOVE_DURATION, Easing.OutQuint);
        }

        tip.Text = LoadingTips.RandomTip;
    }

    private void contentOut(bool moveDown = true)
    {
        this.Delay(MOVE_DURATION).FadeOut(FADE_DURATION);
        tip.FadeOut(FADE_DURATION);
        lowPass.CutoffTo(LowPassFilter.MAX);

        if (moveDown)
            content.MoveToY(800, MOVE_DURATION + FADE_DURATION + 300, Easing.InQuint);
    }
}
