using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Configuration;
using fluXis.Graphics.Background;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Shaders;
using fluXis.Graphics.Shaders.Bloom;
using fluXis.Graphics.Shaders.Chromatic;
using fluXis.Graphics.Shaders.Glitch;
using fluXis.Graphics.Shaders.Greyscale;
using fluXis.Graphics.Shaders.HueShift;
using fluXis.Graphics.Shaders.Invert;
using fluXis.Graphics.Shaders.Mosaic;
using fluXis.Graphics.Shaders.Noise;
using fluXis.Graphics.Shaders.Retro;
using fluXis.Graphics.Shaders.Vignette;
using fluXis.Graphics.Shaders.SplitScreen;
using fluXis.Graphics.Shaders.FishEye;
using fluXis.Graphics.Shaders.Reflections;
using fluXis.Graphics.Shaders.DrunkThing;
using fluXis.Graphics.Shaders.NeonThing;
using fluXis.Graphics.Shaders.Perspective;
using fluXis.Graphics.Sprites;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Map.Structures.Events.Camera;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Screens.Edit.Tabs.Design.Effects;
using fluXis.Screens.Edit.Tabs.Design.Points;
using fluXis.Screens.Edit.Tabs.Shared;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Overlay.Effect;
using fluXis.Screens.Gameplay.Replays;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Scripting;
using fluXis.Storyboards;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Design;

public partial class DesignContainer : EditorTabContainer
{
    protected override MarginPadding ContentPadding => new(16) { Right = 0 };

    [Resolved]
    private ScriptStorage scripts { get; set; }

    private static Type[] ignoredForRebuild { get; } =
    {
        typeof(PulseEvent),
        typeof(ShaderEvent),
        typeof(NoteEvent),
        typeof(StoryboardElement)
    };

    private DrawSizePreservingFillContainer drawSizePreserve;
    private ShaderStackContainer shaders;
    private DesignShaderHandler handler;
    private CameraContainer camera;

    private SpriteStack<BlurableBackground> backgroundStack;
    private Box backgroundDim;
    private Container rulesetWrapper;
    private LoadingIcon loadingIcon;
    private PulseEffect pulseEffect;
    private EditorFlashLayer backFlash;
    private EditorFlashLayer frontFlash;

    private IdleTracker rulesetIdleTracker;

    private Bindable<float> userScrollSpeed;
    private Bindable<float> rulesetScrollSpeed { get; } = new();

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        userScrollSpeed = config.GetBindable<float>(FluXisSetting.ScrollSpeed);
    }

    private BackgroundVideo backgroundVideo;

    protected override IEnumerable<Drawable> CreateContent()
    {
        drawSizePreserve = new DrawSizePreservingFillContainer
        {
            RelativeSizeAxes = Axes.Both,
            TargetDrawSize = new Vector2(1920, 1080),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
        };

        camera = new CameraContainer(Map.MapEvents.Where(x => x is ICameraEvent).Cast<ICameraEvent>().ToList());

        return new Drawable[]
        {
            handler = new DesignShaderHandler(),
            rulesetIdleTracker = new IdleTracker(400, rebuildRuleset, () =>
            {
                loadingIcon.Show();
                rulesetWrapper.FirstOrDefault()?.FadeOut(100);
            }),
            drawSizePreserve.WithChild(createShaderStack().AddContent(new[]
            {
                camera.CreateProxyDrawable().With(x => x.Clock = EditorClock),
                camera.WithChildren(new Drawable[]
                {
                    backgroundStack = new SpriteStack<BlurableBackground> { AutoFill = false },
                    backgroundVideo = new BackgroundVideo
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        VideoClock = EditorClock
                    },
                    backgroundDim = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = Editor.BindableBackgroundDim.Value
                    },
                    backFlash = new EditorFlashLayer { Clock = EditorClock },
                    rulesetWrapper = new Container { RelativeSizeAxes = Axes.Both },
                }),
                pulseEffect = new PulseEffect(Map.MapEvents.PulseEvents) { Clock = EditorClock },
                frontFlash = new EditorFlashLayer { Clock = EditorClock },
                loadingIcon = new LoadingIcon
                {
                    Size = new Vector2(32),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            }))
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        backgroundVideo.LoadVideo(Map.MapInfo);
        backgroundVideo.Start();

        Scheduler.AddOnce(rulesetIdleTracker.Reset);
        Map.AnyChange += t =>
        {
            if (t is not null && ignoredForRebuild.Contains(t.GetType()))
                return;

            Scheduler.AddOnce(rulesetIdleTracker.Reset);
        };

        Map.RegisterAddListener<ShaderEvent>(_ => checkForRebuild());
        Map.RegisterUpdateListener<ShaderEvent>(_ => checkForRebuild());
        Map.RegisterRemoveListener<ShaderEvent>(_ => checkForRebuild());

        Map.RegisterAddListener<PulseEvent>(_ => pulseEffect.Rebuild());
        Map.RegisterUpdateListener<PulseEvent>(_ => pulseEffect.Rebuild());
        Map.RegisterRemoveListener<PulseEvent>(_ => pulseEffect.Rebuild());

        registerCameraUpdate<CameraMoveEvent>();
        registerCameraUpdate<CameraScaleEvent>();
        registerCameraUpdate<CameraRotateEvent>();

        Editor.BindableBackgroundDim.BindValueChanged(e => backgroundDim.FadeTo(e.NewValue, 300));
        Editor.BindableBackgroundBlur.BindValueChanged(e => backgroundStack.Add(new BlurableBackground(Map.RealmMap, e.NewValue)), true);

        void registerCameraUpdate<T>() where T : class, ICameraEvent
        {
            Map.RegisterAddListener<T>(_ => rebuildCamera());
            Map.RegisterUpdateListener<T>(_ => rebuildCamera());
            Map.RegisterRemoveListener<T>(_ => rebuildCamera());
        }
    }

    protected override void Update()
    {
        base.Update();

        rulesetScrollSpeed.Value = (float)(userScrollSpeed.Value * (Settings.Zoom / 2f));
    }

    private RulesetContainer createRuleset()
    {
        var effects = Map.MapEvents.JsonCopy();
        effects.RunScripts(scripts);

        backFlash.Rebuild(effects.FlashEvents.Where(x => x.InBackground).ToList());
        frontFlash.Rebuild(effects.FlashEvents.Where(x => !x.InBackground).ToList());

        var auto = new AutoGenerator(Map.MapInfo, Map.RealmMap.KeyCount);
        var container = new ReplayRulesetContainer(auto.Generate(), Map.MapInfo, effects, new List<IMod> { new NoFailMod() });
        container.ScrollSpeed = rulesetScrollSpeed;
        container.ParentClock = EditorClock;

        return container;
    }

    private ShaderStackContainer createShaderStack()
    {
        shaders = new ShaderStackContainer();

        var shaderTypes = Map.MapEvents.ShaderEvents.Select(x => x.Type).Distinct();

        foreach (var type in shaderTypes)
        {
            ShaderContainer shader = type switch
            {
                ShaderType.Chromatic => new ChromaticContainer(),
                ShaderType.Greyscale => new GreyscaleContainer(),
                ShaderType.Invert => new InvertContainer(),
                ShaderType.Bloom => new BloomContainer(),
                ShaderType.Mosaic => new MosaicContainer(),
                ShaderType.Noise => new NoiseContainer(),
                ShaderType.Vignette => new VignetteContainer(),
                ShaderType.Retro => new RetroContainer(),
                ShaderType.HueShift => new HueShiftContainer(),
                ShaderType.Glitch => new GlitchContainer(),
                ShaderType.SplitScreen => new SplitScreenContainer(),
                ShaderType.FishEye => new FishEyeContainer(),
                ShaderType.Reflections => new ReflectionsContainer(),
                ShaderType.DrunkThing => new DrunkThingContainer(),
                ShaderType.NeonThing => new NeonThingContainer(),
                ShaderType.Perspective => new PerspectiveContainer(),
                _ => null
            };

            if (shader is null)
                continue;

            shader.RelativeSizeAxes = Axes.Both;
            shaders.AddShader(shader);
        }

        handler.ShaderStack = shaders;
        return shaders;
    }

    private void checkForRebuild()
    {
        var current = shaders.ShaderTypes;
        var shaderTypes = Map.MapEvents.ShaderEvents.Select(x => x.Type).Distinct();

        if (!current.SequenceEqual(shaderTypes))
            rebuildShaderStack();
    }

    private void rebuildShaderStack()
    {
        var content = shaders.RemoveContent();
        drawSizePreserve.Clear();
        drawSizePreserve.Add(createShaderStack().AddContent(content.ToArray()));
    }

    private void rebuildCamera()
    {
        var events = Map.MapEvents.Where(x => x is ICameraEvent).Cast<ICameraEvent>().ToList();
        camera.Refresh(events);
    }

    private void rebuildRuleset()
    {
        rulesetWrapper.Clear();

        var ruleset = createRuleset();
        rulesetWrapper.Child = ruleset;
        ruleset.FadeInFromZero(100);

        loadingIcon.Hide();
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case Key.R when e.ShiftPressed:
                rebuildRuleset();
                rebuildShaderStack();
                return true;

            default:
                return base.OnKeyDown(e);
        }
    }

    protected override Container CreateContentContainer() => new AspectRatioContainer(Settings.ForceAspectRatio);
    protected override Drawable CreateLeftSide() => Empty();
    protected override PointsSidebar CreatePointsSidebar() => new DesignSidebar();
}
