using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;

namespace fluXis.Game.Graphics.Shaders;

public partial class ShaderContainer
{
    protected class ShaderDrawNode<T> : BufferedDrawNode, ICompositeDrawNode where T : ShaderContainer
    {
        protected new T Source => (T)base.Source;
        protected new CompositeDrawableDrawNode Child => (CompositeDrawableDrawNode)base.Child;

        public List<DrawNode> Children
        {
            get => Child.Children;
            set => Child.Children = value;
        }

        public bool AddChildDrawNodes => RequiresRedraw;

        private bool drawOriginal;
        private ColourInfo effectColour;
        private BlendingParameters effectBlending;
        private EffectPlacement effectPlacement;

        private long updateVersion;

        protected IShader Shader { get; private set; }

        public ShaderDrawNode(ShaderContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, new CompositeDrawableDrawNode(source), sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();

            effectColour = Source.EffectColour;
            effectBlending = Source.DrawEffectBlending;
            effectPlacement = Source.EffectPlacement;

            drawOriginal = Source.DrawOriginal;
            updateVersion = Source.updateVersion;
            Shader = Source.shader;
        }

        protected override long GetDrawVersion() => updateVersion;

        protected override void DrawContents(IRenderer renderer)
        {
            if (drawOriginal && effectPlacement == EffectPlacement.InFront)
                base.DrawContents(renderer);

            renderer.SetBlend(effectBlending);

            ColourInfo finalEffectColour = DrawColourInfo.Colour;
            finalEffectColour.ApplyChild(effectColour);

            renderer.DrawFrameBuffer(SharedData.CurrentEffectBuffer, DrawRectangle, finalEffectColour);

            if (drawOriginal && effectPlacement == EffectPlacement.Behind)
                base.DrawContents(renderer);
        }
    }
}
