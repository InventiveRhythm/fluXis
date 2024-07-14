using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.Shaders.Vignette;

public partial class VignetteContainer
{
    private class VignetteContainerDrawNode : ShaderDrawNode<VignetteContainer>
    {
        private float strength;
        private IUniformBuffer<VignetteParameters> parametersBuffer;

        public VignetteContainerDrawNode(VignetteContainer source, BufferedDrawNodeSharedData sharedData)
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
                drawFrameBuffer(renderer, strength);
        }

        private void drawFrameBuffer(IRenderer renderer, float strength)
        {
            parametersBuffer ??= renderer.CreateUniformBuffer<VignetteParameters>();

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

                Shader.BindUniformBlock("m_VignetteParameters", parametersBuffer);
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
        private record struct VignetteParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat Strength;
            public readonly UniformPadding4 Pad;
        }
    }
}
