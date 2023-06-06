using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsDivider : CircularContainer
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;
        CornerRadius = 5;
        RelativeSizeAxes = Axes.X;
        Height = 3;
        Margin = new MarginPadding { Vertical = 10 };
        EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Colour = Colour4.Black.Opacity(0.2f),
            Radius = 1,
            Offset = new Vector2(0, 1)
        };

        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Alpha = 0.5f
        };
    }
}
