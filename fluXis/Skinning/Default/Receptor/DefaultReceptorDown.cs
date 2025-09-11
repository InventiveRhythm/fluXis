using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Json;
using osu.Framework.Graphics;

namespace fluXis.Skinning.Default.Receptor;

public partial class DefaultReceptorDown : DefaultReceptorUp
{
    public DefaultReceptorDown(SkinJson skinJson, MapColor index)
        : base(skinJson, index)
    {
    }

    public override void SetColor(Colour4 color) => Diamond.BorderColour = color;
}
