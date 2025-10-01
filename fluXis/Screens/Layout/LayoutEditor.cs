using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Input;
using fluXis.Map;
using fluXis.Map.Drawables;
using fluXis.Map.Structures;
using fluXis.Mods;
using fluXis.Overlay.Notifications;
using fluXis.Replays;
using fluXis.Scoring;
using fluXis.Scoring.Processing;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Edit.Input;
using fluXis.Screens.Edit.UI.MenuBar;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Gameplay.Replays;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Screens.Layout.Blueprints;
using fluXis.Screens.Layout.Components;
using fluXis.Screens.Layout.Settings;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using MongoDB.Bson;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Layout;

public partial class LayoutEditor : FluXisScreen, IHUDDependencyProvider, IKeyBindingHandler<FluXisGlobalKeybind>, IKeyBindingHandler<EditorKeybinding>
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

    [Resolved]
    private LayoutManager manager { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    public event Action RulesetLoaded;
    public bool RulesetIsLoaded => ruleset is not null;

    private HUDLayout layout { get; }

    private BindableBool forceAspect { get; } = new();

    private string lastHash = string.Empty;
    private string currentHash => MapUtils.GetHash(layout.Serialize(true));

    private bool unsavedChanges => lastHash != currentHash;
    private bool confirmedExit;

    private EditorMenuBar menuBar;
    private LayoutBlueprintContainer blueprints;
    private AspectRatioContainer content;
    private Container rulesetWrapper;
    private ReplayRulesetContainer ruleset;
    private GameplayHUD hud;

    private DependencyContainer dependencies;

    public LayoutEditor(HUDLayout layout)
    {
        this.layout = layout.JsonCopy();
        this.layout.ID = layout.ID;
    }

    [BackgroundDependencyLoader]
    private void load(GameHost host, FluXisConfig config)
    {
        dependencies.CacheAs(this);

        InternalChild = new EditorKeybindingContainer(this, config.GetBindable<string>(FluXisSetting.EditorKeymap), host)
        {
            RelativeSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                dependencies.CacheAsAndReturn(new Hitsounding(maps.CurrentMapSet, new List<HitSoundFade>(), clock.RateBindable.GetBoundCopy())),
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.Background2
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
                                            },
                                            blueprints = new LayoutBlueprintContainer()
                                        }
                                    }
                                },
                                new ComponentsSettings(blueprints)
                                {
                                    Width = sidebar_width,
                                    RelativeSizeAxes = Axes.Y
                                }
                            }
                        }
                    },
                },
                menuBar = new EditorMenuBar
                {
                    Items = new MenuItem[]
                    {
                        new MenuExpandItem("File", FontAwesome6.Solid.File, new MenuItem[]
                        {
                            new MenuActionItem("Save", FontAwesome6.Solid.FloppyDisk, MenuItemType.Normal, () => save()),
                            new MenuActionItem("Exit", FontAwesome6.Solid.DoorOpen, MenuItemType.Dangerous, this.Exit)
                        }),
                        new MenuExpandItem("View", FontAwesome6.Solid.Eye, new MenuItem[]
                        {
                            new MenuToggleItem("Force 16:9", FontAwesome6.Solid.RectangleWide, forceAspect.Toggle, () => forceAspect.Value)
                        })
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        updateHash();

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

                    LoadComponentAsync(hud = new GameplayHUD(c, layout) { AutoRefresh = false }, hud => ScheduleAfterChildren(() =>
                    {
                        rulesetWrapper.Add(hud);
                        hud.Components.ForEach(ComponentAdded);
                    }));
                }));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to create ruleset!");
            }
        });
    }

    private bool save()
    {
        if (!unsavedChanges)
        {
            notifications.SendSmallText("Layout is already up to date!", FontAwesome6.Solid.Check);
            return true;
        }

        updateDictionary();
        manager.SaveLayout(layout);
        notifications.SendSmallText("Saved!", FontAwesome6.Solid.Check);
        updateHash();

        return true;
    }

    private void updateHash() => lastHash = currentHash;

    private void updateDictionary()
    {
        var components = hud.Components;
        layout.Gameplay = components.ToDictionary(
            x => x.Settings.GetDictionaryKey(manager),
            x =>
            {
                var settings = x.Settings;
                settings.GetSettingsFrom(x.Settings.Drawable);
                return settings;
            }
        );
    }

    public void AddComponent(string type, HUDComponentSettings settings)
    {
        settings.Key = ObjectId.GenerateNewId().ToString();

        var comp = hud.AddComponent($"{type}#{settings.Key}", settings);
        ComponentAdded?.Invoke(comp);

        updateDictionary();
    }

    public void UpdateAnchorToPlayfield(GameplayHUDComponent comp)
    {
        var key = comp.Settings.GetDictionaryKey(manager);

        hud.RemoveComponent(comp);
        ComponentRemoved?.Invoke(comp);

        comp = hud.AddComponent(key, comp.Settings);
        ComponentAdded?.Invoke(comp);

        blueprints.Select(comp);

        updateDictionary();
    }

    public void RemoveComponent(GameplayHUDComponent component)
    {
        var key = component.Settings.GetDictionaryKey(manager);

        layout.Gameplay.Remove(key);
        hud.RemoveComponent(component);
        ComponentRemoved?.Invoke(component);

        updateDictionary();
    }

    #region Events

    public event Action<GameplayHUDComponent> ComponentAdded;
    public event Action<GameplayHUDComponent> ComponentRemoved;

    #endregion

    #region IHUDDependencyProvider Implementation

    RulesetContainer IHUDDependencyProvider.Ruleset => ruleset;
    JudgementProcessor IHUDDependencyProvider.JudgementProcessor => ruleset.PlayfieldManager.Players[0].JudgementProcessor;
    HealthProcessor IHUDDependencyProvider.HealthProcessor => ruleset.PlayfieldManager.Players[0].HealthProcessor;
    ScoreProcessor IHUDDependencyProvider.ScoreProcessor => ruleset.PlayfieldManager.Players[0].ScoreProcessor;
    HitWindows IHUDDependencyProvider.HitWindows => ruleset.HitWindows;
    RealmMap IHUDDependencyProvider.RealmMap => ruleset.MapInfo.RealmEntry;
    MapInfo IHUDDependencyProvider.MapInfo => ruleset.MapInfo;
    float IHUDDependencyProvider.PlaybackRate => ruleset.Rate;
    double IHUDDependencyProvider.CurrentTime => ruleset.CurrentTime;

    #endregion

    #region Overrides

    public override void OnEntering(ScreenTransitionEvent e)
    {
        clock.RestartPoint = 0;
        clock.AllowLimitedLoop = false;

        this.FadeOut();

        using (BeginDelayedSequence(Styling.TRANSITION_ENTER_DELAY))
        {
            this.FadeIn(Styling.TRANSITION_FADE);
            menuBar.MoveToY(-45).MoveToY(0, Styling.TRANSITION_MOVE, Easing.OutQuint);
            content.FadeOut().ScaleTo(.98f).Delay(Styling.TRANSITION_ENTER_DELAY)
                   .FadeIn(Styling.TRANSITION_FADE).ScaleTo(1f, Styling.TRANSITION_MOVE, Easing.OutQuint);
        }
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (unsavedChanges && !confirmedExit)
        {
            panels.Content = new UnsavedChangesPanel(() =>
            {
                if (!save())
                    return;

                confirmedExit = true;
                this.Exit();
            }, () =>
            {
                confirmedExit = true;
                this.Exit();
            });

            return true;
        }

        clock.Looping = false;

        menuBar.MoveToY(-45, Styling.TRANSITION_MOVE, Easing.OutQuint);
        this.FadeOut(Styling.TRANSITION_FADE);
        content.FadeOut(Styling.TRANSITION_FADE).ScaleTo(.98f, Styling.TRANSITION_MOVE, Easing.OutQuint);
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

    public bool OnPressed(KeyBindingPressEvent<EditorKeybinding> e)
    {
        if (e.Repeat)
            return false;

        switch (e.Action)
        {
            case EditorKeybinding.Save:
                save();
                return true;

            default:
                return false;
        }
    }

    public void OnReleased(KeyBindingReleaseEvent<EditorKeybinding> e)
    {
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    #endregion
}
