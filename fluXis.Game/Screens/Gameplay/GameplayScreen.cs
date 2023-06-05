using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Import;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Overlay.Mouse;
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

namespace fluXis.Game.Screens.Gameplay;

public partial class GameplayScreen : FluXisScreen, IKeyBindingHandler<FluXisKeybind>
{
    public override float ParallaxStrength => 0f;
    public override bool ShowToolbar => false;
    public override bool AllowMusicControl => false;

    [Resolved]
    private GlobalCursorOverlay cursorOverlay { get; set; }

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    [Resolved]
    private NotificationOverlay notifications { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    public AudioClock AudioClock { get; set; }

    private bool starting = true;
    private bool ended;
    private bool restarting;

    public BindableBool IsPaused { get; } = new();
    public float DeathTime { get; private set; }

    public GameplayInput Input;
    public Performance Performance;
    public MapInfo Map;
    public RealmMap RealmMap;
    public MapEvents MapEvents;
    public List<IMod> Mods;
    public MapPackage MapPackage;

    public Playfield Playfield { get; private set; }
    public Container HUD { get; private set; }

    private FailOverlay failOverlay;
    private FullComboOverlay fcOverlay;
    private QuickActionOverlay quickActionOverlay;

    private FluXisConfig config;

    public Sample HitSound;
    public Sample Combobreak;
    public Sample Restart;

    public float Rate { get; }

    public GameplayScreen(RealmMap realmMap, List<IMod> mods)
    {
        RealmMap = realmMap;
        Mods = mods;

        Rate = ((RateMod)Mods.Find(m => m is RateMod))?.Rate ?? 1f;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples, FluXisConfig config, ImportManager importManager)
    {
        this.config = config;

        if (RealmMap.MapSet.Managed)
        {
            var package = importManager.GetMapPackage(RealmMap);

            if (package != null)
            {
                Map = package.MapInfo;
                MapEvents = package.MapEvents;

                if (!Map.Validate())
                {
                    notifications.PostError("The map you tried to play not valid.");
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

        Input = new GameplayInput(this, Map.KeyCount);
        Performance = new Performance(Map, RealmMap.OnlineID, RealmMap.Hash, Mods);

        HitSound = samples.Get("Gameplay/hitsound.mp3");
        Combobreak = samples.Get("Gameplay/combobreak.mp3");
        Restart = samples.Get("Gameplay/restart.mp3");

        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            Input,
            new FlashOverlay(MapEvents.FlashEvents.Where(e => e.InBackground).ToList()),
            Playfield = new Playfield(this),
            HUD = new Container
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
                    new AutoPlayDisplay(this),
                    new JudgementCounter(Performance),
                    new HealthBar(this),
                    new DangerHealthOverlay(this),
                    new HitErrorBar(this),
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
            new LaneSwitchAlert { Playfield = Playfield },
            new FlashOverlay(MapEvents.FlashEvents.Where(e => !e.InBackground).ToList()),
            failOverlay = new FailOverlay { Screen = this },
            fcOverlay = new FullComboOverlay(),
            quickActionOverlay = new QuickActionOverlay(),
            new GameplayTouchInput { Screen = this },
            new PauseMenu(this)
        };

        Discord.Update("Playing a map", $"{Map.Metadata.Title} - {Map.Metadata.Artist} [{Map.Metadata.Difficulty}]", "playing", 0, (int)((Map.EndTime - AudioClock.CurrentTime) / 1000));
    }

    protected override void LoadComplete()
    {
        AudioClock.LoadMap(RealmMap, true);
        AudioClock.Seek(-2000);
        backgrounds.SetVideoBackground(RealmMap, Map);

        AudioClock.RateTo(Rate, 0);

        backgrounds.SetDim(config.Get<float>(FluXisSetting.BackgroundDim), 600);
        backgrounds.SetBlur(config.Get<float>(FluXisSetting.BackgroundBlur), 600);

        base.LoadComplete();
    }

    public virtual MapInfo LoadMap()
    {
        var map = MapUtils.LoadFromPath(storage.GetFullPath("files/" + PathUtils.HashToPath(RealmMap.Hash)));

        // map is null or invalid, leave
        if (map == null || !map.Validate())
        {
            notifications.PostError("The map you tried to play not valid.");
            this.Exit();
            return null;
        }

        return map;
    }

    protected override void Update()
    {
        if (AudioClock.CurrentTime >= 0 && starting)
        {
            starting = false;
            cursorOverlay.ShowCursor = false;
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
            AudioClock.RateTo(1f, 0);
            AudioClock.LowPassFilter.CutoffTo(0, 0, Easing.None);
        });
    }

    public virtual void End()
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
        cursorOverlay.ShowCursor = true;

        int time = Playfield.Manager.Dead ? 1400 : 500;

        if (Playfield.Manager.Dead)
        {
            AudioClock.LoadMap(RealmMap, true);
            AudioClock.Seek(DeathTime);
            AudioClock.Rate = 0;
            AudioClock.LowPassFilter.CutoffTo(0, 0, Easing.None);
        }

        AudioClock.LowPassFilter.CutoffTo(LowPassFilter.MAX, time, Easing.None);
        AudioClock.RateTo(Rate, time);
        backgrounds.StopVideo();

        return base.OnExiting(e);
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        Alpha = 0;
        this.ScaleTo(1.2f)
            .ScaleTo(1f, 750, Easing.OutQuint)
            .Delay(250)
            .FadeIn(250);

        base.OnEntering(e);
    }

    private void fadeOut()
    {
        if (Playfield.Manager.Dead)
        {
            HUD.FadeOut();
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

                if (!Mods.Any(m => m is PausedMod))
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
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e)
    {
        if (e.Action is FluXisKeybind.Restart or FluXisKeybind.Exit)
            quickActionOverlay.IsHolding = false;
    }

    public virtual MapEvents LoadMapEvents()
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
