using System;
using System.Collections.Generic;
using System.Linq;
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
using fluXis.Graphics.Sprites;
using fluXis.Map.Structures.Events;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Screens.Edit.Tabs.Design.Effects;
using fluXis.Screens.Edit.Tabs.Design.Points;
using fluXis.Screens.Edit.Tabs.Shared;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using fluXis.Screens.Gameplay.Overlay.Effect;
using fluXis.Screens.Gameplay.Replays;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Scripting;
using fluXis.Storyboards;
using fluXis.Utils;
using osu.Framework.Allocation;
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

    private SpriteStack<BlurableBackground> backgroundStack;
    private Box backgroundDim;
    private Container rulesetWrapper;
    private LoadingIcon loadingIcon;
    private PulseEffect pulseEffect;
    private EditorFlashLayer backFlash;
    private EditorFlashLayer frontFlash;

    private IdleTracker rulesetIdleTracker;

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
        return new Drawable[]
        {
            handler = new DesignShaderHandler(),
            rulesetIdleTracker = new IdleTracker(400, rebuildRuleset, () =>
            {
                loadingIcon.Show();
                rulesetWrapper.FirstOrDefault()?.FadeOut(100);
            }),
            drawSizePreserve.WithChild(createShaderStack().AddContent(new Drawable[]
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
                pulseEffect = new PulseEffect(Map.MapEvents.PulseEvents) { Clock = EditorClock },
                loadingIcon = new LoadingIcon
                {
                    Size = new Vector2(32),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                frontFlash = new EditorFlashLayer { Clock = EditorClock }
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

        Map.ShaderEventAdded += _ => checkForRebuild();
        Map.ShaderEventUpdated += _ => checkForRebuild();
        Map.ShaderEventRemoved += _ => checkForRebuild();

        Map.PulseEventAdded += _ => pulseEffect.Rebuild();
        Map.PulseEventUpdated += _ => pulseEffect.Rebuild();
        Map.PulseEventRemoved += _ => pulseEffect.Rebuild();

        Editor.BindableBackgroundDim.BindValueChanged(e => backgroundDim.FadeTo(e.NewValue, 300));
        Editor.BindableBackgroundBlur.BindValueChanged(e => backgroundStack.Add(new BlurableBackground(Map.RealmMap, e.NewValue)), true);
    }

    private RulesetContainer createRuleset()
    {
        var effects = Map.MapEvents.JsonCopy();
        effects.RunScripts(scripts);

        backFlash.Rebuild(effects.FlashEvents.Where(x => x.InBackground).ToList());
        frontFlash.Rebuild(effects.FlashEvents.Where(x => !x.InBackground).ToList());

        var auto = new AutoGenerator(Map.MapInfo, Map.RealmMap.KeyCount);
        var container = new ReplayRulesetContainer(auto.Generate(), Map.MapInfo, effects, new List<IMod> { new NoFailMod() });
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
