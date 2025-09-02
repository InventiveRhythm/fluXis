using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Skinning.Default.Stage;

public partial class DefaultHitLine : ColorableSkinDrawable
{
    public DefaultHitLine(SkinJson skinJson)
        : base(skinJson, MapColor.Other)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.TopCentre;
        Height = 3;
        Colour = ColourInfo.GradientHorizontal(GetIndexOrFallback(1, Theme.Secondary), GetIndexOrFallback(2, Theme.Primary));

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }
}
