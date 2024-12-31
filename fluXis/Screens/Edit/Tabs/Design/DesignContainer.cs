using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Background;
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
using fluXis.Graphics.Sprites;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Design.Effects;
using fluXis.Screens.Edit.Tabs.Design.Playfield;
using fluXis.Screens.Edit.Tabs.Design.Points;
using fluXis.Screens.Edit.Tabs.Design.Toolbox;
using fluXis.Screens.Edit.Tabs.Shared;
using fluXis.Screens.Edit.Tabs.Shared.Points;
using fluXis.Screens.Edit.Tabs.Shared.Toolbox;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Design;

public partial class DesignContainer : EditorTabContainer
{
    private DrawSizePreservingFillContainer drawSizePreserve;
    private ShaderStackContainer shaders;
    private DesignShaderHandler handler;

    private SpriteStack<BlurableBackground> backgroundStack;
    private Box backgroundDim;

    private BackgroundVideo backgroundVideo;
    private bool showingVideo;

    protected override IEnumerable<Drawable> CreateContent()
    {
        return new Drawable[]
        {
            handler = new DesignShaderHandler(),
            drawSizePreserve = new DrawSizePreservingFillContainer
            {
                RelativeSizeAxes = Axes.Both,
                TargetDrawSize = new Vector2(1920, 1080),
                Child = createShaderStack().AddContent(new Drawable[]
                {
                    backgroundStack = new SpriteStack<BlurableBackground> { AutoFill = false },
                    backgroundVideo = new BackgroundVideo
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        VideoClock = EditorClock,
                        ShowDim = false
                    },
                    backgroundDim = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = Editor.BindableBackgroundDim.Value
                    },
                    new EditorFlashLayer { InBackground = true },
                    new EditorDesignPlayfieldManager(Map.MapInfo.DualMode),
                    new EditorFlashLayer()
                })
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        backgroundVideo.Map = Map.RealmMap;
        backgroundVideo.Info = Map.MapInfo;
        backgroundVideo.LoadVideo();

        Map.ShaderEventAdded += _ => checkForRebuild();
        Map.ShaderEventUpdated += _ => checkForRebuild();
        Map.ShaderEventRemoved += _ => checkForRebuild();

        Editor.BindableBackgroundDim.BindValueChanged(e => backgroundDim.FadeTo(e.NewValue, 300));
        Editor.BindableBackgroundBlur.BindValueChanged(e => backgroundStack.Add(new BlurableBackground(Map.RealmMap, e.NewValue)), true);
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

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        switch (e.Key)
        {
            case Key.V:
            {
                showingVideo = !showingVideo;

                if (showingVideo)
                    backgroundVideo.Start();
                else
                    backgroundVideo.Stop();

                return true;
            }

            case Key.R when e.ShiftPressed:
                rebuildShaderStack();
                return true;

            default:
                return base.OnKeyDown(e);
        }
    }

    protected override EditorToolbox CreateToolbox() => new DesignToolbox();
    protected override PointsSidebar CreatePointsSidebar() => new DesignSidebar();
}
