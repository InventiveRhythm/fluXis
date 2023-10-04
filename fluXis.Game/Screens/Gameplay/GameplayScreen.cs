using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Activity;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Online.API.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Processing;
using fluXis.Game.Scoring.Processing.Health;
using fluXis.Game.Screens.Gameplay.HUD;
using fluXis.Game.Screens.Gameplay.HUD.Judgement;
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
using osuTK.Input;

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

    [Resolved]
    private ActivityManager activity { get; set; }

    private bool starting = true;
    private bool restarting;

    public BindableBool IsPaused { get; } = new();
    public GameplaySamples Samples { get; } = new();

    public Action<double, double> OnSeek { get; set; }

    public JudgementProcessor JudgementProcessor { get; } = new();
    public HealthProcessor HealthProcessor { get; private set; }
    public ScoreProcessor ScoreProcessor { get; private set; }

    public GameplayInput Input { get; private set; }
    public HitWindows HitWindows { get; }
    public ReleaseWindows ReleaseWindows { get; }

    public MapInfo Map { get; private set; }
    public RealmMap RealmMap { get; }
    public MapEvents MapEvents { get; private set; }
    public List<IMod> Mods { get; }

    public Playfield Playfield { get; private set; }
    private Container hud { get; set; }

    private FluXisTextFlow debugText;
    private static bool showDebug;

    private FailOverlay failOverlay;
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

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples, FluXisConfig config)
    {
        this.config = config;
        hudVisibility = config.GetBindable<HudVisibility>(FluXisSetting.HudVisibility);

        Map = LoadMap();

        if (Map == null)
        {
            this.Exit();
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
                HitWindows = HitWindows, Map = RealmMap, MapInfo = Map, Mods = Mods
            }
        });

        HealthProcessor.CanFail = !Mods.Any(m => m is NoFailMod);
        Input = new GameplayInput(this, Map.KeyCount);

        backgroundBlur = config.GetBindable<float>(FluXisSetting.BackgroundBlur);
        backgroundDim = config.GetBindable<float>(FluXisSetting.BackgroundDim);

        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChild = new GameplayKeybindContainer(Game, realm)
        {
            Children = new Drawable[]
            {
                Input,
                Samples,
                new FlashOverlay(MapEvents.FlashEvents.Where(e => e.InBackground).ToList())
                {
                    Clock = AudioClock
                },
                new PulseEffect
                {
                    ParentScreen = this, Clock = AudioClock
                },
                Playfield = new Playfield
                {
                    Screen = this, Clock = AudioClock
                },
                hud = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        createHudElement(new ComboCounter()),
                        createHudElement(new AccuracyDisplay()),
                        createHudElement(new Progressbar()),
                        createHudElement(new JudgementDisplay()),
                        createHudElement(new JudgementCounter()),
                        createHudElement(new HealthBar()),
                        createHudElement(new HitErrorBar()),
                        createHudElement(new AttributeText
                        {
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Margin = new MarginPadding(10)
                            {
                                Horizontal = 20
                            },
                            AttributeType = AttributeType.Title,
                            FontSize = 48
                        }),
                        createHudElement(new AttributeText
                        {
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomLeft,
                            Margin = new MarginPadding(10)
                            {
                                Bottom = 52, Horizontal = 20
                            },
                            AttributeType = AttributeType.Artist
                        }),
                        createHudElement(new AttributeText
                        {
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            Margin = new MarginPadding(10)
                            {
                                Horizontal = 20
                            },
                            AttributeType = AttributeType.Difficulty,
                            FontSize = 48
                        }),
                        createHudElement(new AttributeText
                        {
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight,
                            Margin = new MarginPadding(10)
                            {
                                Bottom = 52, Horizontal = 20
                            },
                            AttributeType = AttributeType.Mapper,
                            Text = "mapped by {value}"
                        }),
                        new ModsDisplay
                        {
                            Screen = this
                        },
                        new KeyOverlay
                        {
                            Screen = this
                        }
                    }
                },
                new AutoPlayDisplay
                {
                    Screen = this
                },
                new DangerHealthOverlay
                {
                    Screen = this
                },
                new LaneSwitchAlert
                {
                    Playfield = Playfield
                },
                new FlashOverlay(MapEvents.FlashEvents.Where(e => !e.InBackground).ToList())
                {
                    Clock = AudioClock
                },
                failOverlay = new FailOverlay
                {
                    Screen = this
                },
                failMenu = new FailMenu
                {
                    Screen = this
                },
                fcOverlay = new FullComboOverlay(),
                quickActionOverlay = new QuickActionOverlay(),
                new GameplayTouchInput
                {
                    Screen = this
                },
                new PauseMenu
                {
                    Screen = this
                },
                debugText = new FluXisTextFlow
                {
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    RelativeSizeAxes = Axes.Both,
                    Margin = new MarginPadding
                    {
                        Top = 100, Left = 20
                    },
                    FontSize = 20,
                    TextAnchor = Anchor.TopLeft,
                    Text = "debug text",
                    Alpha = showDebug ? 1 : 0
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        JudgementProcessor.ApplyMap(Map);
        ScoreProcessor.OnComboBreak += () => Samples.Miss();

        Playfield.Manager.OnFinished = () =>
        {
            if (HealthProcessor.Failed) return;

            if (HealthProcessor.OnComplete())
                End();
        };
    }

    private void updateRpc()
    {
        string details = $"{Map.Metadata.Title} - {Map.Metadata.Artist} [{Map.Metadata.Difficulty}]";
        string icon = RealmMap.MapSet.OnlineID <= 0 ? "playing" : $"{fluxel.Endpoint.APIUrl}/assets/cover/{RealmMap.MapSet.OnlineID}";

        if (!IsPaused.Value)
            activity.Update(Playfield.Manager.AutoPlay ? "Watching auto-play" : "Playing a map", details, icon, 0, (int)((Map.EndTime / Rate - AudioClock.CurrentTime) / 1000));
        else
            activity.Update("Paused", details, icon);
    }

    private T createHudElement<T>(T hudElement) where T : GameplayHUDElement
    {
        hudElement.Screen = this;
        return hudElement;
    }

    protected virtual MapInfo LoadMap()
    {
        var map = RealmMap.GetMapInfo();

        // map is null or invalid, leave
        if (map == null || !map.Validate(notifications)) return null;

        return map;
    }

    protected HealthProcessor CreateHealthProcessor()
    {
        var processor = null as HealthProcessor;

        if (Mods.Any(m => m is HardMod)) processor = new DrainHealthProcessor();
        else if (Mods.Any(m => m is EasyMod))
            processor = new RequirementHeathProcessor
            {
                HealthRequirement = EasyMod.HEALTH_REQUIREMENT
            };

        processor ??= new HealthProcessor();
        processor.Screen = this;
        processor.OnFail = OnDeath;
        return processor;
    }

    protected override void Update()
    {
        HealthProcessor.Update();

        if (AudioClock.CurrentTime >= 0 && starting)
        {
            starting = false;
            backgrounds.StartVideo();
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

        if (showDebug)
        {
            string text = "HitObjects";

            text += $"\n HitObjects: {Playfield.Manager.HitObjects.Count}";
            text += $"\n Future HitObjects: {Playfield.Manager.FutureHitObjects.Count}";
            text += $"\n Internal Child Count: {Playfield.Manager.InternalChildCount}";
            text += "\nTime";
            text += $"\n Current Time: {AudioClock.CurrentTime}";
            text += $"\n Current Rate: {Rate}";
            text += "\nProcessors";
            text += $"\n Health: {HealthProcessor.Health.Value}";

            debugText.Text = text;
        }

        base.Update();
    }

    public virtual void OnDeath()
    {
        // failOverlay.Show();
        failMenu.Show();
        AudioClock.RateTo(.4f, 2000, Easing.OutQuart).OnComplete(_ =>
        {
            // AudioClock.PlayTrack("Gameplay/DeathLoop.mp3");
            // AudioClock.RestartPoint = 0;
            // AudioClock.Looping = true;
            // AudioClock.RateTo(1, 0);
            AudioClock.LowPassFilter.CutoffTo(0);
            ScheduleAfterChildren(AudioClock.Start);
        });
    }

    protected virtual void End()
    {
        var player = Playfield.Manager.AutoPlay
            ? new APIUserShort
            {
                Username = "AutoPlay", ID = 0
            }
            : fluxel.LoggedInUser;

        if (ScoreProcessor.FullCombo || ScoreProcessor.FullFlawless)
            fcOverlay.Show(ScoreProcessor.FullFlawless ? FullComboOverlay.FullComboType.AllFlawless : FullComboOverlay.FullComboType.FullCombo);

        this.Delay(1000).FadeOut(500).OnComplete(_ => this.Push(new ResultsScreen(RealmMap, Map, ScoreProcessor.ToScoreInfo(), player, true, Mods.All(m => m.Rankable))));
    }

    public virtual void RestartMap()
    {
        restarting = true;
        Samples.Restart();
        this.Push(new GameplayScreen(RealmMap, Mods));
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        fadeOut();
        backgrounds.StopVideo();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        fadeOut();

        if (HealthProcessor.Failed)
        {
            // AudioClock.LoadMap(RealmMap, true);
            // AudioClock.SeekForce(HealthProcessor.FailTime);
            // AudioClock.RateTo(0, 0);
            AudioClock.LowPassFilter.CutoffTo(0);
        }

        AudioClock.LowPassFilter.CutoffTo(LowPassFilter.MAX, 500);
        ScheduleAfterChildren(() => AudioClock.RateTo(Rate, 500, Easing.InQuint));
        backgrounds.StopVideo();

        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        AudioClock.LoadMap(RealmMap, true);
        AudioClock.Seek(GameplayStartTime - 2000 * Rate);
        AudioClock.RateTo(Rate, 0);

        IsPaused.BindValueChanged(_ => updateRpc(), true);

        this.ScaleTo(1.2f)
            .FadeOut()
            .ScaleTo(1f, 750, Easing.OutQuint)
            .Delay(250)
            .FadeIn(250);

        base.OnEntering(e);
    }

    private void fadeOut()
    {
        if (HealthProcessor.Failed)
        {
            hud.FadeOut();
            Playfield.FadeOut();
            this.FadeOut(500);
        }
        else
            this.FadeOut(restarting ? 0 : 250);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        // probably coming back from results screen
        // so just go to song select
        this.Exit();

        base.OnResuming(e);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (Playfield.Manager.Finished) return false;

        if (e.ControlPressed && e.AltPressed && e.ShiftPressed && e.Key == Key.D)
        {
            showDebug = !showDebug;
            debugText.FadeTo(showDebug ? 1 : 0, 250, Easing.OutQuint);
            return true;
        }

        return false;
    }

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

            Map.KeyCount = Math.Max(Map.KeyCount, switchEvent.Count);
        }

        if (Map.InitialKeyCount == 0)
            Map.InitialKeyCount = Map.KeyCount;
    }
}
