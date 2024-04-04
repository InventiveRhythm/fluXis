using fluXis.Game.Skinning.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Skinning.Default.Receptor;

public partial class DefaultReceptorDown : DefaultReceptorUp
{
    public DefaultReceptorDown(SkinJson skinJson)
        : base(skinJson)
    {
    }

    protected override void SetColor(Colour4 color) => Diamond.BorderColour = color;
}
