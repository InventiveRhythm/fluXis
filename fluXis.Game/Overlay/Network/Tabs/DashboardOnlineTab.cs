using System;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Network.Tabs.Online;
using Newtonsoft.Json;
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
    public override IconUsage Icon => FontAwesome.Solid.Globe;

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
            var req = fluxel.CreateAPIRequest("/users/online");
            req.Perform();

            var json = JsonConvert.DeserializeObject<APIResponse<APIOnlineUsers>>(req.GetResponseString());

            if (json.Status == 200)
            {
                foreach (var user in json.Data.Users)
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
