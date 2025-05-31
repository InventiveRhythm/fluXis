using fluXis.Graphics.Sprites.Text;
using fluXis.Online.API.Models.Clubs;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Club.Sidebar;

public partial class ClubSidebarStats : FillFlowContainer
{
    private APIClub club { get; }

    public ClubSidebarStats(APIClub club)
    {
        this.club = club;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(8);

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = "Statistics",
                WebFontSize = 24
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new StatsEntry("Rank", "#-"),
                    new StatsEntry("Overall Rating", $"{club.Statistics!.OverallRating}"),
                    new StatsEntry("Total Score", "---,---"),
                    new StatsEntry("Average Rank", "#-"),
                }
            }
        };
    }

    private partial class StatsEntry : CompositeDrawable
    {
        private string text { get; }
        private string value { get; }

        public StatsEntry(string text, string value)
        {
            this.text = text;
            this.value = value;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 20;

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = text,
                    WebFontSize = 12,
                    Alpha = .8f
                },
                new FluXisSpriteText
                {
                    Text = value,
                    WebFontSize = 12,
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight
                }
            };
        }
    }
}
