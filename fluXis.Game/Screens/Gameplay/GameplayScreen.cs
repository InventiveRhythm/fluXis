using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
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

    [Resolved]
    private GlobalCursorOverlay cursorOverlay { get; set; }

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    [Resolved]
    private NotificationOverlay notifications { get; set; }

    [Resolved]
    private Storage storage { get; set; }

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

    public Playfield Playfield { get; private set; }
    public Container HUD { get; private set; }

    private FailOverlay failOverlay;
    private FullComboOverlay fcOverlay;

    private FluXisConfig config;

    public Sample HitSound;
    public Sample Combobreak;
    public Sample Restart;

    public GameplayScreen(RealmMap realmMap, List<IMod> mods)
    {
        RealmMap = realmMap;
        Mods = mods;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples, FluXisConfig config)
    {
        this.config = config;

        Map = LoadMap();
        Map.Sort();
        MapEvents = LoadMapEvents();
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
            new FlashOverlay(MapEvents.FlashEvents.Where(e => !e.InBackground).ToList())
        };

        AddInternal(failOverlay = new FailOverlay { Screen = this });
        AddInternal(fcOverlay = new FullComboOverlay());

        AddInternal(new PauseMenu(this));

        Discord.Update("Playing a map", $"{Map.Metadata.Title} - {Map.Metadata.Artist} [{Map.Metadata.Difficulty}]", "playing", 0, (int)((Map.EndTime - Conductor.CurrentTime) / 1000));
    }

    protected override void LoadComplete()
    {
        Conductor.PlayTrack(RealmMap);
        Conductor.Seek(-2000 + Conductor.Offset);
        Conductor.TimingPoints = Map.TimingPoints;
        Conductor.SetSpeed(1, 0);
        backgrounds.SetVideoBackground(RealmMap, Map);

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
        if (Conductor.CurrentTime >= 0 && starting)
        {
            starting = false;
            Conductor.ResumeTrack();
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
        DeathTime = Conductor.CurrentTime;
        failOverlay.Show();
        Conductor.SetSpeed(0f, 2000, Easing.OutQuart).OnComplete(_ =>
        {
            Conductor.PlayTrack("Gameplay/DeathLoop.mp3");
            Conductor.SetLoop(0);
            Conductor.LowPassFilter.CutoffTo(0, 0, Easing.None);
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
            Conductor.PlayTrack(RealmMap, true, DeathTime);
            Conductor.SetSpeed(0, 0, Easing.None);
            Conductor.LowPassFilter.CutoffTo(0, 0, Easing.None);
        }

        Conductor.LowPassFilter.CutoffTo(LowPassFilter.MAX, time, Easing.None);
        Conductor.SetSpeed(1, time, Easing.None);
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
        switch (e.Action)
        {
            case FluXisKeybind.Restart:
                RestartMap();
                return true;

            case FluXisKeybind.Exit:
                this.Exit();
                return true;

            case FluXisKeybind.Pause:
                if (Playfield.Manager.Dead) return false;

                if (!Mods.Any(m => m is PausedMod))
                    Mods.Add(new PausedMod());

                IsPaused.Value = !IsPaused.Value;
                return true;

            case FluXisKeybind.Skip:
                if (Map.StartTime - Conductor.CurrentTime > 2000)
                    Conductor.Seek(Map.StartTime - 2000);
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
        // nothing to do here...
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
