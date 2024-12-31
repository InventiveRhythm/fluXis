using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Graphics.Shaders.Bloom;

public partial class BufferedBloomContainer : BufferedContainer
{
    public BufferedBloomContainer()
    {
        DrawOriginal = true;
        EffectBlending = BlendingParameters.Additive;
        EffectPlacement = EffectPlacement.InFront;
        BlurSigma = new Vector2(40);
    }
}
