using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Online.API.Requests.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Network.Tabs.Online;
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

    [Resolved]
    private Fluxel fluxel { get; set; }

    private bool visible;
    private FillFlowContainer flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = flow = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.Both,
            Direction = FillDirection.Full,
            Spacing = new Vector2(20)
        };
    }

    public override void Enter()
    {
        visible = true;
        flow.Clear();

        try
        {
            var req = new OnlineUsersRequest();
            req.Perform(fluxel);

            if (req.Response.Status == 200)
            {
                foreach (var user in req.Response.Data.Users)
                {
                    Schedule(() =>
                    {
                        if (!visible) return;
                        flow.Add(new UserCard(user));
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
