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
        private float speed;
        private float density;
        private IUniformBuffer<NeonThingParameters> parametersBuffer;

        private float thickness;

        public NeonThingContainerDrawNode(NeonThingContainer source, BufferedDrawNodeSharedData sharedData)
            : base(source, sharedData)
        {
        }

        public override void ApplyState()
        {
            base.ApplyState();

            speed = Source.Strength;
            density = Source.Strength2;
            thickness = Source.Strength3;
        }

        protected override void PopulateContents(IRenderer renderer)
        {
            base.PopulateContents(renderer);
            if (speed > 0)
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
                    Strength = speed,
                    Strength2 = density,
                    Strength3 = thickness,
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
            public UniformFloat Strength2;
            public UniformFloat Strength3;
            private readonly UniformPadding8 pad1;
        }
    }
}