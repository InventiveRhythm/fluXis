using fluXis.Game.Map.Structures.Events;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Graphics.Shaders.Bloom;

public partial class BloomContainer : ShaderContainer
{
    protected override string FragmentShader => "Blur";
    public override ShaderType Type => ShaderType.Bloom;

    public BloomContainer()
    {
        DrawOriginal = true;
        EffectBlending = BlendingParameters.Additive;
        EffectPlacement = EffectPlacement.InFront;
    }

    protected override DrawNode CreateShaderDrawNode() => new BloomContainerDrawNode(this, SharedData);
}
