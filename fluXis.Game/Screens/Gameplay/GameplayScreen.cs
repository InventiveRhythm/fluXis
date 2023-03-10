using System;
using System.IO;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Mouse;
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
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Gameplay;

public partial class GameplayScreen : Screen, IKeyBindingHandler<FluXisKeybind>
{
    [Resolved]
    private GlobalCursorOverlay cursorOverlay { get; set; }

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    private bool starting = true;
    private bool ended;
    private bool restarting;

    public BindableBool IsPaused { get; } = new();

    public GameplayInput Input;
    public Performance Performance;
    public MapInfo Map;
    public RealmMap RealmMap;
    public MapEvents MapEvents;
    public JudgementDisplay JudgementDisplay;
    public HitErrorBar HitErrorBar;
    public Playfield Playfield { get; private set; }
    private FailOverlay failOverlay;
    private FullComboOverlay fcOverlay;

    private FluXisConfig config;

    public Sample HitSound;
    public Sample Combobreak;
    public Sample Restart;

    public bool CanSubmitScore { get; set; } = true;

    public GameplayScreen(RealmMap realmMap)
    {
        RealmMap = realmMap;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples, FluXisConfig config, Storage storage)
    {
        this.config = config;

        Map = MapUtils.LoadFromPath(storage.GetFullPath("files/" + PathUtils.HashToPath(RealmMap.Hash)));

        // map is null or invalid, leave
        if (Map == null || !Map.Validate())
        {
            this.Exit();
            return;
        }

        loadMapEvents(storage);

        Input = new GameplayInput(this, Map.KeyCount);
        Performance = new Performance(Map);

        HitSound = samples.Get("Gameplay/hitsound.mp3");
        Combobreak = samples.Get("Gameplay/combobreak.ogg");
        Restart = samples.Get("Gameplay/restart.ogg");

        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            Input,
            Playfield = new Playfield(this)
        };

        AddInternal(new ComboCounter(this));
        AddInternal(new AccuracyDisplay(this));
        AddInternal(new Progressbar(this));
        AddInternal(JudgementDisplay = new JudgementDisplay(this));
        AddInternal(new AutoPlayDisplay(this));
        AddInternal(new JudgementCounter(Performance));
        AddInternal(new HealthBar(this));
        AddInternal(new DangerHealthOverlay(this));
        AddInternal(HitErrorBar = new HitErrorBar(this));

        AddRangeInternal(new Drawable[]
        {
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
        });

        AddInternal(new FlashOverlay(MapEvents.FlashEvents));

        AddInternal(failOverlay = new FailOverlay());
        AddInternal(fcOverlay = new FullComboOverlay());

        AddInternal(new PauseMenu(this));

        Discord.Update("Playing a map", $"{Map.Metadata.Title} - {Map.Metadata.Artist} [{Map.Metadata.Difficulty}]", "playing", 0, (int)((Map.EndTime - Conductor.CurrentTime) / 1000));
    }

    protected override void LoadComplete()
    {
        Conductor.PlayTrack(RealmMap);
        Conductor.CurrentTime = -2000;
        Conductor.TimingPoints = Map.TimingPoints;
        Conductor.SetSpeed(1, 0);
        backgrounds.SetVideoBackground(RealmMap, Map);

        base.LoadComplete();
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
                else
                    End();
            }
            else
            {
                End();
            }
        }

        base.Update();
    }

    public void Die()
    {
        Playfield.Manager.Dead = true;
        failOverlay.Show();
        Conductor.SetSpeed(0f, 2000, Easing.OutQuart).OnComplete(_ => this.Exit());
    }

    public void End()
    {
        if (ended)
            return;

        ended = true;

        cursorOverlay.ShowCursor = true;

        if (Performance.IsFullCombo() || Performance.IsAllFlawless())
        {
            fcOverlay.Show(Performance.IsAllFlawless() ? FullComboOverlay.FullComboType.AllFlawless : FullComboOverlay.FullComboType.FullCombo);
            this.Delay(1000).FadeOut(500).OnComplete(_ => this.Push(new ResultsScreen(RealmMap, Map, Performance, CanSubmitScore)));
        }
        else
            this.Push(new ResultsScreen(RealmMap, Map, Performance, CanSubmitScore));
    }

    public void RestartMap()
    {
        restarting = true;
        Restart.Play();
        this.Push(new GameplayScreen(RealmMap));
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

        backgrounds.Zoom = 1f;

        base.OnEntering(e);
    }

    private void fadeOut()
    {
        if (Playfield.Manager.Dead)
            this.FadeOut(500);
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
            case FluXisKeybind.ToggleAutoplay:
                Playfield.Manager.AutoPlay.Value = !Playfield.Manager.AutoPlay.Value;
                return true;

            case FluXisKeybind.Restart:
                RestartMap();
                return true;

            case FluXisKeybind.Pause:
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

    private void loadMapEvents(Storage storage)
    {
        var mapEvents = new MapEvents();

        try
        {
            if (!string.IsNullOrEmpty(Map.EffectFile))
            {
                var effectFile = RealmMap.MapSet.GetFile(Map.EffectFile);
                if (effectFile == null) return;

                string content = File.ReadAllText(storage.GetFullPath("files/" + effectFile.GetPath()));
                mapEvents.Load(content);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error loading map events");
        }

        MapEvents = mapEvents;

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
