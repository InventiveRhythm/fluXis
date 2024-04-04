using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Skinning.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Stage;

public partial class DefaultStageBorderRight : DefaultSkinDrawable
{
    public DefaultStageBorderRight(SkinJson skinJson)
        : base(skinJson)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopLeft;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 5,
                Colour = FluXisColors.Background3
            },
            new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 2,
                Margin = new MarginPadding { Left = 5 },
                Alpha = .5f,
                Colour = GetIndexOrFallback(2, FluXisColors.Accent)
            }
        };
    }
}
