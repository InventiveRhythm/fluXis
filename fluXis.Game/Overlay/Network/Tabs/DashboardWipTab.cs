using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Overlay.Network.Tabs;

public abstract partial class DashboardWipTab : DashboardTab
{
    protected DashboardWipTab() { }

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Child = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Direction = FillDirection.Vertical,
            Children = new Drawable[]
            {
                new FluXisSpriteIcon
                {
                    Icon = Icon,
                    Size = new Vector2(38),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Margin = new MarginPadding { Bottom = 20 }
                },
                new FluXisSpriteText
                {
                    Text = Title,
                    FontSize = 30,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                },
                new FluXisSpriteText
                {
                    Text = "This tab is still work in progress.",
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                }
            }
        };
    }
}
