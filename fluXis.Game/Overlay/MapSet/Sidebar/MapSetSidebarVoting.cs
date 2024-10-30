using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Shared.Components.Maps;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Overlay.MapSet.Sidebar;

public partial class MapSetSidebarVoting : FillFlowContainer
{
    private Bindable<APIMap> mapBind { get; }

    private Circle progress;
    private FluXisSpriteText upCount;
    private FluXisSpriteText percent;
    private FluXisSpriteText downCount;

    public MapSetSidebarVoting(Bindable<APIMap> mapBind)
    {
        this.mapBind = mapBind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(6);

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
                    progress = new Circle
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.VoteUp,
                        Width = 0.5f
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 20,
                Children = new Drawable[]
                {
                    upCount = new FluXisSpriteText
                    {
                        WebFontSize = 14,
                        Colour = FluXisColors.VoteUp,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    percent = new FluXisSpriteText
                    {
                        WebFontSize = 14,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    },
                    downCount = new FluXisSpriteText
                    {
                        WebFontSize = 14,
                        Colour = FluXisColors.VoteDown,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        mapBind.BindValueChanged(e =>
        {
            var map = e.NewValue;
            upCount.Text = $"{map.UpVotes}";
            downCount.Text = $"{map.DownVotes}";

            var total = map.UpVotes + map.DownVotes;
            var p = total == 0 ? .5f : map.UpVotes / (float)total;
            percent.Text = $"{(int)(p * 100)}%";
            progress.ResizeWidthTo(p, 300, Easing.OutQuint);
        }, true);
    }
}
