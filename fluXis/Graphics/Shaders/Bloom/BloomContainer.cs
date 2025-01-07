using fluXis.Configuration;
using fluXis.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Graphics.Shaders.Bloom;

public partial class BloomContainer : ShaderContainer
{
    protected override string FragmentShader => "Blur";
    public override ShaderType Type => ShaderType.Bloom;

    private Bindable<bool> disable;

    public BloomContainer()
    {
        DrawOriginal = true;
        EffectBlending = BlendingParameters.Additive;
        EffectPlacement = EffectPlacement.InFront;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        disable = config.GetBindable<bool>(FluXisSetting.DisableBloom);
    }

    protected override DrawNode CreateShaderDrawNode() => new BloomContainerDrawNode(this, SharedData);
}
