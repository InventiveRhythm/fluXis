using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Tabs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Club.Tabs;

public partial class ClubScoresTab : TabContainer
{
    public override IconUsage Icon => FontAwesome6.Solid.ArrowTrendUp;
    public override string Title => "Scores";

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Padding = new MarginPadding { Top = 48 },
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = "We're still working on this!",
                    WebFontSize = 20,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                },
                new FluXisSpriteText
                {
                    Text = "Please check back later...",
                    WebFontSize = 16,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Alpha = .8f
                }
            }
        };
    }
}
