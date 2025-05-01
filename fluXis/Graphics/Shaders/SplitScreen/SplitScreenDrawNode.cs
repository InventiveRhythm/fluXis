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

namespace fluXis.Graphics.Shaders.SplitScreen;

public partial class SplitScreenContainer
{
    private class SplitScreenContainerDrawNode : ShaderDrawNode<SplitScreenContainer>
    {
        private float strength;
        private IUniformBuffer<SplitScreenParameters> parametersBuffer;

        private int splitsX = 2;
        private int splitsY = 2;

        public SplitScreenContainerDrawNode(ShaderContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();

            strength = Source.Strength;
            splitsX = (int) Source.Strength2;
            splitsY = (int) Source.Strength3;
        }

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);

            if (strength > 0)
                drawFrameBuffer(renderer);
        }

        private void drawFrameBuffer(IRenderer renderer)
        {
            parametersBuffer ??= renderer.CreateUniformBuffer<SplitScreenParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();
            
            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                parametersBuffer.Data = parametersBuffer.Data with
                {
                    TexSize = current.Size,
                    SplitsInv = new Vector2(1.0f / splitsX, 1.0f / splitsY),
                    Strength = strength,
                    SplitsX = splitsX,
                    SplitsY = splitsY,
                };

                Shader.BindUniformBlock("m_SplitScreenParameters", parametersBuffer);
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
        private record struct SplitScreenParameters
        {
            public UniformVector2 TexSize;
            public UniformVector2 SplitsInv;
            public UniformFloat Strength;
            public UniformInt SplitsX;
            public UniformInt SplitsY;
            private readonly UniformPadding4 pad1;
        }
    }
}
