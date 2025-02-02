﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Audio;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Input;
using fluXis.Map;
using fluXis.Map.Drawables;
using fluXis.Map.Structures;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Scoring;
using fluXis.Scoring.Processing;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Edit.MenuBar;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Gameplay.Replays;
using fluXis.Screens.Layout.Components;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Layout;

public partial class LayoutEditor : FluXisScreen, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public override bool AllowMusicControl => false;
    public override bool AllowMusicPausing => true;
    public override bool AutoPlayNext => false;
    public override bool ShowToolbar => false;
    public override float BackgroundBlur => .4f;
    public override float BackgroundDim => .6f;
    public override float ParallaxStrength => 0f;

    private const float sidebar_width = 360;

    [Resolved]
    private GlobalClock clock { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    public event Action RulesetLoaded;
    public bool RulesetIsLoaded => ruleset is not null;

    public JudgementProcessor JudgementProcessor => ruleset.PlayfieldManager.Players[0].JudgementProcessor;
    public HealthProcessor HealthProcessor => ruleset.PlayfieldManager.Players[0].HealthProcessor;
    public ScoreProcessor ScoreProcessor => ruleset.PlayfieldManager.Players[0].ScoreProcessor;
    public HitWindows HitWindows => ruleset.HitWindows;

    private HUDLayout layout { get; }

    private BindableBool forceAspect { get; } = new();

    private EditorMenuBar menuBar;
    private AspectRatioContainer content;
    private Container rulesetWrapper;
    private ReplayRulesetContainer ruleset;

    private DependencyContainer dependencies;

    public LayoutEditor(HUDLayout layout)
    {
        this.layout = layout.JsonCopy();
        this.layout.ID = layout.ID;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        dependencies.CacheAs(this);

        InternalChildren = new Drawable[]
        {
            dependencies.CacheAsAndReturn(new Hitsounding(maps.CurrentMapSet, new List<HitSoundFade>(), clock.RateBindable.GetBoundCopy())),
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Top = 45 },
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new Dimension[]
                    {
                        new(GridSizeMode.AutoSize),
                        new(),
                        new(GridSizeMode.AutoSize)
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new ComponentList
                            {
                                Width = sidebar_width,
                                RelativeSizeAxes = Axes.Y
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding { Vertical = 16 },
                                Child = content = new AspectRatioContainer(forceAspect)
                                {
                                    CornerRadius = 12,
                                    Masking = true,
                                    Children = new Drawable[]
                                    {
                                        new MapBackground(maps.CurrentMap)
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre
                                        },
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = Colour4.Black,
                                            Alpha = 0.4f
                                        },
                                        rulesetWrapper = new DrawSizePreservingFillContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            TargetDrawSize = new Vector2(1920, 1080)
                                        }
                                    }
                                }
                            },
                            new Container
                            {
                                Width = sidebar_width,
                                RelativeSizeAxes = Axes.Y
                            },
                        }
                    }
                },
            },
            menuBar = new EditorMenuBar
            {
                Items = new MenuItem[]
                {
                    new FluXisMenuItem("File", FontAwesome6.Solid.File)
                    {
                        Items = new MenuItem[]
                        {
                            new FluXisMenuItem("Save", FontAwesome6.Solid.FloppyDisk, MenuItemType.Normal, save),
                            new FluXisMenuItem("Exit", FontAwesome6.Solid.DoorOpen, MenuItemType.Dangerous, this.Exit)
                        }
                    },
                    new FluXisMenuItem("View", FontAwesome6.Solid.Eye)
                    {
                        Items = new MenuItem[]
                        {
                            new FluXisMenuItem("Force 16:9", FontAwesome6.Solid.RectangleWide, MenuItemType.Normal, forceAspect.Toggle)
                            {
                                IsActive = () => forceAspect.Value
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Task.Run(() =>
        {
            try
            {
                var rm = maps.CurrentMap;
                var map = rm.GetMapInfo();

                string reason = "";

                if (map is null || !map.Validate(out reason))
                    throw new Exception($"Failed to load map. ({reason})");

                var auto = new AutoGenerator(map, rm.KeyCount);
                var replay = auto.Generate();
                var events = map.GetMapEvents();

                Schedule(() => LoadComponentAsync(ruleset = new ReplayRulesetContainer(replay, map, events, new List<IMod> { new AutoPlayMod() })
                {
                    ParentClock = clock
                }, c =>
                {
                    rulesetWrapper.Add(c);
                    RulesetLoaded?.Invoke();
                }));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to create ruleset!");
            }
        });
    }

    private void save()
    {
    }

    #region Overrides

    public override void OnEntering(ScreenTransitionEvent e)
    {
        clock.RestartPoint = 0;
        clock.AllowLimitedLoop = false;

        this.FadeOut();

        using (BeginDelayedSequence(ENTER_DELAY))
        {
            this.FadeIn(FADE_DURATION);
            menuBar.MoveToY(-45).MoveToY(0, MOVE_DURATION, Easing.OutQuint);
            content.FadeOut().ScaleTo(.98f).Delay(ENTER_DELAY)
                   .FadeIn(FADE_DURATION).ScaleTo(1f, MOVE_DURATION, Easing.OutQuint);
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        clock.Looping = false;

        menuBar.MoveToY(-45, MOVE_DURATION, Easing.OutQuint);
        this.FadeOut(FADE_DURATION);
        content.FadeOut(FADE_DURATION).ScaleTo(.98f, MOVE_DURATION, Easing.OutQuint);
        return base.OnExiting(e);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                this.Exit();
                return true;

            default:
                return false;
        }
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e)
    {
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    #endregion
}
