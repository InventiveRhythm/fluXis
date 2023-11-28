using System.Collections.Generic;
using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.Shaders.Chromatic;

public partial class ChromaticContainer<T>
{
    private class ChromaticContainerDrawNode : BufferedDrawNode, ICompositeDrawNode
    {
        protected new ChromaticContainer<T> Source => (ChromaticContainer<T>)base.Source;
        protected new CompositeDrawableDrawNode Child => (CompositeDrawableDrawNode)base.Child;

        public List<DrawNode> Children
        {
            get => Child.Children;
            set => Child.Children = value;
        }

        public bool AddChildDrawNodes => RequiresRedraw;

        private float strength;

        private long updateVersion;

        private IShader chromaticShader;

        private IUniformBuffer<ChromaticParameters> chromaticParametersBuffer;

        public ChromaticContainerDrawNode(ChromaticContainer<T> source, ChromaticDrawData sharedData)
            : base(source, new CompositeDrawableDrawNode(source), sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();

            updateVersion = Source.updateVersion;

            strength = Source.Strength;

            chromaticShader = Source.chromaticShader;
        }

        protected override long GetDrawVersion() => updateVersion;

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);

            if (strength > 0)
            {
                renderer.PushScissorState(false);
                drawFrameBuffer(renderer, strength);
                renderer.PopScissorState();
            }
        }

        protected override void DrawContents(IRenderer renderer)
        {
            base.DrawContents(renderer);

            renderer.SetBlend(BlendingParameters.Inherit);

            ColourInfo finalEffectColour = DrawColourInfo.Colour;
            finalEffectColour.ApplyChild(Colour4.White);

            renderer.DrawFrameBuffer(SharedData.CurrentEffectBuffer, DrawRectangle, finalEffectColour);
        }

        private void drawFrameBuffer(IRenderer renderer, float strength)
        {
            chromaticParametersBuffer ??= renderer.CreateUniformBuffer<ChromaticParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                chromaticParametersBuffer.Data = chromaticParametersBuffer.Data with
                {
                    TexSize = new Vector2(current.Texture.Width, current.Texture.Height),
                    Radius = strength
                };

                chromaticShader.BindUniformBlock("m_ChromaticParameters", chromaticParametersBuffer);
                chromaticShader.Bind();
                renderer.DrawFrameBuffer(current, new RectangleF(0, 0, current.Texture.Width, current.Texture.Height), ColourInfo.SingleColour(Color4.White));
                chromaticShader.Unbind();
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            chromaticParametersBuffer?.Dispose();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct ChromaticParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat Radius;
            private readonly UniformPadding8 pad1;
        }
    }

    private class ChromaticDrawData : BufferedDrawNodeSharedData
    {
        public ChromaticDrawData(RenderBufferFormat[] mainBufferFormats, bool pixelSnapping, bool clipToRootNode)
            : base(2, mainBufferFormats, pixelSnapping, clipToRootNode)
        {
        }
    }
}
