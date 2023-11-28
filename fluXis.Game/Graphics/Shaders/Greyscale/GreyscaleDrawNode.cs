using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.Shaders.Greyscale;

public partial class GreyscaleContainer
{
    private class GreyscaleDrawNode : ShaderDrawNode<GreyscaleContainer>
    {
        private IUniformBuffer<GreyscaleParameters> parameters;

        public GreyscaleDrawNode(ShaderContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, sharedData)
        {
        }

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);
            drawFrameBuffer(renderer);
        }

        private void drawFrameBuffer(IRenderer renderer)
        {
            parameters ??= renderer.CreateUniformBuffer<GreyscaleParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                parameters.Data = parameters.Data with { TexSize = current.Size };

                Shader.BindUniformBlock("m_GreyscaleParameters", parameters);
                Shader.Bind();
                renderer.DrawFrameBuffer(current, new RectangleF(0, 0, current.Texture.Width, current.Texture.Height), ColourInfo.SingleColour(Color4.White));
                Shader.Unbind();
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            parameters?.Dispose();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct GreyscaleParameters
        {
            public UniformVector2 TexSize;
            private readonly UniformPadding4 pad1;
            private readonly UniformPadding4 pad2;
        }
    }
}
