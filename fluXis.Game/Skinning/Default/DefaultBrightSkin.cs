using fluXis.Game.Skinning.Json;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics.Textures;

namespace fluXis.Game.Skinning.Default;

public class DefaultBrightSkin : DefaultSkin
{
    public DefaultBrightSkin(TextureStore textures, ISampleStore samples)
        : base(textures, samples)
    {
    }

    protected override SkinJson CreateJson() => new DefaultBrightSkinJson();
}
