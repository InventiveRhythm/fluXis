using System;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Online.API.Requests.Users;
using fluXis.Game.Online.Drawables;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Game.Overlay.Network.Tabs;

public partial class DashboardOnlineTab : DashboardWipTab
{
    public override string Title => "Online";
    public override IconUsage Icon => FontAwesome6.Solid.EarthAmericas;
    public override DashboardTabType Type => DashboardTabType.Online;

    [Resolved]
    private IAPIClient fluxel { get; set; }

    private bool visible;
    private FillFlowContainer flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Child = new FluXisScrollContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Full,
                Spacing = new Vector2(20)
            }
        };
    }

    public override void Enter()
    {
        visible = true;
        flow.Clear();

        try
        {
            var req = new OnlineUsersRequest();
            fluxel.PerformRequest(req);

            if (req.IsSuccessful)
            {
                foreach (var user in req.Response.Data.Users)
                {
                    Schedule(() =>
                    {
                        if (!visible)
                            return;

                        flow.Add(new DrawableUserCard(user)
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre
                        });
                    });
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to get online users");
        }
    }

    public override void Exit()
    {
        visible = false;
    }
}
