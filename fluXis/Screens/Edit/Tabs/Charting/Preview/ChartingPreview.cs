using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Configuration;
using fluXis.Graphics;
using fluXis.Graphics.Background;
using fluXis.Graphics.Shaders;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Map.Structures.Events.Camera;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Overlay.Effect;
using fluXis.Screens.Gameplay.Replays;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Storyboards;
using fluXis.Utils;
using Midori.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Preview;

#nullable enable

public partial class ChartingPreview : DrawSizePreservingFillContainer
{
    [Resolved]
    protected Editor Editor { get; private set; } = null!;

    [Resolved]
    protected EditorClock EditorClock { get; private set; } = null!;

    [Resolved]
    protected EditorMap Map { get; private set; } = null!;

    private IdleTracker idleTracker = null!;
    private Bindable<float> userScrollSpeed = null!;

    private SpriteStack<BlurableBackground> background = null!;
    private BackgroundVideo backgroundVideo = null!;
    private Box backgroundDim = null!;

    private Container rulesetWrapper = null!;
    private RulesetContainer? ruleset;

    private PreviewFlashLayer backFlash = null!;
    private PreviewFlashLayer frontFlash = null!;
    private PulseEffect pulseEffect = null!;

    private LoadingIcon loading = null!;

    private static Type[] ignoredForRebuild { get; } =
    [
        typeof(PulseEvent),
        typeof(ShaderEvent),
        typeof(NoteEvent),
        typeof(StoryboardElement),
        typeof(CameraMoveEvent),
        typeof(CameraScaleEvent),
        typeof(CameraRotateEvent)
    ];

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        userScrollSpeed = config.GetBindable<float>(FluXisSetting.ScrollSpeed);

        RelativeSizeAxes = Axes.Both;
        TargetDrawSize = new Vector2(1920, 1080);
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        camera = new CameraContainer([.. Map.MapEvents.Where(x => x is ICameraEvent).Cast<ICameraEvent>()]);

        Children =
        [
            new Box { RelativeSizeAxes = Axes.Both, Colour = Theme.Background2 },
            idleTracker = new IdleTracker(400, rebuildRuleset, () =>
            {
                loading.Show();
                ruleset?.FadeOut(Styling.TRANSITION_FADE);
            }),
            handler = new PreviewShaderHandler(),
            createShaderStack().WithChildren([
                camera.CreateProxyDrawable().With(x => x.Clock = EditorClock),
                camera.WithChildren(new Drawable[]
                {
                    background = new SpriteStack<BlurableBackground> { AutoFill = false },
                    backgroundVideo = new BackgroundVideo
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        VideoClock = EditorClock
                    },
                    backgroundDim = new Box
                    {
                        Colour = Color4.Black,
                        RelativeSizeAxes = Axes.Both,
                        Alpha = Editor.BackgroundDim,
                    },
                    backFlash = new PreviewFlashLayer { Clock = EditorClock },
                    rulesetWrapper = new Container { RelativeSizeAxes = Axes.Both }
                }),
                frontFlash = new PreviewFlashLayer { Clock = EditorClock },
                pulseEffect = new PulseEffect(Map.MapEvents.PulseEvents) { Clock = EditorClock }
            ]),
            loading = new LoadingIcon
            {
                Size = new Vector2(32),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        ];
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        backgroundVideo.LoadVideo(Map.MapInfo);
        backgroundVideo.Start();

        Editor.BindableBackgroundDim.BindValueChanged(e => backgroundDim.FadeTo(e.NewValue, 300));
        Editor.BindableBackgroundBlur.BindValueChanged(e => background.Add(new BlurableBackground(Map.RealmMap, e.NewValue)), true);

        Scheduler.AddOnce(idleTracker.Reset);
        Map.AnyChange += t =>
        {
            if (t is not null)
            {
                if (ignoredForRebuild.Contains(t.GetType()))
                    return;

                var type = t.GetType();

                if (ruleset?.HasReloadListener(type) ?? false)
                {
                    var objs = Map.GetObjectsOfType(type);
                    if (ruleset.TriggerReload(type, objs)) return;
                }
            }

            Scheduler.AddOnce(idleTracker.Reset);
        };

        Map.RegisterAddListener<ShaderEvent>(_ => checkShaderRebuild());
        Map.RegisterUpdateListener<ShaderEvent>(_ => checkShaderRebuild());
        Map.RegisterRemoveListener<ShaderEvent>(_ => checkShaderRebuild());

        Map.RegisterAddListener<PulseEvent>(_ => pulseEffect.Rebuild());
        Map.RegisterUpdateListener<PulseEvent>(_ => pulseEffect.Rebuild());
        Map.RegisterRemoveListener<PulseEvent>(_ => pulseEffect.Rebuild());

        registerCameraUpdate<CameraMoveEvent>();
        registerCameraUpdate<CameraScaleEvent>();
        registerCameraUpdate<CameraRotateEvent>();

        void registerCameraUpdate<T>() where T : class, ICameraEvent
        {
            Map.RegisterAddListener<T>(_ => rebuildCamera());
            Map.RegisterUpdateListener<T>(_ => rebuildCamera());
            Map.RegisterRemoveListener<T>(_ => rebuildCamera());
        }
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case Key.R when e.ShiftPressed:
                rebuildRuleset();
                rebuildCamera();
                checkShaderRebuild(true);
                pulseEffect.Rebuild();
                return true;

            default:
                return base.OnKeyDown(e);
        }
    }

    #region Shaders

    private ShaderStackContainer shaders = null!;
    private PreviewShaderHandler handler = null!;

    private ShaderStackContainer createShaderStack()
    {
        shaders = new ShaderStackContainer();

        var shaderTypes = Map.MapEvents.ShaderEvents.Select(x => x.Type).Distinct();
        rebuildShaders(shaderTypes);
        handler.ShaderStack = shaders;
        return shaders;
    }

    private void checkShaderRebuild(bool force = false)
    {
        var current = shaders.ShaderTypes;
        var shaderTypes = Map.MapEvents.ShaderEvents.Select(x => x.Type).Distinct().ToArray();

        if (!current.SequenceEqual(shaderTypes) || force)
            rebuildShaders(shaderTypes);
    }

    private void rebuildShaders(IEnumerable<ShaderType> types)
    {
        shaders.ClearShaders();

        foreach (var type in types)
        {
            var shader = ShaderStackContainer.CreateForType(type);
            if (shader is null) continue;

            shaders.AddShader(shader);
        }
    }

    #endregion

    #region Ruleset

    private void rebuildRuleset()
    {
        rulesetWrapper.Clear();
        ruleset = null;

        ruleset = createRuleset();
        rulesetWrapper.Child = ruleset;
        ruleset.FadeInFromZero(100);

        loading.Hide();
    }

    private RulesetContainer createRuleset()
    {
        var effects = Map.MapEvents.JsonCopy()!;
        effects.Compile();
        effects.Sort();

        backFlash.Rebuild(effects.FlashEvents.Where(x => x.InBackground).ToList());
        frontFlash.Rebuild(effects.FlashEvents.Where(x => !x.InBackground).ToList());

        var auto = new AutoGenerator(Map.MapInfo, Map.RealmMap.KeyCount);
        var container = new ReplayRulesetContainer(auto.Generate(), Map.MapInfo, effects, [new NoFailMod()]);
        container.ScrollSpeed = userScrollSpeed;
        container.ParentClock = EditorClock;
        return container;
    }

    #endregion

    #region Camera

    private CameraContainer camera = null!;

    private void rebuildCamera()
    {
        var events = Map.MapEvents.Where(x => x is ICameraEvent).Cast<ICameraEvent>().ToList();
        camera.Refresh(events);
    }

    #endregion
}
