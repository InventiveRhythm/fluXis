using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Settings;

public partial class SettingsSubSection : FillFlowContainer
{
    [Resolved]
    protected FluXisConfig Config { get; private set; }

    public virtual LocalisableString Title => "Subsection";
    public virtual IconUsage Icon => FontAwesome6.Solid.AngleRight;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 10);

        InternalChild = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(10, 0),
            Children = new Drawable[]
            {
                new FluXisSpriteIcon
                {
                    Icon = Icon,
                    Size = new Vector2(24),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                },
                new FluXisSpriteText
                {
                    Text = Title,
                    FontSize = 38,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                }
            }
        };
    }
}
