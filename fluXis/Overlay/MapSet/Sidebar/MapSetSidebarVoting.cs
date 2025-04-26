using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Online.API.Models.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.MapSet.Sidebar;

public partial class MapSetSidebarVoting : FillFlowContainer
{
    private APIMapSet set { get; }

    public MapSetSidebarVoting(APIMapSet set)
    {
        this.set = set;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(6);

        var total = set.UpVotes + set.DownVotes;
        var progress = total == 0 ? .5f : set.UpVotes / (float)total;

        InternalChildren = new Drawable[]
        {
            new ForcedHeightText
            {
                Text = "Voting",
                WebFontSize = 20,
                Height = 28,
                Margin = new MarginPadding { Bottom = 6 }
            },
            new CircularContainer
            {
                RelativeSizeAxes = Axes.X,
                Height = 8,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.VoteDown
                    },
                    new Circle
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.VoteUp,
                        Width = progress
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 20,
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = $"{set.UpVotes}",
                        WebFontSize = 14,
                        Colour = FluXisColors.VoteUp,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    new FluXisSpriteText
                    {
                        Text = $"{(int)(progress * 100)}%",
                        WebFontSize = 14,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    },
                    new FluXisSpriteText
                    {
                        Text = $"{set.DownVotes}",
                        WebFontSize = 14,
                        Colour = FluXisColors.VoteDown,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight
                    }
                }
            }
        };
    }
}
