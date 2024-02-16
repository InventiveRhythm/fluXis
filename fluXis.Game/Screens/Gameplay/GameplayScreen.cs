using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Audio.Transforms;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Configuration;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.API.Requests.Scores;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Replays;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Scoring.Processing;
using fluXis.Game.Scoring.Processing.Health;
using fluXis.Game.Screens.Gameplay.Audio;
using fluXis.Game.Screens.Gameplay.HUD;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Game.Screens.Gameplay.Overlay;
using fluXis.Game.Screens.Gameplay.Overlay.Effect;
using fluXis.Game.Screens.Gameplay.UI;
using fluXis.Game.Screens.Gameplay.UI.Menus;
using fluXis.Game.Screens.Result;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Gameplay;

public partial class GameplayScreen : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override float ParallaxStrength => 0f;
    public override float BackgroundBlur => backgroundBlur?.Value ?? 0f;
    public override float BackgroundDim => backgroundDim?.Value ?? .4f;
    public override bool ShowToolbar => false;
    public override bool AllowMusicControl => false;
    public override bool ApplyValuesAfterLoad => true;
    public override bool AllowExit => false;

    protected virtual double GameplayStartTime => 0;
    protected virtual bool InstantlyExitOnPause => false;
    public virtual bool FadeBackToGlobalClock => true;
    public virtual bool SubmitScore => true;
    protected virtual bool UseGlobalOffset => true;

    [Resolved]
    protected FluXisConfig Config { get; private set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private GlobalClock globalClock { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    private DependencyContainer dependencies;

    private bool starting = true;
    private bool restarting;

    public BindableBool IsPaused { get; } = new();
    public GameplaySamples Samples { get; } = new();
    public Hitsounding Hitsounding { get; private set; }

    public GameplayClock GameplayClock { get; private set; }
    public Action<double, double> OnSeek { get; set; }

    public JudgementProcessor JudgementProcessor { get; } = new();
    public HealthProcessor HealthProcessor { get; private set; }
    public ScoreProcessor ScoreProcessor { get; private set; }

    public GameplayInput Input { get; private set; }
    public HitWindows HitWindows { get; private set; }
    public ReleaseWindows ReleaseWindows { get; private set; }

    public GameplayLoader Loader { get; set; }
    public Action OnRestart { get; set; }
    private ReplayRecorder replayRecorder;

    public MapInfo Map { get; private set; }
    public RealmMap RealmMap { get; }
    public MapEvents MapEvents { get; private set; }
    public List<IMod> Mods { get; }

    private GameplayKeybindContainer keybindContainer;
    private BackgroundVideo backgroundVideo;
    private GameplayClockContainer clockContainer;
    public Playfield Playfield { get; private set; }
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
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

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

        MapEvents = Map.GetMapEvents();
        Map.Sort();
        getKeyCountFromEvents();

        var difficulty = Map.AccuracyDifficulty == 0 ? 8 : Map.AccuracyDifficulty;
        difficulty *= Mods.Any(m => m is HardMod) ? 1.5f : 1;

        HitWindows = new HitWindows(difficulty, Rate);
        ReleaseWindows = new ReleaseWindows(difficulty, Rate);

        JudgementProcessor.AddDependants(new JudgementDependant[]
        {
            HealthProcessor = CreateHealthProcessor(),
            ScoreProcessor = new ScoreProcessor
            {
                HitWindows = HitWindows,
                Map = RealmMap,
                MapInfo = Map,
                Mods = Mods
            }
        });

        HealthProcessor.CanFail = !Mods.Any(m => m is NoFailMod);

        dependencies.CacheAs(Input = GetInput());
        dependencies.Cache(Playfield = new Playfield());

        clockContainer = new GameplayClockContainer(RealmMap, Map, new Drawable[]
        {
            new FlashOverlay(MapEvents.FlashEvents.Where(e => e.InBackground).ToList()),
            new PulseEffect(),
            Playfield,
            new LaneSwitchAlert(),
            replayRecorder = new ReplayRecorder()
        }, UseGlobalOffset);

        dependencies.Cache(GameplayClock = clockContainer.GameplayClock);

        InternalChildren = new Drawable[]
        {
            keybindContainer = new GameplayKeybindContainer(realm, RealmMap.KeyCount)
            {
                Children = new[]
                {
                    Input,
                    Samples,
                    Hitsounding = new Hitsounding(RealmMap.MapSet, GameplayClock.RateBindable),
                    new DrawSizePreservingFillContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        TargetDrawSize = new Vector2(1920, 1080),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            backgroundVideo = new BackgroundVideo
                            {
                                Clock = GameplayClock,
                                Map = RealmMap,
                                Info = Map
                            },
                            clockContainer,
                            new KeyOverlay()
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
                    CreateTextOverlay(),
                    new DangerHealthOverlay(),
                    new FlashOverlay(MapEvents.FlashEvents.Where(e => !e.InBackground).ToList()) { Clock = GameplayClock },
                    new SkipOverlay(),
                    failMenu = new FailMenu(),
                    fcOverlay = new FullComboOverlay(),
                    quickActionOverlay = new QuickActionOverlay(),
                    new GameplayTouchInput(),
                    new PauseMenu()
                },
            },
            scoreSubmissionOverlay = new ScoreSubmissionOverlay()
        };

        backgroundVideo.LoadVideo();

        Input.OnPress += replayRecorder.PressKey;
        Input.OnRelease += replayRecorder.ReleaseKey;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        JudgementProcessor.ApplyMap(Map);
        ScoreProcessor.OnComboBreak += () =>
        {
            if (!Playfield.Manager.Seeking)
                Samples.Miss();
        };

        Playfield.Manager.OnFinished = () =>
        {
            if (HealthProcessor.Failed) return;

            if (HealthProcessor.OnComplete())
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

    private void updateRpc() => Activity.Value = !IsPaused.Value ? GetPlayingActivity() : new UserActivity.Paused(RealmMap);

    protected virtual MapInfo LoadMap() => RealmMap.GetMapInfo();
    protected virtual GameplayInput GetInput() => new(this, RealmMap.KeyCount);
    protected virtual Drawable CreateTextOverlay() => Empty();
    protected virtual UserActivity GetPlayingActivity() => new UserActivity.Playing(RealmMap);

    protected HealthProcessor CreateHealthProcessor()
    {
        var processor = null as HealthProcessor;

        if (Mods.Any(m => m is HardMod)) processor = new DrainHealthProcessor();
        else if (Mods.Any(m => m is EasyMod)) processor = new RequirementHeathProcessor { HealthRequirement = EasyMod.HEALTH_REQUIREMENT };

        processor ??= new HealthProcessor();
        processor.Screen = this;
        processor.OnFail = OnDeath;

        if (Mods.Any(m => m is FragileMod))
            processor.ExtraFailCondition = r => r.Judgement == Judgement.Miss;

        if (Mods.Any(m => m is FlawlessMod))
            processor.ExtraFailCondition = r => r.Judgement < Judgement.Flawless;

        return processor;
    }

    protected override void Update()
    {
        HealthProcessor.Update();

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
            HudVisibility.ShowDuringBreaks => Playfield.Manager.Break,
            HudVisibility.ShowDuringGameplay => !IsPaused.Value && !Playfield.Manager.Break,
            _ => true
        };

        if (hudVisible != hudWasVisible)
            hud.FadeTo(hudVisible ? 1 : 0, 500, Easing.OutQuint);

        base.Update();
    }

    public virtual void OnDeath()
    {
        failMenu.Show();
        GameplayClock.RateTo(.4f, 2000, Easing.OutQuart);
    }

    protected virtual void End()
    {
        // no fail was enabled, but the player never actually failed
        // so we just remove the mod to make the score count normally
        if (Mods.Any(m => m is NoFailMod) && !HealthProcessor.FailedAlready)
        {
            Mods.RemoveAll(m => m is NoFailMod);
            ScoreProcessor.Recalculate();
        }

        if (ScoreProcessor.FullCombo || ScoreProcessor.FullFlawless)
            fcOverlay.Show(ScoreProcessor.FullFlawless ? FullComboOverlay.FullComboType.AllFlawless : FullComboOverlay.FullComboType.FullCombo);

        keybindContainer.Delay(400).FadeOut(400);
        if (SubmitScore) scoreSubmissionOverlay.FadeIn(400);

        Task.Run(() =>
        {
            var player = fluxel.LoggedInUser;

            var bestScore = realm.Run(r =>
            {
                var onMap = r.All<RealmScore>().Where(s => s.MapID == RealmMap.ID).ToList();
                var best = onMap.MaxBy(s => s.Score);
                return best.Detach();
            });

            var score = ScoreProcessor.ToScoreInfo();
            score.ScrollSpeed = Config.Get<float>(FluXisSetting.ScrollSpeed);

            var screen = new SoloResults(RealmMap, score, player);
            screen.OnRestart = OnRestart;
            if (bestScore != null) screen.ComparisonScore = bestScore.ToScoreInfo();

            if (Mods.All(m => m.Rankable) && SubmitScore && RealmMap.Status < 100)
            {
                realm.RunWrite(r =>
                {
                    var rScore = r.Add(RealmScore.Create(RealmMap, player, score));

                    if (rScore != null)
                        SaveReplay(rScore.ID);
                });

                var request = new ScoreSubmitRequest(score);
                screen.SubmitRequest = request;
                request.Perform(fluxel);
                Schedule(() => scoreSubmissionOverlay.FadeOut(400));
            }

            Schedule(() => this.Delay(600).FadeOut(400).OnComplete(_ => this.Push(screen)));
        });
    }

    protected void SaveReplay(Guid scoreID)
    {
        try
        {
            var replay = replayRecorder.Replay;
            replay.PlayerID = fluxel.LoggedInUser?.ID ?? -1;
            var folder = storage.GetFullPath("replays");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, $"{scoreID}.frp");
            Logger.Log($"Saving replay to {path}", LoggingTarget.Runtime, LogLevel.Debug);
            File.WriteAllText(path, replay.Serialize());
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to save replay!");
        }
    }

    public virtual void RestartMap()
    {
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
            globalClock.LowPassFilter.CutoffTo(LowPassFilter.MAX, 500);
            globalClock.RateTo(GameplayClock.Rate, 0);
            ScheduleAfterChildren(() =>
            {
                globalClock.Seek(GameplayClock.CurrentTime);
                globalClock.RateTo(Rate, 500, Easing.InQuint);
            });
        }
        else
        {
            // we definetly want to reset the cutoff
            globalClock.LowPassFilter.CutoffTo(LowPassFilter.MAX);
        }

        GameplayClock.Stop();
        Samples.CancelFail();

        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        GameplayClock.Start();
        GameplayClock.Seek(GameplayStartTime - 2000 * Rate);
        GameplayClock.RateTo(Rate, 0);

        IsPaused.BindValueChanged(_ => updateRpc(), true);

        this.ScaleTo(1.2f).FadeOut()
            .ScaleTo(1f, 800, Easing.OutQuint).FadeIn(400);
    }

    private void fadeOut() => this.FadeOut(restarting ? 100 : 300);

    public virtual bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Repeat || Playfield.Manager.Finished) return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.QuickRestart when !starting && !restarting:
                quickActionOverlay.OnConfirm = RestartMap;
                quickActionOverlay.IsHolding = true;
                return true;

            case FluXisGlobalKeybind.QuickExit:
                quickActionOverlay.OnConfirm = this.Exit;
                quickActionOverlay.IsHolding = true;
                return true;

            case FluXisGlobalKeybind.GameplayPause:
                if (HealthProcessor.Failed) return false;

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
