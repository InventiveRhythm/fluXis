using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Skinning.Bases;
using fluXis.Game.Skinning.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Stage;

public partial class DefaultStageBorderRight : ColorableSkinDrawable
{
    public DefaultStageBorderRight(SkinJson skinJson)
        : base(skinJson)
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = DefaultSkin.BORDER_BASE,
                Colour = FluXisColors.Background3
            },
            new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = DefaultSkin.BORDER_COLOR,
                Margin = new MarginPadding { Left = DefaultSkin.BORDER_BASE },
                Colour = GetIndexOrFallback(2, FluXisColors.Accent)
            }
        };
    }
}
