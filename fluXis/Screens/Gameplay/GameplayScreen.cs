using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using fluXis.Audio;
using fluXis.Audio.Transforms;
using fluXis.Configuration;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Background;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Shaders;
using fluXis.Input;
using fluXis.Map;
using fluXis.Map.Structures.Bases;
using fluXis.Mods;
using fluXis.Online.Activity;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Notifications;
using fluXis.Replays;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Audio;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Screens.Gameplay.Capabilities;
using fluXis.Screens.Gameplay.Capabilities.Bases;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Gameplay.Input;
using fluXis.Screens.Gameplay.Overlay;
using fluXis.Screens.Gameplay.Overlay.Effect;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Screens.Gameplay.UI;
using fluXis.Screens.Gameplay.UI.Menus;
using fluXis.Screens.Intro;
using fluXis.Skinning.Default;
using fluXis.Storyboards;
using fluXis.Storyboards.Drawables;
using fluXis.Utils.Extensions;
using Midori.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Screens.Gameplay;

public sealed partial class GameplayScreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float ParallaxStrength => 0f;
    public override float BackgroundBlur => backgroundBlur?.Value ?? 0f;
    public override float BackgroundDim => backgroundDim?.Value ?? .4f;
    public override bool ShowToolbar => false;
    public override bool AllowMusicControl => false;
    public override bool ShowCursor => PlayfieldManager.AnyFailed || IsPaused.Value || CursorVisible;
    public override bool ApplyValuesAfterLoad => true;
    public override bool AllowExit => false;

    public override string WindowTitle
    {
        get
        {
            var title = "";

            if (Game is null)
                return title;

            if (Game.UsingOriginalMetadata || string.IsNullOrWhiteSpace(Map.Metadata.ArtistRomanized))
                title += Map.Metadata.Artist;
            else
                title += Map.Metadata.ArtistRomanized;

            title += " - ";

            if (Game.UsingOriginalMetadata || string.IsNullOrWhiteSpace(Map.Metadata.TitleRomanized))
                title += Map.Metadata.Title;
            else
                title += Map.Metadata.TitleRomanized;

            return title + $" [{Map.Metadata.Difficulty}]";
        }
    }

    public bool AllowPausing { get; set; } = true;
    public bool AllowRestarting { get; set; } = true;
    public bool CursorVisible { get; set; }
    public bool FadeBackToGlobalClock { get; set; } = true;
    public bool InstantlyExitOnPause { get; set; }
    public bool UseGlobalOffset { get; set; } = true;
    public double GameplayStartTime { get; set; }

    public bool SubmitScore => capabilities.Any(x => x is ScoreSubmissionCapability);

    [Resolved]
    public FluXisConfig Config { get; private set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private GlobalClock globalClock { get; set; }

    [Resolved(CanBeNull = true)]
    private GlobalFFTProcessor fftProcessor { get; set; }

    private DependencyContainer dependencies;

    private bool starting = true;
    public bool Restarting { get; private set; }

    public event Action OnExit;

    public BindableBool IsPaused { get; } = new();
    public GameplaySamples Samples { get; } = new();
    public Hitsounding Hitsounding { get; private set; }

    public GameplayClock GameplayClock { get; private set; }
    public Action<double, double> OnSeek { get; set; }

    public Action OnRestart { get; set; }
    public ReplayRecorder ReplayRecorder { get; private set; }

    public List<ScoreInfo> Scores { get; init; } = new();

    public DebugText Debug { get; private set; }

    public MapInfo Map { get; private set; }
    public RealmMap RealmMap { get; }
    public MapEvents MapEvents { get; private set; }
    public List<IMod> Mods { get; }

    private GameplayKeybindContainer keybindContainer;
    private GlobalBackground background;
    private BackgroundVideo backgroundVideo;
    private GameplayClockContainer clockContainer;
    private Container hud { get; set; }

    public RulesetContainer RulesetContainer { get; private set; }
    public PlayfieldManager PlayfieldManager { get; private set; }

    private FailMenu failMenu;
    private FullComboOverlay fcOverlay;
    private QuickActionOverlay quickActionOverlay;

    private Bindable<HudVisibility> hudVisibility;
    private Bindable<float> backgroundDim;
    private Bindable<float> backgroundBlur;
    private Bindable<bool> bgaEnabled;

    private bool hudVisible = true;

    public float Rate { get; }

    private readonly List<IGameplayCapability> capabilities = new();

    public GameplayScreen(RealmMap realmMap, List<IMod> mods)
    {
        RealmMap = realmMap;
        Mods = mods;

        Rate = ((RateMod)Mods.Find(m => m is RateMod))?.Rate ?? 1f;
        Mods.RemoveAll(m => m is PausedMod);
    }

    #region Screen Presets

    public static GameplayScreen Solo(RealmMap map, List<IMod> mods, List<ScoreInfo> scores)
        => new GameplayScreen(map, mods) { Scores = scores }
           .RegisterCapability(new ScoreSubmissionCapability())
           .RegisterCapability(new SpectatorSendCapability());

    #endregion

    public GameplayScreen RegisterCapability(IGameplayCapability capability)
    {
        capability.Screen = this;
        capabilities.Add(capability);

        return this;
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    [BackgroundDependencyLoader]
    private void load(ITrackStore tracks)
    {
        capabilities.ForEach(x =>
        {
            x.PreLoad();

            if (x is Drawable draw)
                AddInternal(draw);
        });

        dependencies.CacheAs(this);

        hudVisibility = Config.GetBindable<HudVisibility>(FluXisSetting.HudVisibility);
        backgroundDim = Config.GetBindable<float>(FluXisSetting.BackgroundDim);
        backgroundBlur = Config.GetBindable<float>(FluXisSetting.BackgroundBlur);
        bgaEnabled = Config.GetBindable<bool>(FluXisSetting.BackgroundVideo);

        Map = loadMap();

        if (Map == null)
        {
            notifications.SendError("Map could not be loaded");
            ValidForPush = false;
            return;
        }

        if (!Map.Validate(out var issue))
        {
            notifications.SendError(issue);
            ValidForPush = false;
            return;
        }

        Map.Sort();
        MapEvents = Map.GetMapEvents(Mods, true);

        getKeyCountFromEvents();

        dependencies.CacheAs(Samples);

        var colors = Map.Colors.JsonCopy();

        if (RealmMap.Settings.DisableColors)
            colors.PrimaryHex = colors.SecondaryHex = colors.MiddleHex = "";

        dependencies.CacheAs<ICustomColorProvider>(colors);

        var shaders = buildShaders();
        var transforms = shaders.TransformHandlers.ToList();

        clockContainer = new GameplayClockContainer(tracks, RealmMap, Map, new Drawable[]
        {
            new FlashOverlay(MapEvents.FlashEvents.Where(e => e.InBackground).ToList()),
            RulesetContainer = createRuleset().With(x =>
            {
                x.ScrollSpeed = new Bindable<float>(Config.Get<float>(FluXisSetting.ScrollSpeed));
            }),
            ReplayRecorder = new ReplayRecorder()
        }.Concat(transforms), UseGlobalOffset);

        RulesetContainer.ParentClock = clockContainer.GameplayClock;
        RulesetContainer.IsPaused.BindTo(IsPaused);
        RulesetContainer.ShakeTarget = this;
        RulesetContainer.OnDeath += onDeath;

        dependencies.Cache(PlayfieldManager = RulesetContainer.PlayfieldManager);

        dependencies.Cache(GameplayClock = clockContainer.GameplayClock);
        dependencies.CacheAs<IBeatSyncProvider>(GameplayClock);
        LoadComponent(GameplayClock);

        var storyboard = Map.CreateDrawableStoryboard() ?? new DrawableStoryboard(Map, new Storyboard(), ".");
        LoadComponent(storyboard);

        var camera = new CameraContainer(MapEvents.Where(x => x is ICameraEvent).Cast<ICameraEvent>().ToList());

        AddRangeInternal([
            keybindContainer = new GameplayKeybindContainer(realm, RealmMap.KeyCount, Map.IsDual)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new[]
                {
                    camera.CreateProxyDrawable().With(x => x.Clock = GameplayClock),
                    Samples,
                    dependencies.CacheAsAndReturn(Hitsounding = new Hitsounding(RealmMap.MapSet, Map.HitSoundFades, GameplayClock.RateBindable) { Clock = GameplayClock }),
                    shaders.AddContent(new Drawable[]
                    {
                        new AspectRatioContainer(Map.Force16By9)
                        {
                            Children = new Drawable[]
                            {
                                camera.WithChildren(new Drawable[]
                                {
                                    new DrawSizePreservingFillContainer
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        TargetDrawSize = new Vector2(1920, 1080),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Children = new Drawable[]
                                        {
                                            new Container
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = ColourInfo.GradientHorizontal(Color4.White, Color4.Black).Interpolate(new Vector2(BackgroundDim, 0)),
                                                Children = new Drawable[]
                                                {
                                                    background = new GlobalBackground
                                                    {
                                                        DefaultMap = RealmMap,
                                                        InitialBlur = BackgroundBlur
                                                    },
                                                    backgroundVideo = new BackgroundVideo
                                                    {
                                                        Clock = GameplayClock
                                                    },
                                                    new DrawableStoryboardLayer(GameplayClock, storyboard, StoryboardLayer.Background),
                                                }
                                            },
                                            new ComboBurst(RulesetContainer),
                                            clockContainer,
                                            new DrawableStoryboardLayer(GameplayClock, storyboard, StoryboardLayer.Foreground)
                                        }
                                    },
                                    hud = new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Child = new GameplayHUD(RulesetContainer)
                                    },
                                    new DrawSizePreservingFillContainer
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        TargetDrawSize = new Vector2(1920, 1080),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Child = new DrawableStoryboardLayer(GameplayClock, storyboard, StoryboardLayer.Overlay)
                                    }
                                }),
                                new PulseEffect(MapEvents.PulseEvents) { Clock = GameplayClock },
                                new FlashOverlay(MapEvents.FlashEvents.Where(e => !e.InBackground).ToList()) { Clock = GameplayClock },
                            }
                        }
                    }),
                    new DangerHealthOverlay(),
                    new SkipOverlay(),
                    failMenu = new FailMenu(),
                    fcOverlay = new FullComboOverlay(),
                    quickActionOverlay = new QuickActionOverlay(),
                    new GameplayTouchInput(RulesetContainer.Input),
                    new PauseMenu()
                },
            },
            Debug = new DebugText()
        ]);

        clockContainer.Add(new BeatPulseManager(Map, MapEvents.BeatPulseEvents, keybindContainer));

        backgroundVideo.LoadVideo(Map);

        RulesetContainer.Input.OnPress += ReplayRecorder.PressKey;
        RulesetContainer.Input.OnRelease += ReplayRecorder.ReleaseKey;

        capabilities.ForEach(x => x.PostLoad());
    }

    public void Add(Drawable draw) => AddInternal(draw);

    private ShaderStackContainer buildShaders()
    {
        var stack = new ShaderStackContainer();
        var shaders = MapEvents.ShaderEvents;
        var shaderTypes = shaders.Select(e => e.Type).Distinct().ToList();

        foreach (var shaderType in shaderTypes)
        {
            var shader = ShaderStackContainer.CreateForType(shaderType);

            if (shader == null)
            {
                Logger.Log($"Shader '{shaderType}' not found", LoggingTarget.Runtime, LogLevel.Error);
                continue;
            }

            var handler = stack.AddShader(shader);
            LoadComponent(handler);

            shaders.Where(x => x.Type == shaderType)
                   .ForEach(s => s.Apply(handler));
        }

        LoadComponent(stack);
        return stack;
    }

    private void showNotifications()
    {
        if (ValidForPush)
            notifications.Tasks?.FadeIn().MoveToX(0, Styling.TRANSITION_MOVE, Easing.OutQuint);
    }

    private void hideNotifications()
    {
        if (ValidForPush)
            notifications.Tasks?.MoveToX(-400, Styling.TRANSITION_MOVE, Easing.InQuint).Then().FadeOut();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        background.ParallaxStrength = 0;

        PlayfieldManager.OnFinish += () =>
        {
            if (PlayfieldManager.AnyFailed) return;

            if (PlayfieldManager.OnComplete())
                end();
        };

        bgaEnabled.BindValueChanged(e =>
        {
            if (e.NewValue)
                backgroundVideo.Start();
            else
                backgroundVideo.Stop();
        });
    }

    private void updatePausedState()
    {
        AllowOverlays.Value = IsPaused.Value;

        var uac = capabilities.OfType<IUserActivityCapability>().ToArray();
        UserActivity activity = null;

        foreach (var capability in uac)
        {
            activity = capability.Create();
            if (activity != null) break;
        }

        activity ??= !IsPaused.Value ? new UserActivity.Playing(this, RealmMap) : new UserActivity.Paused(this, RealmMap);
        uac.ForEach(x => x.Modify(activity));
        Activity.Value = activity;
    }

    private RulesetContainer createRuleset()
    {
        var rsc = capabilities.OfType<IRulesetCapability>().ToArray();
        RulesetContainer ruleset = null;

        foreach (var capability in rsc)
        {
            ruleset = capability.Create();
            if (ruleset != null) break;
        }

        ruleset ??= new RulesetContainer(Map, MapEvents, Mods) { CurrentPlayer = api.User.Value ?? APIUser.Default };
        rsc.ForEach(x => x.Modify(ruleset));
        return ruleset;
    }

    private MapInfo loadMap()
    {
        var mc = capabilities.OfType<IMapCapability>().ToArray();
        MapInfo map = null;

        foreach (var capability in mc)
        {
            map = capability.Load();
            if (map != null) break;
        }

        map ??= RealmMap.GetMapInfo(Mods);
        mc.ForEach(x => x.Modify(map));
        return map;
    }

    protected override void Update()
    {
        if (GameplayClock.CurrentTime >= 0 && starting)
        {
            starting = false;

            if (bgaEnabled.Value)
                backgroundVideo.Start();
        }

        var hudWasVisible = hudVisible;

        hudVisible = hudVisibility.Value switch
        {
            HudVisibility.Hidden => false,
            HudVisibility.ShowDuringBreaks => PlayfieldManager.InBreak.Value,
            HudVisibility.ShowDuringGameplay => !IsPaused.Value && !PlayfieldManager.InBreak.Value,
            _ => true
        };

        if (hudVisible != hudWasVisible)
            hud.FadeTo(hudVisible ? 1 : 0, 500, Easing.OutQuint);

        base.Update();
    }

    private bool failed;

    private void onDeath()
    {
        if (failed)
            return;

        if (Mods.Any(x => x is FlawlessMod))
        {
            RestartMap();
            return;
        }

        PlayfieldManager.Players.ForEach(p => p.HealthProcessor.Kill());
        failed = true;
        failMenu.Show();
        GameplayClock.RateTo(Rate * .75f, 2000, Easing.OutQuart);
    }

    private void end()
    {
        var field = PlayfieldManager.FirstPlayer;

        // no fail was enabled, but the player never actually failed
        // so, we just remove the mod to make the score count normally
        if (Mods.Any(m => m is NoFailMod) && !field.HealthProcessor.FailedAlready)
        {
            Mods.RemoveAll(m => m is NoFailMod);
            field.ScoreProcessor.Recalculate();
        }

        ReplayRecorder.IsRecording.Value = false;

        var showingOverlay = field.ScoreProcessor.FullCombo || field.ScoreProcessor.FullFlawless;

        if (showingOverlay)
        {
            fcOverlay.Show(field.ScoreProcessor.FullFlawless ? FullComboOverlay.FullComboType.AllFlawless : FullComboOverlay.FullComboType.FullCombo);
            Scheduler.AddDelayed(() => fcOverlay.Hide(), 1400);
        }

        var stopwatch = Stopwatch.StartNew();
        var ending = capabilities.OfType<IEndingCapability>().ToArray();

        var score = field.ScoreProcessor.ToScoreInfo();
        score.ScrollSpeed = Config.Get<float>(FluXisSetting.ScrollSpeed);

        Screen screen = null;
        nextHandler(0);

        return;

        void nextHandler(int index)
        {
            if (index >= ending.Length)
            {
                // ReSharper disable once AccessToModifiedClosure
                defaultBehaviour(screen);
                return;
            }

            var scr = ending[index].OnEnd(score, () => nextHandler(index + 1));
            if (scr != null) screen = scr;
        }

        void defaultBehaviour(Screen scr)
        {
            var minDelay = showingOverlay ? 2400 : 400;
            var elapsed = stopwatch.Elapsed.TotalMilliseconds;

            var delay = Math.Max(200, minDelay - elapsed);
            Schedule(() => this.Delay(delay).FadeOut(Styling.TRANSITION_FADE).OnComplete(_ =>
            {
                if (!this.IsCurrentScreen()) return;

                if (scr != null) this.Push(scr);
                else this.Exit();
            }));
        }
    }

    public void RestartMap()
    {
        if (Restarting || !AllowRestarting) return;

        Restarting = true;
        Samples.Restart();
        OnRestart?.Invoke();
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        ValidForResume = false;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        capabilities.ForEach(x => x.Exit());

        if (fftProcessor is not null) fftProcessor.Enabled.Value = true;
        this.FadeOut(Styling.TRANSITION_FADE);

        if (FadeBackToGlobalClock)
        {
            globalClock.RateTo(GameplayClock.Rate);
            ScheduleAfterChildren(() =>
            {
                globalClock.Seek(GameplayClock.CurrentTime);
                globalClock.RateTo(globalClock.Rate, Styling.TRANSITION_MOVE, Easing.InQuint);
            });
        }

        showNotifications();

        OnExit?.Invoke();
        GameplayClock.Stop();
        Samples.CancelFail();

        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        // there is absolutely no reason for our global fft processor to be active during gameplay
        if (fftProcessor is not null) fftProcessor.Enabled.Value = false;

        GameplayClock.Start();
        GameplayClock.RateTo(Rate, 0);

        hideNotifications();

        if (SubmitScore)
        {
            realm.RunWrite(r =>
            {
                var map = r.Find<RealmMap>(RealmMap.ID);

                if (map != null)
                    map.LastPlayed = DateTimeOffset.Now;
            });
        }

        IsPaused.BindValueChanged(_ => updatePausedState(), true);

        var pause = 2000;

        switch (e.Last)
        {
            case IntroAnimation:
                break;

            case GameplayLoader { HasRestarted: true }:
                pause = 800;
                this.FadeOut();
                break;

            default:
                this.ScaleTo(1.2f).FadeOut();
                break;
        }

        GameplayClock.Seek(GameplayStartTime - pause * Rate);

        using (BeginDelayedSequence(Styling.TRANSITION_ENTER_DELAY))
            this.ScaleTo(1f, Styling.TRANSITION_MOVE, Easing.OutQuint).FadeIn(Styling.TRANSITION_FADE);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Repeat || PlayfieldManager.Finished) return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.QuickRestart when !starting && !Restarting && AllowRestarting:
                quickActionOverlay.OnConfirm = RestartMap;
                quickActionOverlay.IsHolding = true;
                return true;

            case FluXisGlobalKeybind.QuickExit:
                quickActionOverlay.OnConfirm = this.Exit;
                quickActionOverlay.IsHolding = true;
                return true;

            case FluXisGlobalKeybind.GameplayPause when AllowPausing:
                if (PlayfieldManager.AnyFailed) return false;

                if (InstantlyExitOnPause)
                {
                    this.Exit();
                    return true;
                }

                if (!Mods.Any(m => m is PausedMod))
                    Mods.Add(new PausedMod());

                IsPaused.Toggle();
                return true;

            case FluXisGlobalKeybind.Skip:
                if (Map.StartTime - GameplayClock.CurrentTime < 2000) return false;

                GameplayClock.Seek(Map.StartTime - 2000);
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action is FluXisGlobalKeybind.QuickRestart or FluXisGlobalKeybind.QuickExit)
            quickActionOverlay.IsHolding = false;
    }

    private void getKeyCountFromEvents()
    {
        foreach (var switchEvent in MapEvents.LaneSwitchEvents)
        {
            if (Map.InitialKeyCount == 0)
                Map.InitialKeyCount = switchEvent.Count;
        }

        if (Map.InitialKeyCount == 0)
            Map.InitialKeyCount = RealmMap.KeyCount;
    }
}
