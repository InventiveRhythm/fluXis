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

namespace fluXis.Graphics.Shaders.Bloom;

public partial class BloomContainer
{
    private class BloomContainerDrawNode : ShaderDrawNode<BloomContainer>
    {
        private float strength;
        private IUniformBuffer<BlurParameters> parametersBuffer;

        private int radius;

        public BloomContainerDrawNode(ShaderContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, sharedData)
        {
        }

        public override void ApplyState()
        {
            Source.EffectColour = Color4.White.Opacity(Source.Strength);
            strength = 20 * Source.Strength;
            radius = Blur.KernelSize(strength);

            base.ApplyState();
        }

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);

            if (strength <= 0 || Source.disable.Value)
                return;

            renderer.PushScissorState(false);

            drawFrameBuffer(renderer, radius, strength, 0);
            drawFrameBuffer(renderer, radius, strength, 90);

            renderer.PopScissorState();
        }

        private void drawFrameBuffer(IRenderer renderer, int kernelRadius, float sigma, int rotation)
        {
            parametersBuffer ??= renderer.CreateUniformBuffer<BlurParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            using (BindFrameBuffer(target))
            {
                float radians = float.DegreesToRadians(rotation);

                parametersBuffer.Data = parametersBuffer.Data with
                {
                    Radius = kernelRadius,
                    Sigma = sigma,
                    TexSize = current.Size,
                    Direction = new Vector2(MathF.Cos(radians), MathF.Sin(radians))
                };

                Shader.BindUniformBlock("m_BlurParameters", parametersBuffer);
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
        private record struct BlurParameters
        {
            public UniformVector2 TexSize;
            public UniformInt Radius;
            public UniformFloat Sigma;
            public UniformVector2 Direction;
            private readonly UniformPadding8 pad1;
        }
    }
}
