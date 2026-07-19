using System.Collections.Generic;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Leaderboards;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Navigator.Pages.Rankings;

public partial class NavigatorRankingsPage : NavigatorPage<List<APIUser>>
{
    public override string Path => "/rankings/overall";
    protected override float ContentWidth => 960;

    protected override List<APIUser> PullData()
    {
        var req = new OverallRatingLeaderboardRequest();
        API.PerformRequest(req);
        req.ThrowIfFailed();
        return req.Response.Data;
    }

    protected override Drawable CreateContent(List<APIUser> data)
    {
        var flow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Spacing = new Vector2(24),
            Child = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(12),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Children = new Drawable[]
                {
                    new FluXisSpriteIcon
                    {
                        Size = new Vector2(48),
                        Icon = Phosphor.Bold.GlobeHemisphereWest,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Spacing = new Vector2(-4),
                        Children =
                        [
                            new FluXisSpriteText
                            {
                                Text = "Global",
                                WebFontSize = 24
                            },
                            new FluXisSpriteText
                            {
                                Text = "Overall Rating",
                                WebFontSize = 16,
                                Alpha = .8f
                            }
                        ]
                    }
                }
            }
        };

        var table = new RankingsTable { Anchor = Anchor.TopCentre, Origin = Anchor.TopCentre };
        table.SetData(data);
        flow.Add(table);

        return flow;
    }
}
