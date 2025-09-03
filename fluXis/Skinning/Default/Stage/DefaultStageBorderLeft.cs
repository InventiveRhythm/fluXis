using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Skinning.Default.Stage;

public partial class DefaultStageBorderLeft : ColorableSkinDrawable
{
    private Box border;

    public DefaultStageBorderLeft(SkinJson skinJson)
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
                Margin = new MarginPadding { Left = DefaultSkin.BORDER_COLOR },
                Colour = Theme.Background3
            },
            border = new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = DefaultSkin.BORDER_COLOR,
                Colour = GetIndexOrFallback(1, Theme.Secondary)
            }
        };
    }

    public override void SetColor(Colour4 color) => border.Colour = color;

    public override void FadeColor(Colour4 color, double startTime, double duration = 0, Easing easing = Easing.None)
    {
        using (BeginAbsoluteSequence(startTime))
            border.FadeColour(color, duration, easing);
    }
}
