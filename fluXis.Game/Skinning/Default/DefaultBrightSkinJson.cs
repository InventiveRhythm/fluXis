using osu.Framework.Graphics;

namespace fluXis.Game.Skinning.Default;

public class DefaultBrightSkinJson : DefaultSkinJson
{
    public override Colour4 GetLaneColor(int lane, int maxLanes) => base.GetLaneColor(lane, maxLanes).Lighten(.6f);
}
