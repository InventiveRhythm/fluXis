using fluXis.Game.Skinning.Json;

namespace fluXis.Game.Skinning.Default.Receptor;

public partial class DefaultReceptorDown : DefaultReceptorUp
{
    public DefaultReceptorDown(SkinJson skinJson)
        : base(skinJson)
    {
    }

    public override void UpdateColor(int lane, int keyCount) => Diamond.BorderColour = SkinJson.GetLaneColor(lane, keyCount);
}
