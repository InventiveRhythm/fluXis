using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Skinning.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Stage;

public partial class DefaultStageBorderLeft : DefaultSkinDrawable
{
    public DefaultStageBorderLeft(SkinJson skinJson)
        : base(skinJson)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.TopLeft;
        Origin = Anchor.TopRight;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 5,
                Margin = new MarginPadding { Left = 2 },
                Colour = FluXisColors.Background3
            },
            new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 2,
                Alpha = .5f,
                Colour = GetIndexOrFallback(1, FluXisColors.Accent3)
            }
        };
    }
}
