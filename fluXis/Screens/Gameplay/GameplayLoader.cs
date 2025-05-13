using System;
using System.Collections.Generic;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Input;
using fluXis.Map.Drawables;
using fluXis.Mods;
using fluXis.Mods.Drawables;
using fluXis.Online.Activity;
using fluXis.UI.Tips;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Gameplay;

public partial class GameplayLoader : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float Zoom => screen?.Zoom ?? 1.2f;
    public override float ParallaxStrength => 0;
    public override bool ShowToolbar => false;
    public override float BackgroundDim => screen?.BackgroundDim ?? 0.5f;
    public override float BackgroundBlur => screen?.BackgroundBlur ?? 0.5f;
    public override bool AllowMusicControl => false;
    public override UserActivity InitialActivity => new UserActivity.LoadingGameplay();

    [Resolved]
    private GlobalClock clock { get; set; }

    private GameplayScreen screen;
    private readonly Func<GameplayScreen> createFunc;
    private bool fadeBackToGlobalClock;

    public bool HasRestarted { get; private set; }

    private RealmMap map { get; }
    private List<IMod> mods { get; }

    private bool allowExiting = true;
    private AudioFilter lowPass;

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
            lowPass = new AudioFilter(audio.TrackMixer),
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
                        Text = map.Metadata.LocalizedTitle,
                        Margin = new MarginPadding { Bottom = -5 },
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Shadow = true,
                        WebFontSize = 36
                    },
                    new FluXisSpriteText
                    {
                        Text = map.Metadata.LocalizedArtist,
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
                                Rating = map.Rating
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
        allowExiting = true;
        screen = createFunc();

        if (screen == null)
        {
            this.Exit();
            return;
        }

        screen.OnRestart += requestRestart;
        fadeBackToGlobalClock = screen.FadeBackToGlobalClock;

        LoadComponentAsync(screen, _ =>
        {
            allowExiting = false;

            if (!screen.ValidForPush)
            {
                screen.Dispose();
                this.Exit();
                return;
            }

            if (!this.IsCurrentScreen())
                screen.Dispose();
            else
            {
                ValidForResume = false;
                loadingContainer.FadeOut(FADE_DURATION);

                if (HasRestarted)
                {
                    clock.Stop();
                    this.Push(screen);
                    return;
                }

                clock.Delay(MOVE_DURATION).Schedule(() => clock.VolumeOut(MOVE_DURATION));
                this.Delay(MOVE_DURATION * 2).Schedule(() =>
                {
                    clock.Stop();
                    this.Push(screen);
                });
            }
        });
    }

    private void requestRestart()
    {
        ValidForResume = true;
        HasRestarted = true;
        this.MakeCurrent();
    }

    public override void OnEntering(ScreenTransitionEvent e) => contentIn();
    public override void OnSuspending(ScreenTransitionEvent e) => contentOut(!HasRestarted);
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
            content.ScaleTo(1f);
            var seq = this.FadeIn(FADE_DURATION);

            if (!HasRestarted)
                seq = seq.ScaleTo(.9f).ScaleTo(1, MOVE_DURATION, Easing.OutQuint);

            seq.Then(MOVE_DURATION).Schedule(loadGameplay);

            tip.FadeIn().MoveToX(0);
            content.MoveToY(0);
            loadingContainer.FadeIn(FADE_DURATION);
            lowPass.CutoffTo(AudioFilter.MIN, MOVE_DURATION, Easing.OutQuint);
        }

        tip.Text = LoadingTips.RandomTip;
    }

    private void contentOut(bool moveDown = true)
    {
        lowPass.CutoffTo(AudioFilter.MAX);

        if (moveDown)
        {
            this.Delay(MOVE_DURATION).FadeOut(FADE_DURATION);
            tip.FadeOut(FADE_DURATION);
            content.MoveToY(800, MOVE_DURATION + FADE_DURATION + 300, Easing.InQuint);
        }
        else
        {
            this.FadeOut(FADE_DURATION);
            content.ScaleTo(.95f, MOVE_DURATION, Easing.OutQuint);
            tip.MoveToX(-40, MOVE_DURATION, Easing.OutQuint);
        }
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back when allowExiting:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}
