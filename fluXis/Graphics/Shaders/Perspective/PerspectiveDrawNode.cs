using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK.Graphics;

namespace fluXis.Graphics.Shaders.Perspective;

public partial class PerspectiveContainer
{
    private class PerspectiveContainerDrawNode : ShaderDrawNode<PerspectiveContainer>
    {
        private float strength;
        private float strength2;
        private float strength3;
        
        private IUniformBuffer<PerspectiveParameters> parametersBuffer;

        public PerspectiveContainerDrawNode(PerspectiveContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();

            strength = Source.Strength;
            strength2 = Source.Strength2;
            strength3 = Source.Strength3;
        }

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);
            if (strength != 0 || strength2 != 0 || strength3 != 0)
                drawFrameBuffer(renderer);
        }

        private void drawFrameBuffer(IRenderer renderer)
        {
            parametersBuffer ??= renderer.CreateUniformBuffer<PerspectiveParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                parametersBuffer.Data = parametersBuffer.Data with
                {
                    TexSize = current.Size,
                    Strength = strength,
                    Strength2 = strength2,
                    Strength3 = strength3,
                };

                Shader.BindUniformBlock("m_PerspectiveParameters", parametersBuffer);
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
        private record struct PerspectiveParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat Strength;
            public UniformFloat Strength2;
            public UniformFloat Strength3;
            private readonly UniformPadding12 pad1;
        }
    }
}