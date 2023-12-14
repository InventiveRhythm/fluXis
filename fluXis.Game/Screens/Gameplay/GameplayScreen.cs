using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.API.Requests.Scores;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Scoring.Processing;
using fluXis.Game.Scoring.Processing.Health;
using fluXis.Game.Screens.Gameplay.HUD;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Game.Screens.Gameplay.Overlay;
using fluXis.Game.Screens.Gameplay.Overlay.Effect;
using fluXis.Game.Screens.Gameplay.UI;
using fluXis.Game.Screens.Gameplay.UI.Menus;
using fluXis.Game.Screens.Result;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

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
    protected virtual bool InstantlyExitOnPause => Playfield.Manager.AutoPlay;

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    public AudioClock AudioClock { get; private set; }

    private DependencyContainer dependencies;

    private bool starting = true;
    private bool restarting;

    public BindableBool IsPaused { get; } = new();
    public GameplaySamples Samples { get; } = new();
    public Hitsounding Hitsounding { get; private set; }

    public Action<double, double> OnSeek { get; set; }

    public JudgementProcessor JudgementProcessor { get; } = new();
    public HealthProcessor HealthProcessor { get; private set; }
    public ScoreProcessor ScoreProcessor { get; private set; }

    public GameplayInput Input { get; private set; }
    public HitWindows HitWindows { get; }
    public ReleaseWindows ReleaseWindows { get; }

    public GameplayLoader Loader { get; set; }
    public Action OnRestart { get; set; }

    public MapInfo Map { get; private set; }
    public RealmMap RealmMap { get; }
    public MapEvents MapEvents { get; private set; }
    public List<IMod> Mods { get; }

    public Playfield Playfield { get; private set; }
    private Container hud { get; set; }

    private FailMenu failMenu;
    private FullComboOverlay fcOverlay;
    private QuickActionOverlay quickActionOverlay;

    private FluXisConfig config;

    private Bindable<HudVisibility> hudVisibility;
    private Bindable<float> backgroundDim;
    private Bindable<float> backgroundBlur;

    private bool hudVisible = true;

    public float Rate { get; }

    public GameplayScreen(RealmMap realmMap, List<IMod> mods)
    {
        RealmMap = realmMap;
        Mods = mods;

        Rate = ((RateMod)Mods.Find(m => m is RateMod))?.Rate ?? 1f;
        Mods.RemoveAll(m => m is PausedMod);
        HitWindows = new HitWindows(Rate);
        ReleaseWindows = new ReleaseWindows(Rate);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config, ISampleStore samples)
    {
        dependencies.CacheAs(this);

        this.config = config;
        hudVisibility = config.GetBindable<HudVisibility>(FluXisSetting.HudVisibility);

        Map = LoadMap();

        if (Map == null || !Map.Validate(notifications))
        {
            ValidForPush = false;
            return;
        }

        MapEvents = Map.GetMapEvents();
        Map.Sort();
        getKeyCountFromEvents();
        backgrounds.SetVideoBackground(RealmMap, Map);

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
        Input = new GameplayInput(this, RealmMap.KeyCount);

        backgroundBlur = config.GetBindable<float>(FluXisSetting.BackgroundBlur);
        backgroundDim = config.GetBindable<float>(FluXisSetting.BackgroundDim);

        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        Playfield = new Playfield();
        dependencies.Cache(Playfield);

        InternalChild = new GameplayKeybindContainer(Game, realm)
        {
            Children = new Drawable[]
            {
                Input,
                Samples,
                Hitsounding = new Hitsounding(RealmMap.MapSet, AudioClock.RateBindable),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Clock = AudioClock,
                    Children = new Drawable[]
                    {
                        new FlashOverlay(MapEvents.FlashEvents.Where(e => e.InBackground).ToList()),
                        new PulseEffect(),
                        Playfield,
                        new LaneSwitchAlert()
                    }
                },
                hud = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new GameplayHUD(),
                        new ModsDisplay(),
                        new KeyOverlay()
                    }
                },
                new AutoPlayDisplay(),
                new DangerHealthOverlay(),
                new FlashOverlay(MapEvents.FlashEvents.Where(e => !e.InBackground).ToList()) { Clock = AudioClock },
                new SkipOverlay(),
                failMenu = new FailMenu(),
                fcOverlay = new FullComboOverlay(),
                quickActionOverlay = new QuickActionOverlay(),
                new GameplayTouchInput(),
                new PauseMenu()
            }
        };
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
    }

    private void updateRpc()
    {
        if (!IsPaused.Value)
            Activity.Value = Playfield.Manager.AutoPlay ? new UserActivity.WatchingReplay(RealmMap, APIUserShort.AutoPlay) : new UserActivity.Playing(RealmMap);
        else
            Activity.Value = new UserActivity.Paused(RealmMap);
    }

    protected virtual MapInfo LoadMap() => RealmMap.GetMapInfo();

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

        if (AudioClock.CurrentTime >= 0 && starting)
        {
            starting = false;
            backgrounds.StartVideo();
            AudioClock.Volume = 1; // just in case it was muted
        }

        var hudWasVisible = hudVisible;

        hudVisible = hudVisibility.Value switch
        {
            HudVisibility.Hidden => false,
            HudVisibility.ShowDuringBreaks => Playfield.Manager.Break,
            _ => true
        };

        if (hudVisible != hudWasVisible)
            hud.FadeTo(hudVisible ? 1 : 0, 500, Easing.OutQuint);

        base.Update();
    }

    public virtual void OnDeath()
    {
        failMenu.Show();
        AudioClock.RateTo(.4f, 2000, Easing.OutQuart);
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

        var player = Playfield.Manager.AutoPlay ? APIUserShort.AutoPlay : fluxel.LoggedInUser;

        if (ScoreProcessor.FullCombo || ScoreProcessor.FullFlawless)
            fcOverlay.Show(ScoreProcessor.FullFlawless ? FullComboOverlay.FullComboType.AllFlawless : FullComboOverlay.FullComboType.FullCombo);

        var bestScore = realm.Run(r =>
        {
            var onMap = r.All<RealmScore>().Where(s => s.MapID == RealmMap.ID).ToList();
            var best = onMap.MaxBy(s => s.Score);
            return best;
        });

        var score = ScoreProcessor.ToScoreInfo();
        score.ScrollSpeed = config.Get<float>(FluXisSetting.ScrollSpeed);

        var screen = new SoloResults(RealmMap, score, player);
        screen.OnRestart = OnRestart;
        if (bestScore != null) screen.ComparisonScore = bestScore.ToScoreInfo();

        if (Mods.All(m => m.Rankable))
        {
            realm.RunWrite(r => r.Add(RealmScore.Create(RealmMap, player, score)));

            var request = new ScoreSubmitRequest(score);
            screen.SubmitRequest = request;
            request.PerformAsync(fluxel);
        }

        this.Delay(1000).FadeOut(500).OnComplete(_ => this.Push(screen));
    }

    public virtual void RestartMap()
    {
        restarting = true;
        Samples.Restart();
        OnRestart?.Invoke();
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        fadeOut();
        backgrounds.StopVideo();
        ValidForResume = false;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        fadeOut();

        AudioClock.LowPassFilter.CutoffTo(LowPassFilter.MAX, 500);
        ScheduleAfterChildren(() => AudioClock.RateTo(Rate, 500, Easing.InQuint));
        backgrounds.StopVideo();
        Samples.CancelFail();

        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        AudioClock.Seek(GameplayStartTime - 2000 * Rate);
        AudioClock.RateTo(Rate, 0);

        IsPaused.BindValueChanged(_ => updateRpc(), true);

        this.ScaleTo(1.2f).FadeOut()
            .ScaleTo(1f, 800, Easing.OutQuint).FadeIn(400);
    }

    private void fadeOut() => this.FadeOut(restarting ? 100 : 300);

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
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
                if (!Mods.Any(m => m is PausedMod) && AudioClock.CurrentTime > Map.StartTime)
                    Mods.Add(new PausedMod());

                IsPaused.Toggle();
                return true;

            case FluXisGlobalKeybind.Skip:
                if (Map.StartTime - AudioClock.CurrentTime < 2000) return false;

                AudioClock.Seek(Map.StartTime - 2000);
                return true;

            case FluXisGlobalKeybind.ScrollSpeedIncrease:
                config.GetBindable<float>(FluXisSetting.ScrollSpeed).Value += 0.1f;
                return true;

            case FluXisGlobalKeybind.ScrollSpeedDecrease:
                config.GetBindable<float>(FluXisSetting.ScrollSpeed).Value -= 0.1f;
                return true;

            case FluXisGlobalKeybind.SeekBackward:
                OnSeek?.Invoke(AudioClock.CurrentTime, AudioClock.CurrentTime - 5000);
                return Playfield.Manager.AutoPlay;

            case FluXisGlobalKeybind.SeekForward:
                OnSeek?.Invoke(AudioClock.CurrentTime, AudioClock.CurrentTime + 5000);
                return Playfield.Manager.AutoPlay;
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
