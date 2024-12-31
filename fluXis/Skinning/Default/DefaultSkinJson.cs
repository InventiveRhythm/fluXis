using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Json;
using osu.Framework.Graphics;

namespace fluXis.Skinning.Default;

public class DefaultSkinJson : SkinJson
{
    public override Colour4 GetLaneColor(int lane, int maxLanes)
        => FluXisColors.GetLaneColor(lane, maxLanes).Lighten(.2f);
}
