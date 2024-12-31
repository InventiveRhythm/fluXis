using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK.Graphics;

namespace fluXis.Graphics.Shaders.Mosaic;

public partial class MosaicContainer
{
    private class MosaicDrawNode : ShaderDrawNode<MosaicContainer>
    {
        private float strength;
        private IUniformBuffer<MosaicParameters> parametersBuffer;

        public MosaicDrawNode(MosaicContainer source, BufferedDrawNodeSharedData sharedData)
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
            parametersBuffer ??= renderer.CreateUniformBuffer<MosaicParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                parametersBuffer.Data = parametersBuffer.Data with
                {
                    TexSize = current.Size,
                    Strength = strength
                };

                Shader.BindUniformBlock("m_MosaicParameters", parametersBuffer);
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
        private record struct MosaicParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat Strength;
            private readonly UniformPadding4 pad1;
        }
    }
}
