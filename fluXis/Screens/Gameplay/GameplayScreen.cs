using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Audio;
using fluXis.Audio.Transforms;
using fluXis.Configuration;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Background;
using fluXis.Graphics.Shaders;
using fluXis.Graphics.Shaders.Bloom;
using fluXis.Graphics.Shaders.Chromatic;
using fluXis.Graphics.Shaders.Glitch;
using fluXis.Graphics.Shaders.Greyscale;
using fluXis.Graphics.Shaders.HueShift;
using fluXis.Graphics.Shaders.Invert;
using fluXis.Graphics.Shaders.Mosaic;
using fluXis.Graphics.Shaders.Noise;
using fluXis.Graphics.Shaders.Retro;
using fluXis.Graphics.Shaders.Vignette;
using fluXis.Graphics.Shaders.SplitScreen;
using fluXis.Graphics.Shaders.FishEye;
using fluXis.Graphics.Shaders.Reflections;
using fluXis.Graphics.Shaders.Perspective;
using fluXis.Input;
using fluXis.Map;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Mods;
using fluXis.Online;
using fluXis.Online.Activity;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Scores;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Notifications;
using fluXis.Replays;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Audio;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Gameplay.Input;
using fluXis.Screens.Gameplay.Overlay;
using fluXis.Screens.Gameplay.Overlay.Effect;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Screens.Gameplay.UI;
using fluXis.Screens.Gameplay.UI.Menus;
using fluXis.Screens.Intro;
using fluXis.Screens.Result;
using fluXis.Skinning.Default;
using fluXis.Storyboards;
using fluXis.Storyboards.Drawables;
using fluXis.Utils;
using fluXis.Utils.Extensions;
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
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Screens.Gameplay;

public partial class GameplayScreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
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

    protected virtual double GameplayStartTime => 0;
    protected virtual bool AllowRestart => true;
    protected virtual bool AllowPausing => true;
    protected virtual bool DisplayHUD => true;
    protected virtual bool InstantlyExitOnPause => false;
    protected virtual bool SubmitScore => true;
    protected virtual bool UseGlobalOffset => true;
    public virtual bool FadeBackToGlobalClock => true;

    protected bool CursorVisible { get; set; }

    [Resolved]
    protected FluXisConfig Config { get; private set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private ScoreManager scores { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private GlobalClock globalClock { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    protected UserCache Users { get; private set; }

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
    private ReplayRecorder replayRecorder;

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
    private ScoreSubmissionOverlay scoreSubmissionOverlay;

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

    public GameplayScreen(RealmMap realmMap, List<IMod> mods)
    {
        RealmMap = realmMap;
        Mods = mods;

        Rate = ((RateMod)Mods.Find(m => m is RateMod))?.Rate ?? 1f;
        Mods.RemoveAll(m => m is PausedMod);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    [BackgroundDependencyLoader]
    private void load(ITrackStore tracks)
    {
        dependencies.CacheAs(this);

        hudVisibility = Config.GetBindable<HudVisibility>(FluXisSetting.HudVisibility);
        backgroundDim = Config.GetBindable<float>(FluXisSetting.BackgroundDim);
        backgroundBlur = Config.GetBindable<float>(FluXisSetting.BackgroundBlur);
        bgaEnabled = Config.GetBindable<bool>(FluXisSetting.BackgroundVideo);

        Map = LoadMap();

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
        MapEvents = Map.GetMapEvents(Mods);
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
            RulesetContainer = CreateRuleset().With(x =>
            {
                x.ScrollSpeed = new Bindable<float>(Config.Get<float>(FluXisSetting.ScrollSpeed));
            }),
            replayRecorder = new ReplayRecorder()
        }.Concat(transforms), UseGlobalOffset);

        RulesetContainer.ParentClock = clockContainer.GameplayClock;
        RulesetContainer.IsPaused.BindTo(IsPaused);
        RulesetContainer.ShakeTarget = this;
        RulesetContainer.OnDeath += OnDeath;

        dependencies.Cache(PlayfieldManager = RulesetContainer.PlayfieldManager);

        dependencies.Cache(GameplayClock = clockContainer.GameplayClock);
        dependencies.CacheAs<IBeatSyncProvider>(GameplayClock);
        LoadComponent(GameplayClock);

        var storyboard = Map.CreateDrawableStoryboard() ?? new DrawableStoryboard(Map, new Storyboard(), ".");
        LoadComponent(storyboard);

        var camera = new CameraContainer(MapEvents.Where(x => x is ICameraEvent).Cast<ICameraEvent>().ToList());

        InternalChildren = new Drawable[]
        {
            keybindContainer = new GameplayKeybindContainer(realm, RealmMap.KeyCount, Map.IsDual)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new[]
                {
                    camera.CreateProxyDrawable().With(x => x.Clock = GameplayClock),
                    Samples,
                    dependencies.CacheAsAndReturn(Hitsounding = new Hitsounding(RealmMap.MapSet, Map.HitSoundFades, GameplayClock.RateBindable) { Clock = GameplayClock }),
                    shaders.AddContent(new[]
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
                                            new DrawableStoryboardWrapper(GameplayClock, storyboard, StoryboardLayer.Background),
                                        }
                                    },
                                    new ComboBurst(RulesetContainer),
                                    clockContainer,
                                    new DrawableStoryboardWrapper(GameplayClock, storyboard, StoryboardLayer.Foreground)
                                }
                            },
                            hud = new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Alpha = DisplayHUD ? 1 : 0,
                                Child = new GameplayHUD(RulesetContainer)
                            },
                            new DrawSizePreservingFillContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                TargetDrawSize = new Vector2(1920, 1080),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Child = new DrawableStoryboardWrapper(GameplayClock, storyboard, StoryboardLayer.Overlay)
                            }
                        }),
                        CreateTextOverlay(),
                        new DangerHealthOverlay(),
                        new PulseEffect(MapEvents.PulseEvents) { Clock = GameplayClock },
                        new FlashOverlay(MapEvents.FlashEvents.Where(e => !e.InBackground).ToList()) { Clock = GameplayClock },
                        new SkipOverlay(),
                    }),
                    failMenu = new FailMenu(),
                    fcOverlay = new FullComboOverlay(),
                    quickActionOverlay = new QuickActionOverlay(),
                    new GameplayTouchInput(RulesetContainer.Input),
                    new PauseMenu()
                },
            },
            scoreSubmissionOverlay = new ScoreSubmissionOverlay(),
            Debug = new DebugText()
        };

        clockContainer.Add(new BeatPulseManager(Map, MapEvents.BeatPulseEvents, keybindContainer));

        backgroundVideo.LoadVideo(Map);

        RulesetContainer.Input.OnPress += replayRecorder.PressKey;
        RulesetContainer.Input.OnRelease += replayRecorder.ReleaseKey;
    }

    private ShaderStackContainer buildShaders()
    {
        var stack = new ShaderStackContainer();
        var shaders = MapEvents.ShaderEvents;
        var shaderTypes = shaders.Select(e => e.Type).Distinct().ToList();

        LoadComponent(stack);

        foreach (var shaderType in shaderTypes)
        {
            ShaderContainer shader = shaderType switch
            {
                ShaderType.Chromatic => new ChromaticContainer(),
                ShaderType.Greyscale => new GreyscaleContainer(),
                ShaderType.Invert => new InvertContainer(),
                ShaderType.Bloom => new BloomContainer(),
                ShaderType.Mosaic => new MosaicContainer(),
                ShaderType.Noise => new NoiseContainer(),
                ShaderType.Vignette => new VignetteContainer(),
                ShaderType.Retro => new RetroContainer(),
                ShaderType.HueShift => new HueShiftContainer(),
                ShaderType.Glitch => new GlitchContainer(),
                ShaderType.SplitScreen => new SplitScreenContainer(),
                ShaderType.FishEye => new FishEyeContainer(),
                ShaderType.Reflections => new ReflectionsContainer(),
                ShaderType.Perspective => new PerspectiveContainer(),
                _ => null
            };

            if (shader == null)
            {
                Logger.Log($"Shader '{shaderType}' not found", LoggingTarget.Runtime, LogLevel.Error);
                continue;
            }

            shader.RelativeSizeAxes = Axes.Both;
            var handler = stack.AddShader(shader);
            LoadComponent(handler);

            shaders.Where(x => x.Type == shaderType)
                   .ForEach(s => s.Apply(handler));
        }

        return stack;
    }

    private void showNotifcations()
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
                End();
        };

        bgaEnabled.BindValueChanged(e =>
        {
            if (e.NewValue)
                backgroundVideo.Start();
            else
                backgroundVideo.Stop();
        });
    }

    protected virtual void UpdatePausedState()
    {
        Activity.Value = !IsPaused.Value ? GetPlayingActivity() : new UserActivity.Paused(this, RealmMap);
        AllowOverlays.Value = IsPaused.Value;
    }

    protected virtual RulesetContainer CreateRuleset() => new(Map, MapEvents, Mods) { CurrentPlayer = api.User.Value ?? APIUser.Default };
    protected virtual MapInfo LoadMap() => RealmMap.GetMapInfo(Mods);
    protected virtual Drawable CreateTextOverlay() => Empty();
    protected virtual UserActivity GetPlayingActivity() => new UserActivity.Playing(this, RealmMap);

    protected override void Update()
    {
        if (GameplayClock.CurrentTime >= 0 && starting)
        {
            starting = false;

            if (bgaEnabled.Value)
                backgroundVideo.Start();
        }

        if (DisplayHUD)
        {
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
        }

        base.Update();
    }

    private bool failed;

    public virtual void OnDeath()
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

    protected virtual void End()
    {
        var field = PlayfieldManager.FirstPlayer;

        // no fail was enabled, but the player never actually failed
        // so, we just remove the mod to make the score count normally
        if (Mods.Any(m => m is NoFailMod) && !field.HealthProcessor.FailedAlready)
        {
            Mods.RemoveAll(m => m is NoFailMod);
            field.ScoreProcessor.Recalculate();
        }

        replayRecorder.IsRecording.Value = false;

        var showingOverlay = field.ScoreProcessor.FullCombo || field.ScoreProcessor.FullFlawless;

        var stopwatch = Stopwatch.StartNew();

        if (showingOverlay)
        {
            fcOverlay.Show(field.ScoreProcessor.FullFlawless ? FullComboOverlay.FullComboType.AllFlawless : FullComboOverlay.FullComboType.FullCombo);
            Scheduler.AddDelayed(() => fcOverlay.Hide(), 1400);
        }

        var canBeUploaded = Mods.All(m => m.Rankable) && RealmMap.StatusInt < 100 && !RealmMap.MapSet.AutoImported;

        if (SubmitScore && canBeUploaded)
            scoreSubmissionOverlay.FadeIn(Styling.TRANSITION_FADE);

        Task.Run(() =>
        {
            var bestScore = scores.GetCurrentTop(RealmMap.ID);

            var score = field.ScoreProcessor.ToScoreInfo();
            score.ScrollSpeed = Config.Get<float>(FluXisSetting.ScrollSpeed);

            var screen = new SoloResults(RealmMap, score, api.User.Value ?? APIUser.Default);
            screen.OnRestart = OnRestart;
            if (bestScore != null) screen.ComparisonScore = bestScore.ToScoreInfo();

            if (Mods.All(m => m.SaveScore) && SubmitScore && !RealmMap.MapSet.AutoImported)
            {
                var id = scores.Add(RealmMap.ID, score).ID;
                var replay = SaveReplay(id);

                if (canBeUploaded)
                {
                    var request = new ScoreSubmitRequest(score, Mods, replay, Map.Hash, Map.EffectHash, Map.StoryboardHash);
                    screen.SubmitRequest = request;
                    api.PerformRequest(request);

                    var resData = request.Response?.Data;

                    if (request.IsSuccessful && resData?.Score != null)
                        scores.UpdateOnlineID(id, resData.Score.ID);

                    Schedule(() => scoreSubmissionOverlay.FadeOut(Styling.TRANSITION_FADE));
                }
            }

            var minDelay = showingOverlay ? 2400 : 400;
            var elapsed = stopwatch.Elapsed.TotalMilliseconds;

            var delay = Math.Max(200, minDelay - elapsed);
            Schedule(() => this.Delay(delay).FadeOut(Styling.TRANSITION_FADE).OnComplete(_ => this.Push(screen)));
        });
    }

    protected Replay SaveReplay(Guid scoreID)
    {
        try
        {
            var replay = replayRecorder.Replay;
            replay.PlayerID = api.User.Value?.ID ?? -1;
            var folder = storage.GetFullPath("replays");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, $"{scoreID}.frp");
            File.WriteAllText(path, replay.Serialize());

            return replay;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to save replay!");
        }

        return null;
    }

    public virtual void RestartMap()
    {
        if (Restarting || !AllowRestart) return;

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

        showNotifcations();

        OnExit?.Invoke();
        GameplayClock.Stop();
        Samples.CancelFail();

        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
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

        IsPaused.BindValueChanged(_ => UpdatePausedState(), true);

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

    public virtual bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Repeat || PlayfieldManager.Finished) return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.QuickRestart when !starting && !Restarting && AllowRestart:
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

                // only add when we have hit at least one note
                if (!Mods.Any(m => m is PausedMod) && GameplayClock.CurrentTime > Map.StartTime)
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
