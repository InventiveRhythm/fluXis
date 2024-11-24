using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Skinning.Bases;
using fluXis.Game.Skinning.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Stage;

public partial class DefaultHitLine : ColorableSkinDrawable
{
    public DefaultHitLine(SkinJson skinJson)
        : base(skinJson)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.TopCentre;
        Height = 3;
        Colour = ColourInfo.GradientHorizontal(GetIndexOrFallback(1, FluXisColors.Secondary), GetIndexOrFallback(2, FluXisColors.Primary));

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }
}
