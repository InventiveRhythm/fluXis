using System.Runtime.InteropServices;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;
using osuTK.Graphics;

namespace fluXis.Graphics.Shaders.NeonThing;

public partial class NeonThingContainer
{
    private class NeonThingContainerDrawNode : ShaderDrawNode<NeonThingContainer>
    {
        private float strength;
        private IUniformBuffer<NeonThingParameters> parametersBuffer;

        private float scale;

        public NeonThingContainerDrawNode(NeonThingContainer source, BufferedDrawNodeSharedData sharedData)
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
            if (strength > 0)
                drawFrameBuffer(renderer);
        }

        private void drawFrameBuffer(IRenderer renderer)
        {
            parametersBuffer ??= renderer.CreateUniformBuffer<NeonThingParameters>();

            IFrameBuffer current = SharedData.CurrentEffectBuffer;
            IFrameBuffer target = SharedData.GetNextEffectBuffer();

            renderer.SetBlend(BlendingParameters.None);

            using (BindFrameBuffer(target))
            {
                parametersBuffer.Data = parametersBuffer.Data with
                {
                    TexSize = current.Size,
                    Time = (float)(Source.Clock?.CurrentTime ?? 0),
                    Strength = strength,
                    Scale = scale,
                };

                Shader.BindUniformBlock("m_NeonThingParameters", parametersBuffer);
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
        private record struct NeonThingParameters
        {
            public UniformVector2 TexSize;
            public UniformFloat Time;
            public UniformFloat Strength;
            public UniformFloat Scale;
            private readonly UniformPadding12 pad1;
        }
    }
}