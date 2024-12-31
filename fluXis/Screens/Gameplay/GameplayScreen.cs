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
using fluXis.Input;
using fluXis.Map;
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
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Gameplay.Audio;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Gameplay.HUD.Leaderboard;
using fluXis.Screens.Gameplay.Input;
using fluXis.Screens.Gameplay.Overlay;
using fluXis.Screens.Gameplay.Overlay.Effect;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Screens.Gameplay.UI;
using fluXis.Screens.Gameplay.UI.Menus;
using fluXis.Screens.Result;
using fluXis.Skinning.Default;
using fluXis.Storyboards;
using fluXis.Storyboards.Drawables;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
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
    public override bool ShowCursor => PlayfieldManager.Playfields[0].HealthProcessor.Failed || IsPaused.Value || CursorVisible;
    public override bool ApplyValuesAfterLoad => true;
    public override bool AllowExit => false;

    protected virtual double GameplayStartTime => 0;
    protected virtual bool InstantlyExitOnPause => false;
    protected virtual bool AllowRestart => true;
    public virtual bool AllowReverting => false;
    public virtual bool FadeBackToGlobalClock => true;
    public virtual bool SubmitScore => true;
    protected virtual bool UseGlobalOffset => true;
    public virtual APIUser CurrentPlayer => api.User.Value;

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
    private bool restarting;

    public event Action OnExit;

    public BindableBool IsPaused { get; } = new();
    public GameplaySamples Samples { get; } = new();
    public Hitsounding Hitsounding { get; private set; }

    public GameplayClock GameplayClock { get; private set; }
    public Action<double, double> OnSeek { get; set; }

    public GameplayInput Input { get; private set; }
    public HitWindows HitWindows { get; private set; }
    public ReleaseWindows ReleaseWindows { get; private set; }

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
    public PlayfieldManager PlayfieldManager { get; private set; }
    private Container hud { get; set; }
    private ScoreSubmissionOverlay scoreSubmissionOverlay;

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
    private void load()
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

        MapEvents = Map.GetMapEvents(Mods);
        Map.Sort();
        getKeyCountFromEvents();

        dependencies.CacheAs(Samples);
        dependencies.CacheAs<ICustomColorProvider>(Map.Colors);

        var difficulty = Math.Clamp(Map.AccuracyDifficulty == 0 ? 8 : Map.AccuracyDifficulty, 1, 10);
        difficulty *= Mods.Any(m => m is HardMod) ? 1.5f : 1;

        HitWindows = new HitWindows(difficulty, Rate);
        ReleaseWindows = new ReleaseWindows(difficulty, Rate);

        dependencies.CacheAs(Input = GetInput());

        var shaders = buildShaders();
        var transforms = shaders.TransformHandlers.ToList();

        clockContainer = new GameplayClockContainer(RealmMap, Map, new Drawable[]
        {
            dependencies.CacheAsAndReturn(new LaneSwitchManager(MapEvents.LaneSwitchEvents, RealmMap.KeyCount, Map.NewLaneSwitchLayout) { Mirror = Mods.Any(m => m is MirrorMod) }),
            new FlashOverlay(MapEvents.FlashEvents.Where(e => e.InBackground).ToList()),
            dependencies.CacheAsAndReturn(PlayfieldManager = new PlayfieldManager(Map.DualMode)),
            replayRecorder = new ReplayRecorder()
        }.Concat(transforms), UseGlobalOffset);

        var storyboard = Map.CreateDrawableStoryboard() ?? new DrawableStoryboard(new Storyboard(), ".");

        dependencies.Cache(GameplayClock = clockContainer.GameplayClock);
        dependencies.CacheAs<IBeatSyncProvider>(GameplayClock);

        LoadComponent(GameplayClock);
        LoadComponent(storyboard);

        InternalChildren = new Drawable[]
        {
            keybindContainer = new GameplayKeybindContainer(realm, RealmMap.KeyCount, Map.IsDual)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    Input,
                    Samples,
                    Hitsounding = new Hitsounding(RealmMap.MapSet, Map.HitSoundFades, GameplayClock.RateBindable) { Clock = GameplayClock },
                    shaders.AddContent(new[]
                    {
                        new DrawSizePreservingFillContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            TargetDrawSize = new Vector2(1920, 1080),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children = new Drawable[]
                            {
                                background = new GlobalBackground
                                {
                                    DefaultMap = RealmMap,
                                    InitialBlur = BackgroundBlur
                                },
                                backgroundVideo = new BackgroundVideo
                                {
                                    ShowDim = false,
                                    Clock = GameplayClock,
                                    Map = RealmMap,
                                    Info = Map
                                },
                                new DrawableStoryboardWrapper(GameplayClock, storyboard, StoryboardLayer.Background),
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.Black,
                                    Alpha = BackgroundDim
                                },
                                clockContainer,
                                new DrawableStoryboardWrapper(GameplayClock, storyboard, StoryboardLayer.Foreground)
                            }
                        },
                        hud = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new GameplayHUD(),
                                new ModsDisplay()
                            }
                        },
                        new GameplayLeaderboard(Scores),
                        CreateTextOverlay(),
                        new DangerHealthOverlay(),
                        new DrawableStoryboardWrapper(GameplayClock, storyboard, StoryboardLayer.Overlay),
                        new FlashOverlay(MapEvents.FlashEvents.Where(e => !e.InBackground).ToList()) { Clock = GameplayClock },
                        new SkipOverlay(),
                    }),
                    failMenu = new FailMenu(),
                    fcOverlay = new FullComboOverlay(),
                    quickActionOverlay = new QuickActionOverlay(),
                    new GameplayTouchInput(),
                    new PauseMenu()
                },
            },
            scoreSubmissionOverlay = new ScoreSubmissionOverlay(),
            Debug = new DebugText()
        };

        clockContainer.Add(new BeatPulseContainer(Map, MapEvents.BeatPulseEvents, keybindContainer));

        backgroundVideo.LoadVideo();

        Input.OnPress += replayRecorder.PressKey;
        Input.OnRelease += replayRecorder.ReleaseKey;
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
        }, true);
    }

    protected virtual void UpdatePausedState()
    {
        Activity.Value = !IsPaused.Value ? GetPlayingActivity() : new UserActivity.Paused(this, RealmMap);
        AllowOverlays.Value = IsPaused.Value;
    }

    protected virtual MapInfo LoadMap() => RealmMap.GetMapInfo(Mods);
    protected virtual GameplayInput GetInput() => new(this, RealmMap.KeyCount, Map.IsDual);
    protected virtual Drawable CreateTextOverlay() => Empty();
    protected virtual UserActivity GetPlayingActivity() => new UserActivity.Playing(this, RealmMap);

    public HealthProcessor CreateHealthProcessor()
    {
        var processor = null as HealthProcessor;

        var difficulty = Math.Clamp(Map.HealthDifficulty == 0 ? 8 : Map.HealthDifficulty, 1, 10);
        difficulty *= Mods.Any(m => m is HardMod) ? 1.2f : 1f;

        if (Mods.Any(m => m is HardMod)) processor = new DrainHealthProcessor(difficulty);
        else if (Mods.Any(m => m is EasyMod)) processor = new RequirementHeathProcessor(difficulty) { HealthRequirement = EasyMod.HEALTH_REQUIREMENT };

        processor ??= new HealthProcessor(difficulty);
        processor.Screen = this;
        processor.OnFail = OnDeath;

        foreach (var mod in Mods.OfType<IApplicableToHealthProcessor>())
            mod.Apply(processor);

        return processor;
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
            HudVisibility.ShowDuringBreaks => PlayfieldManager.InBreak,
            HudVisibility.ShowDuringGameplay => !IsPaused.Value && !PlayfieldManager.InBreak,
            _ => true
        };

        if (hudVisible != hudWasVisible)
            hud.FadeTo(hudVisible ? 1 : 0, 500, Easing.OutQuint);

        base.Update();
    }

    private bool failed;

    public virtual void OnDeath()
    {
        if (failed)
            return;

        PlayfieldManager.Playfields.ForEach(p => p.HealthProcessor.Kill());
        failed = true;
        failMenu.Show();
        GameplayClock.RateTo(Rate * .75f, 2000, Easing.OutQuart);
    }

    protected virtual void End()
    {
        var field = PlayfieldManager.Playfields[0];

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
            scoreSubmissionOverlay.FadeIn(FADE_DURATION);

        Task.Run(() =>
        {
            var bestScore = scores.GetCurrentTop(RealmMap.ID);

            var score = field.ScoreProcessor.ToScoreInfo();
            score.ScrollSpeed = Config.Get<float>(FluXisSetting.ScrollSpeed);

            var screen = new SoloResults(RealmMap, score, CurrentPlayer);
            screen.OnRestart = OnRestart;
            if (bestScore != null) screen.ComparisonScore = bestScore.ToScoreInfo();

            if (Mods.All(m => m.SaveScore) && SubmitScore && !RealmMap.MapSet.AutoImported)
            {
                var id = scores.Add(RealmMap.ID, score).ID;
                score.Replay = SaveReplay(id);

                if (canBeUploaded)
                {
                    var request = new ScoreSubmitRequest(score);
                    screen.SubmitRequest = request;
                    api.PerformRequest(request);

                    var resData = request.Response?.Data;

                    if (request.IsSuccessful && resData?.Score != null)
                        scores.UpdateOnlineID(id, resData.Score.ID);

                    Schedule(() => scoreSubmissionOverlay.FadeOut(FADE_DURATION));
                }
            }

            var minDelay = showingOverlay ? 2400 : 400;
            var elapsed = stopwatch.Elapsed.TotalMilliseconds;

            var delay = Math.Max(200, minDelay - elapsed);
            Schedule(() => this.Delay(delay).FadeOut(FADE_DURATION).OnComplete(_ => this.Push(screen)));
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
            Logger.Log($"Saving replay to {path}", LoggingTarget.Runtime, LogLevel.Debug);
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
        if (restarting || !AllowRestart) return;

        restarting = true;
        Samples.Restart();
        OnRestart?.Invoke();
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        ValidForResume = false;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        fadeOut();

        if (FadeBackToGlobalClock)
        {
            globalClock.RateTo(GameplayClock.Rate);
            ScheduleAfterChildren(() =>
            {
                globalClock.Seek(GameplayClock.CurrentTime);
                globalClock.RateTo(Rate, MOVE_DURATION, Easing.InQuint);
            });
        }

        OnExit?.Invoke();
        GameplayClock.Stop();
        Samples.CancelFail();

        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        GameplayClock.Start();
        GameplayClock.Seek(GameplayStartTime - 2000 * Rate);
        GameplayClock.RateTo(Rate, 0);

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

        this.ScaleTo(1.2f).FadeOut();

        using (BeginDelayedSequence(ENTER_DELAY))
            this.ScaleTo(1f, MOVE_DURATION, Easing.OutQuint).FadeIn(FADE_DURATION);
    }

    private void fadeOut() => this.FadeOut(restarting ? FADE_DURATION / 2 : FADE_DURATION);

    public virtual bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Repeat || PlayfieldManager.Finished) return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.QuickRestart when !starting && !restarting && AllowRestart:
                quickActionOverlay.OnConfirm = RestartMap;
                quickActionOverlay.IsHolding = true;
                return true;

            case FluXisGlobalKeybind.QuickExit:
                quickActionOverlay.OnConfirm = this.Exit;
                quickActionOverlay.IsHolding = true;
                return true;

            case FluXisGlobalKeybind.GameplayPause:
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

            case FluXisGlobalKeybind.ScrollSpeedIncrease:
                Config.GetBindable<float>(FluXisSetting.ScrollSpeed).Value += 0.1f;
                return true;

            case FluXisGlobalKeybind.ScrollSpeedDecrease:
                Config.GetBindable<float>(FluXisSetting.ScrollSpeed).Value -= 0.1f;
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
