using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK.Graphics;

namespace fluXis.Graphics.Shaders.Glitch;

public partial class GlitchContainer
{
    private class GlitchContainerDrawNode : ShaderDrawNode<GlitchContainer>
    {
        private float strength; // x strength
        private float strength2; // y strength
        private float strength3; // glitch block size
        private IUniformBuffer<GlitchParameters> parametersBuffer;

        public GlitchContainerDrawNode(GlitchContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();

            strength = Source.Strength / 10f;
            strength2 = Source.Strength2 / 10f;
            strength3 = Source.Strength3;
        }

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);

            if (strength > 0 || strength2 > 0)
                drawFrameBuffer(renderer);
        }

        private void drawFrameBuffer(IRenderer renderer)
        {
            parametersBuffer ??= renderer.CreateUniformBuffer<GlitchParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                parametersBuffer.Data = parametersBuffer.Data with
                {
                    TexSize = current.Size,
                    StrengthX = strength,
                    StrengthY = strength2,
                    BlockSize = strength3,
                    Time = (float)Source.Time.Current % 10000f
                };

                Shader.BindUniformBlock("m_GlitchParameters", parametersBuffer);
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
        private record struct GlitchParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat StrengthX;
            public UniformFloat StrengthY;
            public UniformFloat BlockSize;
            public UniformFloat Time;
            private readonly UniformPadding8 pad1;
        }
    }
}
