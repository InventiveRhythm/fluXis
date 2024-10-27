using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.Shaders.Noise;

public partial class NoiseContainer
{
    private class NoiseContainerDrawNode : ShaderDrawNode<NoiseContainer>
    {
        private float strength;
        private IUniformBuffer<NoiseParameters> parametersBuffer;

        public NoiseContainerDrawNode(NoiseContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();

            strength = Source.Strength;
        }

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);

            if (strength > 0)
                drawFrameBuffer(renderer);
        }

        private void drawFrameBuffer(IRenderer renderer)
        {
            parametersBuffer ??= renderer.CreateUniformBuffer<NoiseParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                parametersBuffer.Data = new NoiseParameters
                {
                    TexSize = current.Size,
                    Strength = strength,
                    Time = (float)Source.Time.Current
                };

                Shader.BindUniformBlock("m_NoiseParameters", parametersBuffer);
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
        private record struct NoiseParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat Strength;
            public UniformFloat Time;
        }
    }
}
