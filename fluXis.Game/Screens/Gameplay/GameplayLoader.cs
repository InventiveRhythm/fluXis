using System;
using System.Collections.Generic;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Cover;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Map.Drawables;
using fluXis.Game.Mods;
using fluXis.Game.Mods.Drawables;
using fluXis.Game.Online.Activity;
using fluXis.Game.UI.Tips;
using osu.Framework.Allocation;
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
    private void load()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Depth = -1;

        InternalChildren = new Drawable[]
        {
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
                            new DrawableCover(map.MapSet)
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
                            new FluXisSpriteText
                            {
                                Text = map.Metadata.Mapper,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.CentreLeft,
                                WebFontSize = 16,
                                Width = 200,
                                X = 5,
                                Truncate = true,
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
                            new FluXisSpriteText
                            {
                                Text = string.IsNullOrEmpty(map.Metadata.Source) ? "-" : map.Metadata.Source,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.CentreLeft,
                                WebFontSize = 16,
                                Width = 200,
                                X = 5,
                                Truncate = true,
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
                loadingContainer.FadeOut(200);
                clock.Delay(500).Schedule(() => clock.FadeOut(500));
                this.Delay(1000).Schedule(() =>
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
        clock.FadeIn();

        if (fadeBackToGlobalClock)
            clock.Start();

        clock.LowPassFilter.CutoffTo(LowPassFilter.MAX);
        return base.OnExiting(e);
    }

    private void contentIn()
    {
        tip.FadeIn();
        this.ScaleTo(.9f).ScaleTo(1, 400, Easing.OutQuint).FadeInFromZero(200).Then().Schedule(loadGameplay);
        content.MoveToY(0);
        loadingContainer.FadeIn(200);
        clock.LowPassFilter.CutoffTo(1000, 400, Easing.OutQuint);

        tip.Text = LoadingTips.RandomTip;
    }

    private void contentOut()
    {
        tip.FadeOut(400);
        this.Delay(800).FadeOut(200);
        content.MoveToY(800, 1200, Easing.InQuint);
        clock.LowPassFilter.CutoffTo(LowPassFilter.MAX);
    }
}
