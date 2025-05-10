using System;
using System.Runtime.InteropServices;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Graphics.Shaders.Reflections;

public partial class ReflectionsContainer
{
    private class ReflectionsContainerDrawNode : ShaderDrawNode<ReflectionsContainer>
    {
        private float strength;
        private IUniformBuffer<ReflectionsParameters> parametersBuffer;

        private float scale;
        public ReflectionsContainerDrawNode(ReflectionsContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();

            strength = Source.Strength;
            scale = Source.Strength2;
        }

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);

            if (strength != 0)
                drawFrameBuffer(renderer);
        }

        private void drawFrameBuffer(IRenderer renderer)
        {
            parametersBuffer ??= renderer.CreateUniformBuffer<ReflectionsParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();
            
            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                parametersBuffer.Data = parametersBuffer.Data with
                {
                    TexSize = current.Size,
                    Strength = strength,
                    Scale = scale,
                };

                Shader.BindUniformBlock("m_ReflectionsParameters", parametersBuffer);
                Shader.Bind();
                renderer.DrawFrameBuffer(current, new RectangleF(0, 0, current.Texture.Width, current.Texture.Height), ColourInfo.SingleColour(Color4.White));
                Shader.Unbind();
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            parametersBuffer?.Dispose();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct ReflectionsParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat Strength;
            public UniformFloat Scale;
        }
    }
}
