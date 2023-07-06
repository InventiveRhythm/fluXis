using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Activity;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Import;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Gameplay.HUD;
using fluXis.Game.Screens.Gameplay.HUD.Judgement;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Game.Screens.Gameplay.Overlay;
using fluXis.Game.Screens.Gameplay.Overlay.Effect;
using fluXis.Game.Screens.Gameplay.UI;
using fluXis.Game.Screens.Result;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK.Input;

namespace fluXis.Game.Screens.Gameplay;

public partial class GameplayScreen : FluXisScreen, IKeyBindingHandler<FluXisKeybind>
{
    public override float ParallaxStrength => 0f;
    public override float BackgroundBlur => backgroundBlur?.Value ?? 0f;
    public override float BackgroundDim => backgroundDim?.Value ?? .4f;
    public override bool ShowToolbar => false;
    public override bool AllowMusicControl => false;
    public override bool ApplyValuesAfterLoad => true;
    public override bool AllowExit => false;

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    [Resolved]
    private NotificationOverlay notifications { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    public AudioClock AudioClock { get; set; }

    [Resolved]
    private ActivityManager activity { get; set; }

    private bool starting = true;
    private bool ended;
    private bool restarting;

    public BindableBool IsPaused { get; } = new();
    public float DeathTime { get; private set; }
    public Action<double, double> OnSeek { get; set; }

    public GameplayInput Input;
    public Performance Performance;
    public MapInfo Map;
    public RealmMap RealmMap;
    public MapEvents MapEvents;
    public List<IMod> Mods;

    public Playfield Playfield { get; private set; }
    private Container hud { get; set; }

    private FluXisTextFlow debugText;
    private static bool showDebug;

    private FailOverlay failOverlay;
    private FullComboOverlay fcOverlay;
    private QuickActionOverlay quickActionOverlay;

    private FluXisConfig config;
    private Bindable<HudVisibility> hudVisibility;
    private Bindable<float> backgroundDim;
    private Bindable<float> backgroundBlur;

    private bool hudVisible = true;

    public Sample HitSound;
    public Sample Combobreak;
    public Sample Restart;

    public float Rate { get; }

    public GameplayScreen(RealmMap realmMap, List<IMod> mods)
    {
        RealmMap = realmMap;
        Mods = mods;

        Rate = ((RateMod)Mods.Find(m => m is RateMod))?.Rate ?? 1f;
        Mods.RemoveAll(m => m is PausedMod);
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples, FluXisConfig config, ImportManager importManager)
    {
        this.config = config;
        hudVisibility = config.GetBindable<HudVisibility>(FluXisSetting.HudVisibility);

        if (RealmMap.MapSet.Managed)
        {
            var package = importManager.GetMapPackage(RealmMap);

            if (package != null)
            {
                Map = package.MapInfo;
                MapEvents = package.MapEvents;

                if (!Map.Validate(notifications))
                {
                    this.Exit();
                    return;
                }
            }
        }
        else
        {
            Map = LoadMap();
            if (Map != null) MapEvents = LoadMapEvents();
        }

        if (Map == null)
        {
            Logger.Log("Failed to load map", LoggingTarget.Runtime, LogLevel.Error);
            this.Exit();
            return;
        }

        Map.Sort();
        getKeyCountFromEvents();
        backgrounds.SetVideoBackground(RealmMap, Map);

        Input = new GameplayInput(this, Map.KeyCount);
        Performance = new Performance(Map, RealmMap.OnlineID, RealmMap.Hash, Mods);

        HitSound = samples.Get("Gameplay/hitsound.mp3");
        Combobreak = samples.Get("Gameplay/combobreak.mp3");
        Restart = samples.Get("Gameplay/restart.mp3");

        backgroundBlur = config.GetBindable<float>(FluXisSetting.BackgroundBlur);
        backgroundDim = config.GetBindable<float>(FluXisSetting.BackgroundDim);

        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            Input,
            new FlashOverlay(MapEvents.FlashEvents.Where(e => e.InBackground).ToList()),
            Playfield = new Playfield(this),
            hud = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new ComboCounter(this),
                    new AccuracyDisplay(this),
                    new Progressbar(this),
                    new JudgementDisplay(this),
                    new JudgementCounter(Performance),
                    new HealthBar(this),
                    new HitErrorBar(this),
                    new ModsDisplay(this),
                    new AttributeText(this)
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Margin = new MarginPadding(10),
                        AttributeType = AttributeType.Title,
                        FontSize = 48
                    },
                    new AttributeText(this)
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Margin = new MarginPadding(10) { Bottom = 52 },
                        AttributeType = AttributeType.Artist
                    },
                    new AttributeText(this)
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Margin = new MarginPadding(10),
                        AttributeType = AttributeType.Difficulty,
                        FontSize = 48
                    },
                    new AttributeText(this)
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Margin = new MarginPadding(10) { Bottom = 52 },
                        AttributeType = AttributeType.Mapper,
                        Text = "mapped by {value}"
                    }
                }
            },
            new AutoPlayDisplay(this),
            new DangerHealthOverlay(this),
            new LaneSwitchAlert { Playfield = Playfield },
            new FlashOverlay(MapEvents.FlashEvents.Where(e => !e.InBackground).ToList()),
            failOverlay = new FailOverlay { Screen = this },
            fcOverlay = new FullComboOverlay(),
            quickActionOverlay = new QuickActionOverlay(),
            new GameplayTouchInput { Screen = this },
            new PauseMenu { Screen = this },
            debugText = new FluXisTextFlow
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                RelativeSizeAxes = Axes.Both,
                Margin = new MarginPadding { Top = 100, Left = 20 },
                FontSize = 20,
                TextAnchor = Anchor.TopLeft,
                Text = "debug text",
                Alpha = showDebug ? 1 : 0
            }
        };
    }

    private void updateRpc()
    {
        string details = $"{Map.Metadata.Title} - {Map.Metadata.Artist} [{Map.Metadata.Difficulty}]";

        if (!IsPaused.Value)
            activity.Update("Playing a map", details, "playing", 0, (int)((Map.EndTime / Rate - AudioClock.CurrentTime) / 1000));
        else
            activity.Update("Paused", details, "playing");
    }

    protected virtual MapInfo LoadMap()
    {
        var map = MapUtils.LoadFromPath(storage.GetFullPath("files/" + PathUtils.HashToPath(RealmMap.Hash)));

        // map is null or invalid, leave
        if (map == null || !map.Validate(notifications)) return null;

        return map;
    }

    protected override void Update()
    {
        if (AudioClock.CurrentTime >= 0 && starting)
        {
            starting = false;
            backgrounds.StartVideo();
        }

        if (!starting && Playfield.Manager.IsFinished && !Playfield.Manager.Dead)
        {
            if (Playfield.Manager.HealthMode == HealthMode.Requirement)
            {
                if (Playfield.Manager.Health < 70)
                    Die();
                else if (!ended)
                {
                    ended = true;
                    End();
                }
            }
            else if (!ended)
            {
                ended = true;
                End();
            }
        }

        // normal bindings dont work for some reason
        HitSound.Volume.Value = config.Get<double>(FluXisSetting.HitSoundVolume);

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

            debugText.Text = text;
        }

        base.Update();
    }

    public virtual void Die()
    {
        Playfield.Manager.Dead = true;
        DeathTime = (float)AudioClock.CurrentTime;
        failOverlay.Show();
        AudioClock.RateTo(0f, 2000, Easing.OutQuart).OnComplete(_ =>
        {
            AudioClock.PlayTrack("Gameplay/DeathLoop.mp3");
            AudioClock.RestartPoint = 0;
            AudioClock.Looping = true;
            AudioClock.LowPassFilter.CutoffTo(0);
        });
    }

    protected virtual void End()
    {
        if (Performance.FullCombo || Performance.AllFlawless)
        {
            fcOverlay.Show(Performance.AllFlawless ? FullComboOverlay.FullComboType.AllFlawless : FullComboOverlay.FullComboType.FullCombo);
            this.Delay(1000).FadeOut(500).OnComplete(_ => this.Push(new ResultsScreen(RealmMap, Map, Performance)));
        }
        else this.Push(new ResultsScreen(RealmMap, Map, Performance));
    }

    public virtual void RestartMap()
    {
        restarting = true;
        Restart?.Play();
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

        int time = Playfield.Manager.Dead ? 1400 : 500;

        if (Playfield.Manager.Dead)
        {
            AudioClock.LoadMap(RealmMap, true);
            AudioClock.SeekForce(DeathTime);
            AudioClock.LowPassFilter.CutoffTo(0);
        }

        AudioClock.LowPassFilter.CutoffTo(LowPassFilter.MAX, time);
        ScheduleAfterChildren(() => AudioClock.RateTo(Rate, time));
        backgrounds.StopVideo();

        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        AudioClock.LoadMap(RealmMap, true);
        AudioClock.Seek(-2000 * Rate);
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
        if (Playfield.Manager.Dead)
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
        if (e.ControlPressed && e.AltPressed && e.ShiftPressed && e.Key == Key.D)
        {
            showDebug = !showDebug;
            debugText.FadeTo(showDebug ? 1 : 0, 250, Easing.OutQuint);
            return true;
        }

        return false;
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        if (e.Repeat) return false;

        switch (e.Action)
        {
            case FluXisKeybind.Restart:
                quickActionOverlay.OnConfirm = RestartMap;
                quickActionOverlay.IsHolding = true;
                return true;

            case FluXisKeybind.Exit:
                quickActionOverlay.OnConfirm = this.Exit;
                quickActionOverlay.IsHolding = true;
                return true;

            case FluXisKeybind.GameplayPause:
                if (Playfield.Manager.Dead) return false;

                // only add when we have hit at least one note
                if (!Mods.Any(m => m is PausedMod) && AudioClock.CurrentTime > Map.StartTime)
                    Mods.Add(new PausedMod());

                IsPaused.Value = !IsPaused.Value;
                return true;

            case FluXisKeybind.Skip:
                if (Map.StartTime - AudioClock.CurrentTime > 2000)
                    AudioClock.Seek(Map.StartTime - 2000);
                return true;

            case FluXisKeybind.ScrollSpeedIncrease:
                config.GetBindable<float>(FluXisSetting.ScrollSpeed).Value += 0.1f;
                return true;

            case FluXisKeybind.ScrollSpeedDecrease:
                config.GetBindable<float>(FluXisSetting.ScrollSpeed).Value -= 0.1f;
                return true;

            case FluXisKeybind.SeekBackward:
                OnSeek?.Invoke(AudioClock.CurrentTime, AudioClock.CurrentTime - 5000);
                return true;

            case FluXisKeybind.SeekForward:
                OnSeek?.Invoke(AudioClock.CurrentTime, AudioClock.CurrentTime + 5000);
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e)
    {
        if (e.Action is FluXisKeybind.Restart or FluXisKeybind.Exit)
            quickActionOverlay.IsHolding = false;
    }

    protected virtual MapEvents LoadMapEvents()
    {
        var mapEvents = new MapEvents();

        try
        {
            if (!string.IsNullOrEmpty(Map.EffectFile))
            {
                var effectFile = RealmMap.MapSet.GetFile(Map.EffectFile);
                if (effectFile == null) return mapEvents;

                string content = File.ReadAllText(storage.GetFullPath("files/" + effectFile.GetPath()));
                mapEvents.Load(content);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error loading map events");
        }

        return mapEvents;
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
