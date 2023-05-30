using fluXis.Game.Graphics;

namespace fluXis.Game.Skinning.Default.Receptor;

public partial class DefaultReceptorDown : DefaultReceptorUp
{
    public override void UpdateColor(int lane, int keyCount) => Diamond.BorderColour = FluXisColors.GetLaneColor(lane, keyCount);
}
