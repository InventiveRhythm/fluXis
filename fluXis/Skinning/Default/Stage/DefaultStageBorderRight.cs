using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Skinning.Default.Stage;

public partial class DefaultStageBorderRight : ColorableSkinDrawable
{
    private Box border;

    public DefaultStageBorderRight(SkinJson skinJson)
        : base(skinJson, MapColor.Secondary)
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
                Colour = Theme.Background3
            },
            border = new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = DefaultSkin.BORDER_COLOR,
                Margin = new MarginPadding { Left = DefaultSkin.BORDER_BASE },
                Colour = GetIndexOrFallback(2, Theme.Primary)
            }
        };
    }

    public override void SetColor(Colour4 color) => border.Colour = color;
}
